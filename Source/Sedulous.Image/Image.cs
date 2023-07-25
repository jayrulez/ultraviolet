using System;
using System.IO;

namespace Sedulous.Image
{
    public class Image
    {
        public enum ColorComponents
        {
            Default,
            Grey,
            GreyAlpha,
            RedGreenBlue,
            RedGreenBlueAlpha
        }

        /// <summary>
        /// A 3 component pixel.
        /// </summary>
        public struct Pixel3
        {
            /// <summary>
            /// First component.
            /// </summary>
            public byte R;

            /// <summary>
            /// Second component.
            /// </summary>
            public byte G;

            /// <summary>
            /// Third component.
            /// </summary>
            public byte B;
        }
        /// <summary>
        /// A 4 component pixel.
        /// </summary>
        public struct Pixel4
        {
            /// <summary>
            /// First component.
            /// </summary>
            public byte R;

            /// <summary>
            /// Second component.
            /// </summary>
            public byte G;

            /// <summary>
            /// Third component.
            /// </summary>
            public byte B;

            /// <summary>
            /// Fourth component.
            /// </summary>
            public byte A;

            public static Pixel4 operator &(Pixel4 lhs, Pixel4 rhs)
            {
                Pixel4 result = new Pixel4();

                result.R = (byte)(lhs.R & rhs.R);
                result.G = (byte)(lhs.G & rhs.G);
                result.B = (byte)(lhs.B & rhs.B);
                result.A = (byte)(lhs.A & rhs.A);

                return result;
            }
        }

        private StbImageSharp.ImageResult _image;

        public int Width
        {
            get => _image.Width;
            private set => _image.Width = value;
        }

        public int Height
        {
            get => _image.Height;
            private set => _image.Height = value;
        }

        public ColorComponents SourceComp
        {
            get => (ColorComponents)_image.SourceComp;
            private set => _image.SourceComp = (StbImageSharp.ColorComponents)value;
        }

        public ColorComponents Comp
        {
            get => (ColorComponents)_image.Comp;
            private set => _image.Comp = (StbImageSharp.ColorComponents)value;
        }

        public byte[] Data
        {
            get => _image.Data;
            private set => _image.Data = value;
        }

        public static Image FromStream(Stream stream,
            ColorComponents requiredComponents = ColorComponents.Default)
        {
            var image = StbImageSharp.ImageResult.FromStream(stream, (StbImageSharp.ColorComponents)requiredComponents);

            return new Image { _image = image };
        }

        public static Image FromMemory(byte[] data, ColorComponents requiredComponents = ColorComponents.Default)
        {
            var image = StbImageSharp.ImageResult.FromMemory(data, (StbImageSharp.ColorComponents)requiredComponents);
            return new Image { _image = image };
        }

        private Image() { }

        public Image(int width, int height, ColorComponents components, byte[] data = null)
        {
            _image = new StbImageSharp.ImageResult
            {
                Width = width,
                Height = height,
                Comp = (StbImageSharp.ColorComponents)components
            };
            int componentCount = GetComponentCount(components);
            _image.Data = new byte[width * height * componentCount];
            if (data != null)
            {
                data.CopyTo(_image.Data, 0);
            }
        }

        /// <summary>
        /// Get a pixel at the specified coordinates.
        /// </summary>
        public void GetPixel(int x, int y, out Pixel3 pixel)
        {
            int bpp = 3;

            int pixelOffset = (x + _image.Width * y) * bpp;

            pixel = new Pixel3();
            pixel.R = _image.Data[pixelOffset + 0];
            pixel.G = _image.Data[pixelOffset + 1];
            pixel.B = _image.Data[pixelOffset + 2];
        }

        /// <summary>
        /// Get a pixel at the specified coordinates.
        /// </summary>
        public void GetPixel(int x, int y, out Pixel4 pixel)
        {
            int bpp = 4;

            int pixelOffset = (y * _image.Width + x) * bpp;

            pixel = new Pixel4();
            pixel.R = _image.Data[pixelOffset + 0];
            pixel.G = _image.Data[pixelOffset + 1];
            pixel.B = _image.Data[pixelOffset + 2];
            pixel.A = _image.Data[pixelOffset + 3];
        }

        /// <summary>
        /// Set a pixel at the specified coordinates.
        /// </summary>
        public void SetPixel(int x, int y, Pixel4 pixel)
        {
            int bpp = 4;

            int pixelOffset = (x + _image.Width * y) * bpp;

            if (_image.Comp != StbImageSharp.ColorComponents.RedGreenBlueAlpha)
            {
                throw new Exception("Image comp is not expected.");
            }

            _image.Data[pixelOffset + 0] = pixel.R;
            _image.Data[pixelOffset + 1] = pixel.G;
            _image.Data[pixelOffset + 2] = pixel.B;
            _image.Data[pixelOffset + 3] = pixel.A;
        }

        /// <summary>
        /// Set a pixel at the specified coordinates.
        /// </summary>
        public void SetPixel(int x, int y, byte r, byte g, byte b, byte a)
        {
            int bpp = 4;

            int pixelOffset = (x + _image.Width * y) * bpp;

            if (_image.Comp != StbImageSharp.ColorComponents.RedGreenBlueAlpha)
            {
                throw new Exception("Image comp is not expected.");
            }

            _image.Data[pixelOffset + 0] = r;
            _image.Data[pixelOffset + 1] = g;
            _image.Data[pixelOffset + 2] = b;
            _image.Data[pixelOffset + 3] = a;
        }

        /// <summary>
        /// Set a pixel at the specified coordinates.
        /// </summary>
        public void SetPixel(int x, int y, Pixel3 pixel)
        {
            int bpp = 3;

            int pixelOffset = (x + _image.Width * y) * bpp;

            if (_image.Comp != StbImageSharp.ColorComponents.RedGreenBlue)
            {
                throw new Exception("Image comp is not expected.");
            }

            _image.Data[pixelOffset + 0] = pixel.R;
            _image.Data[pixelOffset + 1] = pixel.G;
            _image.Data[pixelOffset + 2] = pixel.B;
        }

        /// <summary>
        /// Set a pixel at the specified coordinates.
        /// </summary>
        public void SetPixel(int x, int y, byte r, byte g, byte b)
        {
            int bpp = 3;

            int pixelOffset = (x + _image.Width * y) * bpp;

            if (_image.Comp != StbImageSharp.ColorComponents.RedGreenBlue)
            {
                throw new Exception("Image comp is not expected.");
            }

            _image.Data[pixelOffset + 0] = r;
            _image.Data[pixelOffset + 1] = g;
            _image.Data[pixelOffset + 2] = b;
        }
        /// <summary>
        /// Gets the image stride.
        /// </summary>
        public int GetStride()
        {
            return GetStride(Width, Comp);
        }

        private static int GetStride(int width, ColorComponents components)
        {
            switch (components)
            {
                case ColorComponents.Grey:
                    return 1 * width;
                case ColorComponents.GreyAlpha:
                    return 2 * width;
                case ColorComponents.RedGreenBlue:
                    return 3 * width;
                case ColorComponents.RedGreenBlueAlpha:
                    return 4 * width;
            }

            return 0;
        }

        private static int GetComponentCount(ColorComponents components)
        {
            switch (components)
            {
                case ColorComponents.Grey:
                    return 1;
                case ColorComponents.GreyAlpha:
                    return 2;
                case ColorComponents.RedGreenBlue:
                    return 3;
                case ColorComponents.RedGreenBlueAlpha:
                    return 4;
            }

            return 0;
        }

        public void SaveAsJpeg(Stream stream, int quality = 100)
        {
            var imageWriter = new StbImageWriteSharp.ImageWriter();
            imageWriter.WriteJpg(_image.Data, _image.Width, _image.Height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream, quality);
        }

        public void SaveAsPng(Stream stream)
        {
            var imageWriter = new StbImageWriteSharp.ImageWriter();
            imageWriter.WritePng(_image.Data, _image.Width, _image.Height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream);
        }

        public void FlipVertical()
        {
            var flipped = new Byte[_image.Data.Length];
            var bytesPerPixel = GetComponentCount(Comp);
            for (var y = 0; y < Height; y++)
            {
                int rowSize = Width * bytesPerPixel;
                //Span<byte> sourceRow = new Span<byte>(_image.Data, y * Width * bytesPerPixel, Width * bytesPerPixel);
                Array.Copy(_image.Data, y * rowSize, flipped, (Height - 1 - y) * rowSize, rowSize);
                //Span<byte> destRow = new Span<byte>(flipped, (Height - 1 - y) * Width * bytesPerPixel, Width * bytesPerPixel);
                //sourceRow.CopyTo(destRow);
            }

            _image.Data = flipped;
        }
    }
}