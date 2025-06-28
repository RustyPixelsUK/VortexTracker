// 
// This is part of Vortex Tracker II project
// 
// (c)2000-2009 S.V.Bulba
// Author: Sergey Bulba, vorobey@mail.khstu.ru
// Support page: http://bulba.untergrund.net/
// 
// Version 1.5 - 2.6
// (c)2017-2021 Ivan Pirog, ivan.pirog@gmail.com
// 
// Version 2.6 - 2.6.1
// (c)2022-2025 Dexus (Volutar), https://github.com/Volutar
// 
// Version 3.0+ (C# port)
// (c)2025 Ben Baker, https://github.com/benbaker76

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading.Tasks;
using Timer = System.Windows.Forms.Timer;
using LibVT;
using System.Reflection;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.GLControl;

namespace VortexTracker.Rendering
{
    public class Quad2
    {
        public Texture2D Texture;
        public float[] Vertices;

        public Quad2(Texture2D texture, float[] vertices)
        {
            Texture = texture;
            Vertices = vertices;
        }
    }

    public class OpenGL
    {
        private Control _parentControl;
        private GLControl _glControl;
        private int _shaderProgram;
        private int _vao;
        private int _vbo;
        private int _dpi;
        private List<float> _vertexBuffer = new();
        private Dictionary<string, FontFnt> _fontDictionary = new();
        private Dictionary<string, Texture2D> _textureDictionary = new();
        private List<Quad2> _quadList = new();
        private Matrix4 _projMatrix;
        private Matrix4 _modelViewMatrix = Matrix4.Identity;
        private readonly object _sync = new();

        private Vector2 _caretPos = new(-1, -1);
        private Vector2 _caretSize = Vector2.Zero;
        private bool _caretVisible = false;
        private Timer _caretTimer;

        private Vector2 _selectionPos = new(-1, -1);
        private Vector2 _selectionSize = Vector2.Zero;
        private bool _selectionVisible = false;

        private Action _drawAction;

        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;
        public event EventHandler Leave;
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseMove;
        public event MouseEventHandler MouseWheel;

        public OpenGL(Control parentControl, Font[] fonts, System.Drawing.Color backColor, Action drawAction)
        {
            _parentControl = parentControl;
            _drawAction = drawAction;

            _glControl = new GLControl(new GLControlSettings())
            {
                Dock = DockStyle.Fill,
                BackColor = backColor
            };

            parentControl.Controls.Add(_glControl);

            _glControl.Resize += (_, _) => Resize();
            _glControl.Paint += (_, _) =>
            {
                _drawAction?.Invoke();
                Render();
            };

            /* _glControl.HandleCreated += (_, _) =>
            {
                Init();
            }; */

            if (_glControl.IsHandleCreated)
            {
                Init();
            }

            _glControl.KeyDown += (s, e) => KeyDown?.Invoke(s, e);
            _glControl.KeyUp += (s, e) => KeyUp?.Invoke(s, e);
            _glControl.Leave += (s, e) => Leave?.Invoke(s, e);
            _glControl.MouseDown += (s, e) => MouseDown?.Invoke(s, e);
            _glControl.MouseMove += (s, e) => MouseMove?.Invoke(s, e);
            _glControl.MouseWheel += (s, e) =>
            {
                MouseWheel?.Invoke(s, e);
                ((HandledMouseEventArgs)e).Handled = true;
            };


            foreach (Font font in fonts)
                LoadTTF(font);

            LoadBar("Bar1");
            LoadBar("Bar2");
            LoadBar("Bar3");

            _caretTimer = new Timer();
            _caretTimer.Interval = 500;
            _caretTimer.Tick += (s, e) => {
                _caretVisible = !_caretVisible;

                //if (!WaveOutAPI.IsPlaying)
                //    _glControl.Invalidate();
            };
        }

        ~OpenGL()
        {
            _caretTimer?.Dispose();
            _glControl?.Dispose();
        }

        public void LoadTTF(Font font)
        {
            FontBMOptions options = new FontBMOptions()
            {
                OriginalFontSize = (int)GetFontPixelSize(font),
                FontSize = (int)GetFontPixelSize(font),
                IncludeBlankChar = true
            };

            options.CreateChars();

            (FontFnt fontFnt, Baker76.Imaging.Image image) = Task.Run(async () => await FontFnt.LoadTTF(font, options)).GetAwaiter().GetResult();

            Texture2D texture = new Texture2D(image);

            string fontName = GetFontName(font);

            _fontDictionary[fontName] = fontFnt;
            _textureDictionary[fontName] = texture;
        }

        public string GetFontName(Font font)
        {
            return $"{font.Name}_{font.Style}_{font.Size}";
        }

        public float GetFontPixelSize(Font font)
        {
            using (System.Drawing.Graphics g = _parentControl.CreateGraphics())
            {
                float dpi = g.DpiX;

                return font.Unit switch
                {
                    GraphicsUnit.Pixel => font.Size,
                    GraphicsUnit.Point => font.Size * dpi / 72f, // 1 point = 1/72 inch
                    GraphicsUnit.Inch => font.Size * dpi,
                    GraphicsUnit.Millimeter => font.Size * dpi / 25.4f,
                    GraphicsUnit.Document => font.Size * dpi / 300f,
                    GraphicsUnit.Display => font.Size * dpi / 75f,
                    _ => font.Size
                };
            }
        }

        public void LoadBar(string name)
        {
            // Load a embedded resource
            using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"VortexTracker.Resources.Images.{name}.png"))
            {
                if (resourceStream != null)
                {
                    Baker76.Imaging.Image image = Baker76.Imaging.Image.Load(resourceStream);
                    Texture2D texture = new Texture2D(image);

                    _textureDictionary.Add(name, texture);
                }
            }
        }

        public void Init()
        {
            if (!_glControl.Context.IsCurrent)
                _glControl.MakeCurrent();

            _shaderProgram = CreateShaderProgram();

            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, 4096, IntPtr.Zero, BufferUsageHint.DynamicDraw);

            int stride = 9 * sizeof(float);
            int posLoc = GL.GetAttribLocation(_shaderProgram, "aPos");
            int texLoc = GL.GetAttribLocation(_shaderProgram, "aTexCoord");
            int colorLoc = GL.GetAttribLocation(_shaderProgram, "aColor");

            GL.EnableVertexAttribArray(posLoc);
            GL.VertexAttribPointer(posLoc, 3, VertexAttribPointerType.Float, false, stride, 0);

            GL.EnableVertexAttribArray(texLoc);
            GL.VertexAttribPointer(texLoc, 2, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));

            GL.EnableVertexAttribArray(colorLoc);
            GL.VertexAttribPointer(colorLoc, 4, VertexAttribPointerType.Float, false, stride, 5 * sizeof(float));

            GL.BindVertexArray(0);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            _projMatrix = Matrix4.CreateOrthographicOffCenter(0, _glControl.Width, _glControl.Height, 0, -1, 1);
        }

        private void Resize()
        {
            if (!_glControl.Context.IsCurrent)
                _glControl.MakeCurrent();

            int width = Math.Max(1, _glControl.Width);
            int height = Math.Max(1, _glControl.Height);
            _projMatrix = Matrix4.CreateOrthographicOffCenter(0, width, height, 0, -1, 1);
            GL.Viewport(0, 0, width, height);
        }

        public void SetCaretPos(float x, float y)
        {
            _caretPos = new Vector2(x, y);
        }

        public void SetCaretSize(float width, float height)
        {
            _caretSize = new Vector2(width, height);
        }

        public void ShowCaret()
        {
            _caretVisible = true;
            _caretTimer.Start();
        }

        public void HideCaret()
        {
            _caretVisible = false;
            _caretTimer.Stop();
            //_glControl.Invalidate();
        }

        public void ShowSelection()
        {
            _selectionVisible = true;
            //_glControl.Invalidate();
        }

        public void HideSelection()
        {
            _selectionVisible = false;
            //_glControl.Invalidate();
        }

        public void SetSelectionPos(float x, float y)
        {
            _selectionPos = new Vector2(x, y);
        }

        public void SetSelectionSize(float width, float height)
        {
            _selectionSize = new Vector2(width, height);
        }

        public void SetSelectionRect(Rectangle rect)
        {
            _selectionPos = new Vector2(rect.X, rect.Y);
            _selectionSize = new Vector2(rect.Width, rect.Height);
        }

        public void Invalidate()
        {
            _glControl?.Invalidate();
        }

        public void Update()
        {
            _glControl?.Update();
        }

        private bool TryGetFont(Font font, out FontFnt fontFnt, out Texture2D texture)
        {
            texture = null;
            string fontName = GetFontName(font);

            if (!_fontDictionary.TryGetValue(fontName, out fontFnt))
            {
                LoadTTF(font);

                if (!_fontDictionary.TryGetValue(fontName, out fontFnt))
                    return false;
            }

            if (!_textureDictionary.TryGetValue(fontName, out texture))
                return false;

            return true;
        }

        public Size MeasureText(Font font, string text)
        {
            if (!TryGetFont(font, out FontFnt fontFnt, out Texture2D texture))
                return Size.Empty;

            int width = 0;
            int height = fontFnt.FontCommon.LineHeight;

            foreach (char c in text)
            {
                foreach (var charInfo in fontFnt.CharInfo)
                {
                    if (charInfo.Id == c)
                    {
                        width += charInfo.XAdvance;
                        break;
                    }
                }
            }

            return new Size(width, height);
        }

        public IntPtr Handle => _glControl.Handle;

        public void AddQuad(Texture2D texture, float x0, float y0, float z0, float x1, float y1, float z1,
                             float u0, float v0, float u1, float v1, Vector4 color)
        {
            float[] quadVerts =
            {
                x0, y0, z0, u0, v0, color.X, color.Y, color.Z, color.W,
                x1, y0, z0, u1, v0, color.X, color.Y, color.Z, color.W,
                x0, y1, z1, u0, v1, color.X, color.Y, color.Z, color.W,

                x1, y0, z0, u1, v0, color.X, color.Y, color.Z, color.W,
                x1, y1, z1, u1, v1, color.X, color.Y, color.Z, color.W,
                x0, y1, z1, u0, v1, color.X, color.Y, color.Z, color.W
            };

            lock (_sync)
            {
                _quadList.Add(new Quad2(texture, quadVerts));
            }
        }

        public void AddQuads(IEnumerable<float> vertices)
        {
            lock (_sync)
            {
                _vertexBuffer.AddRange(vertices);
            }
        }

        public void DrawBar(string name, float x, float y, float width, float height, float level, System.Drawing.Color color)
        {
            if (!_textureDictionary.TryGetValue(name, out Texture2D texture))
                return;

            // Clamp level to [0.0, 1.0]
            level = Math.Clamp(level, 0.0f, 1.0f);

            // Compute actual height
            float actualHeight = height * level;

            // Bottom-left and top-right coordinates (centered at x)
            float x0 = x - width / 2f;
            float x1 = x + width / 2f;
            float y0 = y - actualHeight;
            float y1 = y;

            // Texture UVs (assuming vertical gradient: v=0 bottom, v=1 top)
            float u0 = 0f;
            float u1 = 1f;
            float v1 = 1f;           // top of texture
            float v0 = 1f - level;   // clip to bottom of texture

            AddQuad(texture, x0, y0, 0, x1, y1, 0, u0, v0, u1, v1, ColorToVec4(color));
        }

        public void DrawText(Font font, float x, float y, string text, System.Drawing.Color textColor, System.Drawing.Color backColor)
        {
            if (!TryGetFont(font, out FontFnt fontFnt, out Texture2D texture))
                return;

            var charInfoList = fontFnt.CharInfo;
            ushort blankCharId = 0xFFFE;
            int texWidth = texture.Width;
            int texHeight = texture.Height;

            ushort lineHeight = fontFnt.FontCommon.LineHeight;
            ushort baseLine = 0; // fontFnt.FontCommon.Base;

            float currentX = x;
            float currentY = y + baseLine;

            Vector4 textTint = ColorToVec4(textColor);
            Vector4 backTint = ColorToVec4(backColor);

            foreach (char c in text)
            {
                uint currentChar = c;
                CharInfo? charInfo = null;
                CharInfo? blankInfo = null;

                for (int j = 0; j < charInfoList.Length; j++)
                {
                    if (charInfoList[j].Id == currentChar)
                        charInfo = charInfoList[j];
                    else if (charInfoList[j].Id == blankCharId)
                        blankInfo = charInfoList[j];
                }

                if (charInfo == null)
                    continue;

                ushort charX = charInfo.X;
                ushort charY = charInfo.Y;
                ushort charWidth = charInfo.Width;
                ushort charHeight = charInfo.Height;
                short charOffsetX = charInfo.XOffset;
                short charOffsetY = charInfo.YOffset;
                short xAdvance = charInfo.XAdvance;

                float u0 = (float)charX / texWidth;
                float v0 = (float)charY / texHeight;
                float u1 = ((float)charX + charWidth) / texWidth;
                float v1 = ((float)charY + charHeight) / texHeight;

                if (backColor.A > 0 && blankInfo != null)
                {
                    var bgChar = blankInfo;
                    float bgU0 = (float)bgChar.X / texWidth;
                    float bgV0 = (float)bgChar.Y / texHeight;
                    float bgU1 = ((float)bgChar.X + bgChar.Width) / texWidth;
                    float bgV1 = ((float)bgChar.Y + bgChar.Height) / texHeight;

                    AddQuad(texture, currentX, currentY - baseLine, 0,
                        currentX + xAdvance, currentY - baseLine + lineHeight, 0,
                        bgU0, bgV0, bgU1, bgV1, backTint);
                }

                AddQuad(texture,
                    currentX + charOffsetX,
                    currentY + charOffsetY,
                    0,
                    currentX + charOffsetX + charWidth,
                    currentY + charOffsetY + charHeight,
                    0,
                    u0, v0, u1, v1,
                    textTint);

                currentX += xAdvance;
            }
        }

        public void FillRect(Font font, Rectangle rect, System.Drawing.Color backColor)
        {
            if (!TryGetFont(font, out FontFnt fontFnt, out Texture2D texture))
                return;

            var charInfoList = fontFnt.CharInfo;
            ushort blankCharId = 0xFFFE;
            int texWidth = texture.Width;
            int texHeight = texture.Height;

            Vector4 backTint = ColorToVec4(backColor);

            CharInfo? blankInfo = null;

            for (int j = 0; j < charInfoList.Length; j++)
            {
                if (charInfoList[j].Id == blankCharId)
                    blankInfo = charInfoList[j];
            }

            if (blankInfo == null)
                return;

            if (backColor.A > 0 && blankInfo != null)
            {
                var bgChar = blankInfo;
                float bgU0 = (float)bgChar.X / texWidth;
                float bgV0 = (float)bgChar.Y / texHeight;
                float bgU1 = ((float)bgChar.X + bgChar.Width) / texWidth;
                float bgV1 = ((float)bgChar.Y + bgChar.Height) / texHeight;

                AddQuad(texture, rect.Left, rect.Top, 0, rect.Right, rect.Bottom, 0, bgU0, bgV0, bgU1, bgV1, backTint);
            }
        }

        public void Render()
        {
            if (!_glControl.Context.IsCurrent)
                _glControl.MakeCurrent();

            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.UseProgram(_shaderProgram);

            GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "uProjMatrix"), false, ref _projMatrix);
            GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "uModelViewMatrix"), false, ref _modelViewMatrix);
            GL.Uniform1(GL.GetUniformLocation(_shaderProgram, "uViewportHeight"), (float)_glControl.Height);
            GL.Uniform1(GL.GetUniformLocation(_shaderProgram, "uTexture0"), 0);

            Vector2 caret = _caretVisible ? _caretPos : new Vector2(-1, -1);
            GL.Uniform2(GL.GetUniformLocation(_shaderProgram, "uCaretPos"), caret);
            GL.Uniform2(GL.GetUniformLocation(_shaderProgram, "uCaretSize"), _caretSize);

            Vector2 selection = _selectionVisible ? _selectionPos : new Vector2(-1, -1);
            GL.Uniform2(GL.GetUniformLocation(_shaderProgram, "uSelectionPos"), selection);
            GL.Uniform2(GL.GetUniformLocation(_shaderProgram, "uSelectionSize"), _selectionSize);

            GL.BindVertexArray(_vao);

            lock (_sync)
            {
                Texture2D currentTexture = null;
                List<float> batched = new();

                foreach (var quad in _quadList)
                {
                    if (currentTexture != quad.Texture)
                    {
                        if (batched.Count > 0 && currentTexture != null)
                        {
                            GL.ActiveTexture(TextureUnit.Texture0);
                            GL.BindTexture(TextureTarget.Texture2D, currentTexture.Handle);

                            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                            GL.BufferData(BufferTarget.ArrayBuffer, batched.Count * sizeof(float), batched.ToArray(), BufferUsageHint.DynamicDraw);
                            GL.DrawArrays(PrimitiveType.Triangles, 0, batched.Count / 9);
                            batched.Clear();
                        }

                        currentTexture = quad.Texture;
                    }

                    batched.AddRange(quad.Vertices);
                }

                if (batched.Count > 0 && currentTexture != null)
                {
                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, currentTexture.Handle);

                    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                    GL.BufferData(BufferTarget.ArrayBuffer, batched.Count * sizeof(float), batched.ToArray(), BufferUsageHint.DynamicDraw);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, batched.Count / 9);
                }

                _quadList.Clear();
            }

            GL.BindVertexArray(0);
            _glControl.SwapBuffers();
        }

        private int CreateShaderProgram()
        {
            string vs = """
            #version 330 core
            in vec3 aPos;
            in vec2 aTexCoord;
            in vec4 aColor;
            out vec2 vTexCoord;
            out vec4 vColor;
            uniform mat4 uProjMatrix;
            uniform mat4 uModelViewMatrix;
            void main() {
                gl_Position = uProjMatrix * uModelViewMatrix * vec4(aPos, 1.0);
                vTexCoord = aTexCoord;
                vColor = aColor;
            }
        """;

            string fs = """
            #version 330 core
            in vec2 vTexCoord;
            in vec4 vColor;
            out vec4 FragColor;
            uniform sampler2D uTexture0;
            uniform vec2 uCaretPos;
            uniform vec2 uCaretSize;
            uniform vec2 uSelectionPos;
            uniform vec2 uSelectionSize;
            uniform float uViewportHeight;
            void main() {
                vec4 texColor = texture(uTexture0, vTexCoord);
                vec2 fragCoord = vec2(gl_FragCoord.x, uViewportHeight - gl_FragCoord.y);
                if (texColor.a == 0.0)
                    discard;

                bool inCaret = fragCoord.x >= uCaretPos.x && fragCoord.x < (uCaretPos.x + uCaretSize.x) &&
                               fragCoord.y >= uCaretPos.y && fragCoord.y < (uCaretPos.y + uCaretSize.y);

                bool inSelection = fragCoord.x >= uSelectionPos.x && fragCoord.x < (uSelectionPos.x + uSelectionSize.x) &&
                                   fragCoord.y >= uSelectionPos.y && fragCoord.y < (uSelectionPos.y + uSelectionSize.y);

                inCaret = inCaret && uCaretPos.x != -1 && uCaretPos.y != -1;
                inSelection = inSelection && uSelectionPos.x != -1 && uSelectionPos.y != -1;

                vec4 outColor = texColor * vColor;
                if (inCaret || inSelection)
                    outColor.rgb = vec3(1.0) - outColor.rgb;
                FragColor = outColor;
            }
        """;

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vs);
            GL.CompileShader(vertexShader);
            CheckShaderCompile(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fs);
            GL.CompileShader(fragmentShader);
            CheckShaderCompile(fragmentShader);

            int program = GL.CreateProgram();
            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);
            GL.LinkProgram(program);
            CheckProgramLink(program);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            return program;
        }

        private Vector4 ColorToVec4(System.Drawing.Color color)
        {
            return new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }

        private void CheckShaderCompile(int shader)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
            if (status == 0)
            {
                string log = GL.GetShaderInfoLog(shader);
                throw new Exception("Shader compile error: " + log);
            }
        }

        private void CheckProgramLink(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int status);
            if (status == 0)
            {
                string log = GL.GetProgramInfoLog(program);
                throw new Exception("Program link error: " + log);
            }
        }
    }
}
