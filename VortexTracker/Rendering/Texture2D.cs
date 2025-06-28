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
// Version 2.6.1
// (c)2022-2025 Dexus (Volutar), https://github.com/Volutar
// 
// Version 3.0+ (C# port)
// (c)2025 Ben Baker, https://github.com/benbaker76

using System;
using System.Collections.Generic;
using System;
using OpenTK.Graphics.OpenGL4;
using Image = Baker76.Imaging.Image;
using System.IO;

namespace VortexTracker.Rendering
{
    public class Texture2D
    {
        public int Handle { get; private set; }
        public int Width { get; }
        public int Height { get; }

        public Texture2D(string fileName)
            : this(File.OpenRead(fileName))
        {
        }

        public Texture2D(Stream stream)
            : this(Image.Load(stream))
        {
        }

        public Texture2D(Image image)
        {
            // Ensure the image has 32 bits per pixel
            if (image.BitsPerPixel != 32)
                throw new InvalidOperationException("Only 32-bit RGBA images are supported.");

            Width = image.Width;
            Height = image.Height;

            // Generate and bind the texture
            Handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, Handle);

            // Upload the pixel data to the GPU
            GL.TexImage2D(
                TextureTarget.Texture2D,
                level: 0,
                internalformat: PixelInternalFormat.Rgba,
                width: Width,
                height: Height,
                border: 0,
                format: PixelFormat.Rgba,
                type: PixelType.UnsignedByte,
                pixels: image.PixelData
            );

            // Set texture parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            // Generate mipmaps
            //GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        // Bind the texture to a specified texture unit
        public void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
    }
}
