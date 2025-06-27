using System.Diagnostics;
using System.Drawing.Interop;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Windows.Forms.VisualStyles;
using Baker76.Imaging;
using Image = Baker76.Imaging.Image;

namespace VortexTracker.Rendering
{
    public enum DataFormat
    {
        Text,
        Xml,
        Binary
    };

    public enum AutoSizeMode
    {
        None,
        Texture,
        Font
    }

    public class FontBMOptions
    {
        public const char BlankChar = (char)0xFFFE;

        public TTFont Font;
        public List<char> Chars;
        public int FontSize = 32;
        public int OriginalFontSize = 32;
        public int Spacing = 1;
        public Size TextureSize = new Size(256, 256);
        public AutoSizeMode AutoSize = AutoSizeMode.Texture;
        public bool NoPacking = false;
        public bool IncludeBlankChar = false;
        public Size GridSize = new Size(9, 10); // Default grid size
        public Baker76.Imaging.Color Color = Baker76.Imaging.Color.White;
        public Baker76.Imaging.Color BackgroundColor = Baker76.Imaging.Color.Transparent;
        public DataFormat DataFormat = DataFormat.Text;
        public FontMetrics FontMetrics;
        public Dictionary<char, Point> GlyphPositions;
        public Dictionary<char, GlyphMetrics> GlyphMetrics;
        public Dictionary<char, GlyphBitmap> GlyphBitmaps;
        public List<int> SortedIndices;
        public float Dpi = 72f;

        public FontBMOptions()
        {
        }

        public void CreateChars()
        {
            // Createchars(97, 126);
            CreateChars(32, 126);
        }

        public void CreateChars(int charStart, int charEnd)
        {
            Chars = new List<char>();
            int charCount = charEnd - charStart + 1;

            for (int i = 0; i < charCount; i++)
            {
                var codePoint = (char)(charStart + i);
                Chars.Add(codePoint);
            }

            if (IncludeBlankChar)
                Chars.Add(BlankChar);
        }

        public void CreateChars(string chars)
        {
            Chars = new List<char>();
            Chars.AddRange(chars);

            if (IncludeBlankChar)
                Chars.Add(BlankChar);
        }

        public void ReadCharsFile(string fileName)
        {
            Chars = new List<char>();
            string text = File.ReadAllText(fileName);
            text = new string(text.Where(c => !char.IsControl(c)).ToArray());

            foreach (char ch in text)
                Chars.Add(ch);

            if (IncludeBlankChar)
                Chars.Add(BlankChar);
        }

        public char GetChar(int index)
        {
            return NoPacking ? Chars[index] : Chars[SortedIndices[index]];
        }

        public GlyphMetrics GetBlankCharGlyphMetrics()
        {
            return new GlyphMetrics()
            {
                Bounds = new RectangleF(0, 0, 8, 8),
                LeftSideBearing = 0,
                TopSideBearing = 0,
                AdvanceWidth = 8,
                AdvanceHeight = 8
            };
        }

        public GlyphBitmap GetBlankCharGlyphBitmap()
        {
            return new GlyphBitmap(8, 8, false, Baker76.Imaging.Color.White);
        }

        public RectangleF GetGlyphRect(char codePoint, float scale = 1f)
        {
            if (!GlyphMetrics.ContainsKey(codePoint))
                return RectangleF.Empty;

            var glyphMetrics = GlyphMetrics[codePoint];
            scale = Font.IsSVG() ? (float)FontSize / Font.UnitsPerEm : scale;

            return RectangleF.FromLTRB(
                      glyphMetrics.Bounds.Left * scale,  // Adjust relative to overall bounds
                      glyphMetrics.Bounds.Top * scale,   // Adjust relative to overall bounds
                      glyphMetrics.Bounds.Right * scale, // Scale width
                      glyphMetrics.Bounds.Bottom * scale // Scale height
                  );
        }

        public int GetGlyphXAdvance(char codePoint)
        {
            if (!GlyphMetrics.ContainsKey(codePoint))
                return 0;

            var glyphMetrics = GlyphMetrics[codePoint];

            if (Font.IsSVG())
            {
                float svgScale = FontSize / glyphMetrics.Bounds.Height;

                return (int)(glyphMetrics.Bounds.Width * svgScale);
            }

            return GlyphMetrics[codePoint].AdvanceWidth;
        }

        public List<KernPair> GetKernPairs()
        {
            float scale = (float)FontSize / Font.UnitsPerEm;
            List<KernPair> kernPairs = new List<KernPair>();

            if (Font.IsSVG())
                return kernPairs;

            foreach (char ch1 in Chars)
            {
                foreach (char ch2 in Chars)
                {
                    var kerning = Font.GetKerning(ch1, ch2, scale);

                    if (kerning == 0)
                        continue;

                    KernPair kernPair = new KernPair
                    {
                        First = ch1,
                        Second = ch2,
                        Amount = (short)kerning
                    };

                    kernPairs.Add(kernPair);
                }
            }

            return kernPairs;
        }
    }

    public class FontInfo
    {
        public short FontSize;
        public byte BitField;
        public byte CharSet;
        public ushort StretchH;
        public byte Aa;
        public byte PaddingUp;
        public byte PaddingRight;
        public byte PaddingBottom;
        public byte PaddingLeft;
        public byte SpacingHoriz;
        public byte SpacingVert;
        public byte Outline;
    }

    public class FontCommon
    {
        public ushort LineHeight;
        public ushort Base;
        public ushort ScaleW;
        public ushort ScaleH;
        public ushort Pages;
        public byte BitField;
        public byte AlphaChnl;
        public byte RedChnl;
        public byte GreenChnl;
        public byte BlueChnl;
    }

    public class FontPages
    {
        public List<string> PageNames = new List<string>();
    }

    public class CharInfo
    {
        public uint Id;
        public ushort X;
        public ushort Y;
        public ushort Width;
        public ushort Height;
        public short XOffset;
        public short YOffset;
        public short XAdvance;
        public byte Page;
        public byte Chnl;
    }

    public class KernPair
    {
        public uint First;
        public uint Second;
        public short Amount;
    }

    public class FontFnt
    {
        public string FileName;
        public List<string> PageNames;
        public FontInfo FontInfo;
        public string FontName;
        public FontCommon FontCommon;
        public CharInfo[] CharInfo;
        public KernPair[] KernPairs;

        public FontFnt(string fontName) :
            base()
        {
            FontName = fontName;
        }

        public static async Task<FontFnt> Load(HttpClient httpClient, string fileName)
        {
            using (Stream stream = await httpClient.GetStreamAsync(fileName))
                return await Load(stream, fileName);
        }

        public static async Task<FontFnt> Load(Stream stream, string fileName)
        {
            byte[] header = new byte[4];
            await stream.ReadAsync(header, 0, 4);

            if (header[0] != 'B' || header[1] != 'M' || header[2] != 'F' || header[3] != 3)
            {
                Console.WriteLine("Invalid Font Header!");
                return null;
            }

            string name = Path.GetFileNameWithoutExtension(fileName);
            FontFnt font = new FontFnt(name);

            while (true)
            {
                int blockType = stream.ReadByte();

                if (blockType == -1)
                    break;

                byte[] sizeBuffer = new byte[4];

                if (await stream.ReadAsync(sizeBuffer, 0, 4) != 4)
                    break;

                uint blockSize = BitConverter.ToUInt32(sizeBuffer, 0);

                switch (blockType)
                {
                    case 1: // info
                        {
                            byte[] infoBuffer = new byte[blockSize];

                            if (await stream.ReadAsync(infoBuffer, 0, (int)blockSize) != blockSize)
                                break;

                            FontInfo fontInfo = new FontInfo
                            {
                                FontSize = BitConverter.ToInt16(infoBuffer, 0),
                                BitField = infoBuffer[2],
                                CharSet = infoBuffer[3],
                                StretchH = BitConverter.ToUInt16(infoBuffer, 4),
                                Aa = infoBuffer[6],
                                PaddingUp = infoBuffer[7],
                                PaddingRight = infoBuffer[8],
                                PaddingBottom = infoBuffer[9],
                                PaddingLeft = infoBuffer[10],
                                SpacingHoriz = infoBuffer[11],
                                SpacingVert = infoBuffer[12],
                                Outline = infoBuffer[13]
                            };

                            int nameLength = 0;
                            while (infoBuffer[14 + nameLength] != 0)
                                nameLength++;

                            font.FontName = Encoding.Default.GetString(infoBuffer, 14, nameLength);
                            font.FontInfo = fontInfo;
                            break;
                        }
                    case 2: // common
                        {
                            byte[] commonBuffer = new byte[blockSize];

                            if (await stream.ReadAsync(commonBuffer, 0, (int)blockSize) != blockSize)
                                break;

                            FontCommon fontCommon = new FontCommon
                            {
                                LineHeight = BitConverter.ToUInt16(commonBuffer, 0),
                                Base = BitConverter.ToUInt16(commonBuffer, 2),
                                ScaleW = BitConverter.ToUInt16(commonBuffer, 4),
                                ScaleH = BitConverter.ToUInt16(commonBuffer, 6),
                                Pages = BitConverter.ToUInt16(commonBuffer, 8),
                                BitField = commonBuffer[10],
                                AlphaChnl = commonBuffer[11],
                                RedChnl = commonBuffer[12],
                                GreenChnl = commonBuffer[13],
                                BlueChnl = commonBuffer[14]
                            };

                            font.FontCommon = fontCommon;
                            break;
                        }
                    case 3: // pages
                        {
                            FontPages fontPages = new FontPages();
                            long bytesRemaining = blockSize;

                            while (bytesRemaining > 0)
                            {
                                List<byte> nameBytes = new List<byte>();
                                int b;
                                while ((b = stream.ReadByte()) != 0 && b != -1)
                                {
                                    nameBytes.Add((byte)b);
                                    bytesRemaining--;
                                }
                                if (b == -1)
                                    break;

                                string pageName = Encoding.Default.GetString(nameBytes.ToArray());
                                fontPages.PageNames.Add(pageName);
                                bytesRemaining--;
                            }

                            font.PageNames = fontPages.PageNames;
                            font.FileName = Path.GetFileNameWithoutExtension(fontPages.PageNames[0]);
                            break;
                        }
                    case 4: // chars
                        {
                            int numChars = (int)(blockSize / 20);
                            font.CharInfo = new CharInfo[numChars];

                            byte[] charsBuffer = new byte[blockSize];

                            if (await stream.ReadAsync(charsBuffer, 0, (int)blockSize) != blockSize)
                                break;

                            for (int i = 0; i < numChars; i++)
                            {
                                int offset = i * 20;
                                font.CharInfo[i] = new CharInfo
                                {
                                    Id = BitConverter.ToUInt32(charsBuffer, offset),
                                    X = BitConverter.ToUInt16(charsBuffer, offset + 4),
                                    Y = BitConverter.ToUInt16(charsBuffer, offset + 6),
                                    Width = BitConverter.ToUInt16(charsBuffer, offset + 8),
                                    Height = BitConverter.ToUInt16(charsBuffer, offset + 10),
                                    XOffset = BitConverter.ToInt16(charsBuffer, offset + 12),
                                    YOffset = BitConverter.ToInt16(charsBuffer, offset + 14),
                                    XAdvance = BitConverter.ToInt16(charsBuffer, offset + 16),
                                    Page = charsBuffer[offset + 18],
                                    Chnl = charsBuffer[offset + 19]
                                };
                            }
                            break;
                        }
                    case 5: // kerning pairs
                        {
                            int numKernPairs = (int)(blockSize / 10);
                            font.KernPairs = new KernPair[numKernPairs];

                            byte[] kernBuffer = new byte[blockSize];

                            if (await stream.ReadAsync(kernBuffer, 0, (int)blockSize) != blockSize)
                                break;

                            for (int i = 0; i < numKernPairs; i++)
                            {
                                int offset = i * 10;
                                font.KernPairs[i] = new KernPair
                                {
                                    First = BitConverter.ToUInt32(kernBuffer, offset),
                                    Second = BitConverter.ToUInt32(kernBuffer, offset + 4),
                                    Amount = BitConverter.ToInt16(kernBuffer, offset + 8)
                                };
                            }
                            break;
                        }
                    default:
                        Console.WriteLine("Invalid Font Block!");
                        return null;
                }
            }

            return font;
        }

        public async Task Save(Stream stream, string fileName)
        {
            using (var writer = new BinaryWriter(stream))
            {
                // Write the BMF header
                writer.Write(new byte[] { (byte)'B', (byte)'M', (byte)'F', 3 });

                // Block type 1: info
                {
                    writer.Write((byte)1);

                    List<byte> infoBlock = new List<byte>();
                    infoBlock.AddRange(BitConverter.GetBytes(FontInfo.FontSize));
                    infoBlock.Add(FontInfo.BitField);
                    infoBlock.Add(FontInfo.CharSet);
                    infoBlock.AddRange(BitConverter.GetBytes(FontInfo.StretchH));
                    infoBlock.Add(FontInfo.Aa);
                    infoBlock.Add(FontInfo.PaddingUp);
                    infoBlock.Add(FontInfo.PaddingRight);
                    infoBlock.Add(FontInfo.PaddingBottom);
                    infoBlock.Add(FontInfo.PaddingLeft);
                    infoBlock.Add(FontInfo.SpacingHoriz);
                    infoBlock.Add(FontInfo.SpacingVert);
                    infoBlock.Add(FontInfo.Outline);

                    byte[] fontNameBytes = Encoding.Default.GetBytes(FontName);
                    infoBlock.AddRange(fontNameBytes);
                    infoBlock.Add(0); // Null terminator for the string

                    writer.Write(infoBlock.Count);
                    writer.Write(infoBlock.ToArray());
                }

                // Block type 2: common
                {
                    writer.Write((byte)2);

                    List<byte> commonBlock = new List<byte>();
                    commonBlock.AddRange(BitConverter.GetBytes(FontCommon.LineHeight));
                    commonBlock.AddRange(BitConverter.GetBytes(FontCommon.Base));
                    commonBlock.AddRange(BitConverter.GetBytes(FontCommon.ScaleW));
                    commonBlock.AddRange(BitConverter.GetBytes(FontCommon.ScaleH));
                    commonBlock.AddRange(BitConverter.GetBytes(FontCommon.Pages));
                    commonBlock.Add(FontCommon.BitField);
                    commonBlock.Add(FontCommon.AlphaChnl);
                    commonBlock.Add(FontCommon.RedChnl);
                    commonBlock.Add(FontCommon.GreenChnl);
                    commonBlock.Add(FontCommon.BlueChnl);

                    writer.Write(commonBlock.Count);
                    writer.Write(commonBlock.ToArray());
                }

                // Block type 3: pages
                {
                    writer.Write((byte)3);

                    List<byte> pagesBlock = new List<byte>();
                    foreach (var pageName in PageNames)
                    {
                        byte[] pageNameBytes = Encoding.Default.GetBytes(pageName);
                        pagesBlock.AddRange(pageNameBytes);
                        pagesBlock.Add(0); // Null terminator for each page name
                    }

                    writer.Write(pagesBlock.Count);
                    writer.Write(pagesBlock.ToArray());
                }

                // Block type 4: chars
                {
                    writer.Write((byte)4);

                    List<byte> charsBlock = new List<byte>();
                    foreach (var charInfo in CharInfo)
                    {
                        charsBlock.AddRange(BitConverter.GetBytes(charInfo.Id));
                        charsBlock.AddRange(BitConverter.GetBytes(charInfo.X));
                        charsBlock.AddRange(BitConverter.GetBytes(charInfo.Y));
                        charsBlock.AddRange(BitConverter.GetBytes(charInfo.Width));
                        charsBlock.AddRange(BitConverter.GetBytes(charInfo.Height));
                        charsBlock.AddRange(BitConverter.GetBytes(charInfo.XOffset));
                        charsBlock.AddRange(BitConverter.GetBytes(charInfo.YOffset));
                        charsBlock.AddRange(BitConverter.GetBytes(charInfo.XAdvance));
                        charsBlock.Add(charInfo.Page);
                        charsBlock.Add(charInfo.Chnl);
                    }

                    writer.Write(charsBlock.Count);
                    writer.Write(charsBlock.ToArray());
                }

                // Block type 5: kerning pairs
                {
                    if (KernPairs != null)
                    {
                        writer.Write((byte)5);

                        List<byte> kernPairsBlock = new List<byte>();
                        foreach (var kernPair in KernPairs)
                        {
                            kernPairsBlock.AddRange(BitConverter.GetBytes(kernPair.First));
                            kernPairsBlock.AddRange(BitConverter.GetBytes(kernPair.Second));
                            kernPairsBlock.AddRange(BitConverter.GetBytes(kernPair.Amount));
                        }

                        writer.Write(kernPairsBlock.Count);
                        writer.Write(kernPairsBlock.ToArray());
                    }
                }
            }
        }

        public static async Task<(FontFnt, Image)> LoadTTF(Font font, FontBMOptions options)
        {
            if (!MainForm.TryGetInternalFont(font, out InternalFont internalFont))
                return (null, null);

            string fontsDir = Path.Combine(MainForm.VortexDocumentsDir, MainForm.FontsDefaultDir);
            string filePath = Path.Combine(fontsDir, internalFont.FileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Font file not found: {filePath}");

            byte[] data = await File.ReadAllBytesAsync(filePath);

            return await LoadTTF(data, font.Name, options);
        }

        public static async Task<(FontFnt, Image)> LoadTTF(string fileName, string fontName, FontBMOptions options)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                return await LoadTTF(stream, Path.GetFileNameWithoutExtension(fontName), options);
            }
        }

        public static async Task<(FontFnt, Image)> LoadTTF(Stream stream, string fontName, FontBMOptions options)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return await LoadTTF(memoryStream.ToArray(), fontName, options);
            }
        }

        public static async Task<(FontFnt, Image)> LoadTTF(byte[] buffer, string fontName, FontBMOptions options)
        {
            options.FontSize = options.OriginalFontSize;

            var font = new TTFont(buffer, options);

            options.Font = font;
            options.GlyphMetrics = new Dictionary<char, GlyphMetrics>();
            options.SortedIndices = new List<int>(options.Chars.Count);
            options.FontSize = options.AutoSize == AutoSizeMode.Font ? 1000 : options.FontSize;
            options.FontMetrics = new FontMetrics(font);

            await GetGlyphBounds(font, options);

            return options.NoPacking ? await GenerateFontWithGrid(fontName, font, options) : await GenerateFontWithPacking(fontName, font, options);
        }

        private static async Task GetGlyphBounds(TTFont font, FontBMOptions options)
        {
            float scale = (float)options.FontSize / font.UnitsPerEm;
            options.SortedIndices.Clear();

            for (int i = 0; i < options.Chars.Count; i++)
            {
                var codePoint = options.Chars[i];

                if (codePoint == FontBMOptions.BlankChar)
                {
                    options.GlyphMetrics[codePoint] = options.GetBlankCharGlyphMetrics();
                    options.SortedIndices.Add(i);
                    continue;
                }

                options.GlyphMetrics[codePoint] = await font.GetGlyphMetrics(codePoint, scale, scale, 0, 0);
                options.SortedIndices.Add(i);
            }

            options.SortedIndices = options.SortedIndices
            .OrderByDescending(a =>
            {
                var glyphMetrics = options.GlyphMetrics[options.Chars[a]];
                return glyphMetrics.Bounds.Width * glyphMetrics.Bounds.Height;
            })
            .ToList();
        }

        private static SizeF GetGlyphMaxSize(TTFont font, FontBMOptions options)
        {
            float scale = (float)options.FontSize / font.UnitsPerEm;
            float width = options.FontMetrics.Height * scale;
            float height = width;

            return new SizeF(width, height);
        }

        private static async Task<(FontFnt, Image)> GenerateFontWithGrid(string fontName, TTFont font, FontBMOptions options)
        {
            var charCount = Math.Min(options.GridSize.Width * options.GridSize.Height, options.Chars.Count);
            var fontSize = options.FontSize;
            SizeF maxSize = GetGlyphMaxSize(font, options);
            SizeF cellSize = new SizeF((float)options.TextureSize.Width / options.GridSize.Width, (float)options.TextureSize.Height / options.GridSize.Height);

            options.GlyphPositions = new Dictionary<char, Point>();

            if (options.AutoSize == AutoSizeMode.Texture)
            {
                while (maxSize.Width > cellSize.Width || maxSize.Height > cellSize.Height)
                {
                    IncreaseTextureSize(ref options.TextureSize);
                    cellSize = new SizeF((float)options.TextureSize.Width / options.GridSize.Width, (float)options.TextureSize.Height / options.GridSize.Height);
                }
            }
            else if (options.AutoSize == AutoSizeMode.Font)
            {
                while (maxSize.Width > cellSize.Width || maxSize.Height > cellSize.Height)
                {
                    options.FontSize--;
                    maxSize = GetGlyphMaxSize(font, options);
                }
            }

            await GetGlyphBounds(font, options);

            float scale = (float)options.FontSize / font.UnitsPerEm;
            float ascent = options.FontMetrics.Ascent * scale;
            float baseLine = options.FontMetrics.BaseLine * scale;
            float lineGap = options.FontMetrics.LineGap * scale;
            float height = options.FontMetrics.Height * scale;

            for (int i = 0; i < options.Chars.Count; i++)
            {
                if (i >= charCount)
                    break;

                var codePoint = options.GetChar(i);
                var glyphMetrics = options.GlyphMetrics[codePoint];
                var glyphRect = options.GetGlyphRect(codePoint);

                int col = i % options.GridSize.Width;
                int row = i / options.GridSize.Width;
                float topSideBearing = glyphMetrics.TopSideBearing * scale;
                float cellX = col * cellSize.Width;
                float cellY = row * cellSize.Height;
                float positionX = MathF.Max(cellX + (cellSize.Width / 2.0f - glyphRect.Width / 2.0f), 0);
                float positionY = font.IsSVG() ? cellY : cellY + baseLine + glyphRect.Y;

                options.GlyphPositions[codePoint] = new Point((int)positionX, (int)positionY);
            }

            await RenderGlyphs(font, options);

            var glyphBitmap = new GlyphBitmap(options.TextureSize.Width, options.TextureSize.Height, false, options.BackgroundColor);

            DrawGlyphs(glyphBitmap, options);

            var fontFnt = CreateFontFnt(fontName, options);
            var image = new Image(glyphBitmap.Width, glyphBitmap.Height, 32, glyphBitmap.Pixels);

            return (fontFnt, image);
        }

        // Generates font with RectanglePacker (default packing mode)
        private static async Task<(FontFnt, Image)> GenerateFontWithPacking(string fontName, TTFont font, FontBMOptions options)
        {
            var fontSize = options.FontSize;
            var scale = 1.0f;
            options.TextureSize = options.AutoSize == AutoSizeMode.Texture ? new Size(64, 64) : options.TextureSize;
            bool allGlyphsPacked = false;

            options.GlyphPositions = new Dictionary<char, Point>();

            while (!allGlyphsPacked)
            {
                RectanglePacker packer = new RectanglePacker(options.TextureSize.Width, options.TextureSize.Height);
                allGlyphsPacked = true;

                for (int i = 0; i < options.Chars.Count; i++)
                {
                    var codePoint = options.GetChar(i);
                    var glyphRect = options.GetGlyphRect(codePoint, scale);
                    Size canvasSize = new Size((int)(glyphRect.Width + options.Spacing), (int)(glyphRect.Height + options.Spacing));
                    Point position = Point.Empty;

                    if (!packer.FindPoint(canvasSize, ref position))
                    {
                        if (options.AutoSize == AutoSizeMode.Texture)
                        {
                            IncreaseTextureSize(ref options.TextureSize);
                            allGlyphsPacked = false;
                        }
                        else if (options.AutoSize == AutoSizeMode.Font)
                        {
                            options.FontSize--;
                            scale = (float)options.FontSize / fontSize;
                            allGlyphsPacked = false;
                        }
                        break;
                    }
                    else
                    {
                        options.GlyphPositions[codePoint] = position;
                    }
                }
            }

            await GetGlyphBounds(font, options);
            await RenderGlyphs(font, options);

            var glyphBitmap = new GlyphBitmap(options.TextureSize.Width, options.TextureSize.Height, false, options.BackgroundColor);
            DrawGlyphs(glyphBitmap, options);

            var fontFnt = CreateFontFnt(fontName, options);
            var image = new Image(glyphBitmap.Width, glyphBitmap.Height, 32, glyphBitmap.Pixels);

            return (fontFnt, image);
        }

        // Helper method to increase the texture size
        private static void IncreaseTextureSize(ref Size textureSize)
        {
            if (textureSize.Width == textureSize.Height)
                textureSize.Height <<= 1;
            else
                textureSize.Width <<= 1;
        }


        // Helper method to create a FontFnt object
        private static FontFnt CreateFontFnt(string fontName, FontBMOptions options)
        {
            var scale = (float)options.FontSize / options.Font.UnitsPerEm;
            var lineHeight = options.FontMetrics.LineHeight * scale;
            var height = options.FontMetrics.Height * scale;
            var baseLine = options.FontMetrics.BaseLine * scale;
            var lineGap = options.FontMetrics.LineGap * scale;

            var fontFnt = new FontFnt(fontName);

            fontFnt.PageNames = new List<string>();
            fontFnt.FontInfo = new FontInfo();
            fontFnt.FontCommon = new FontCommon();
            fontFnt.KernPairs = options.GetKernPairs().ToArray();

            fontFnt.PageNames.Add($"{fontName}.png");

            fontFnt.FontInfo.FontSize = (short)options.FontSize;
            fontFnt.FontCommon.LineHeight = (ushort)height;
            fontFnt.FontCommon.Base = (ushort)baseLine;
            fontFnt.FontCommon.ScaleW = (ushort)options.TextureSize.Width;
            fontFnt.FontCommon.ScaleH = (ushort)options.TextureSize.Height;
            fontFnt.FontCommon.Pages = 1;

            List<CharInfo> charInfoList = new List<CharInfo>();

            // Populate CharInfo
            for (int i = 0; i < options.Chars.Count; i++)
            {
                var codePoint = options.GetChar(i);
                var glyphMetrics = options.GlyphMetrics[codePoint];

                if (!options.GlyphPositions.ContainsKey(codePoint))
                    continue;

                var glyphPosition = options.GlyphPositions[codePoint];

                if (!options.GlyphBitmaps.ContainsKey(codePoint))
                    continue;

                var glyphBitmap = options.GlyphBitmaps[codePoint];
                var glyphRect = options.GetGlyphRect(codePoint);
                var xAdvance = options.GetGlyphXAdvance(codePoint);

                CharInfo charInfo = new CharInfo();
                charInfo.Id = codePoint;
                charInfo.X = (ushort)glyphPosition.X;
                charInfo.Y = (ushort)glyphPosition.Y;
                charInfo.Width = (ushort)glyphRect.Width;
                charInfo.Height = (ushort)glyphRect.Height;
                charInfo.XOffset = (short)glyphRect.X;
                charInfo.YOffset = (short)(glyphRect.Y + baseLine + lineGap);
                charInfo.XAdvance = (short)xAdvance;
                charInfo.Page = 0;
                charInfo.Chnl = 15;

                charInfoList.Add(charInfo);
            }

            fontFnt.CharInfo = charInfoList.ToArray();

            return fontFnt;
        }

        // Helper method to render glyphs
        private static async Task RenderGlyphs(TTFont font, FontBMOptions options)
        {
            var scale = (float)options.FontSize / options.Font.UnitsPerEm;
            options.GlyphBitmaps = new Dictionary<char, GlyphBitmap>();

            for (int i = 0; i < options.Chars.Count; i++)
            {
                var codePoint = options.Chars[i];

                if (codePoint == FontBMOptions.BlankChar)
                {
                    options.GlyphBitmaps[codePoint] = options.GetBlankCharGlyphBitmap();

                    continue;
                }

                var glyphBitmap = await font.RenderGlyph(codePoint, scale, options.Color, Baker76.Imaging.Color.Empty);

                if (glyphBitmap == null)
                    continue;

                options.GlyphBitmaps[codePoint] = glyphBitmap;
            }
        }

        // Helper method to draw glyphs using RectanglePacker
        private static void DrawGlyphs(GlyphBitmap bitmap, FontBMOptions options)
        {
            float scale = (float)options.FontSize / options.Font.UnitsPerEm;
            var baseLine = options.FontMetrics.BaseLine * scale;

            for (int i = 0; i < options.Chars.Count; i++)
            {
                var codePoint = options.Chars[i];
                var glyphMetrics = options.GlyphMetrics[codePoint];
                var glyphRect = options.GetGlyphRect(codePoint);

                if (!options.GlyphBitmaps.ContainsKey(codePoint))
                    continue;

                var glyphBitmap = options.GlyphBitmaps[codePoint];

                if (!options.GlyphPositions.ContainsKey(codePoint))
                    continue;

                var glyphPosition = options.GlyphPositions[codePoint];

                bitmap.Draw(glyphBitmap, glyphPosition.X, glyphPosition.Y, options.BackgroundColor);
                //bitmap.DrawFontGuides(glyphBitmap, glyphPosition.X, (int)(glyphPosition.Y - glyphRect.Y), options.FontMetrics, scale);
            }
        }

        public string ToString()
        {
            StringBuilder sb = new StringBuilder();

            // Info Section
            sb.AppendLine($"info face=\"{FontName}\" size={FontInfo.FontSize} bold={FontInfo.BitField & 0x01} italic={FontInfo.BitField & 0x02} charset=\"\" unicode=1 stretchH={FontInfo.StretchH} smooth=1 aa={FontInfo.Aa} padding={FontInfo.PaddingUp},{FontInfo.PaddingRight},{FontInfo.PaddingBottom},{FontInfo.PaddingLeft} spacing={FontInfo.SpacingHoriz},{FontInfo.SpacingVert} outline={FontInfo.Outline}");

            // Common Section
            sb.AppendLine($"common lineHeight={FontCommon.LineHeight} base={FontCommon.Base} scaleW={FontCommon.ScaleW} scaleH={FontCommon.ScaleH} pages={FontCommon.Pages} packed={FontCommon.BitField} alphaChnl={FontCommon.AlphaChnl} redChnl={FontCommon.RedChnl} greenChnl={FontCommon.GreenChnl} blueChnl={FontCommon.BlueChnl}");

            // Page Section
            for (int i = 0; i < PageNames.Count; i++)
            {
                sb.AppendLine($"page id={i} file=\"{PageNames[i]}\"");
            }

            // Chars Section
            sb.AppendLine($"chars count={CharInfo.Length}");
            foreach (var ch in CharInfo)
            {
                sb.AppendLine($"char id={ch.Id} x={ch.X} y={ch.Y} width={ch.Width} height={ch.Height} xoffset={ch.XOffset} yoffset={ch.YOffset} xadvance={ch.XAdvance} page={ch.Page} chnl={ch.Chnl}");
            }

            // Kerning Section
            sb.AppendLine($"kernings count={KernPairs.Length}");
            foreach (var kern in KernPairs)
            {
                sb.AppendLine($"kerning first={kern.First} second={kern.Second} amount={kern.Amount}");
            }

            return sb.ToString();
        }
    }
}
