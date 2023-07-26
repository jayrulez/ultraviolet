using System;
using System.Diagnostics;
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

        public Pixel4[] GetPixelsRGBA()
        {
            Debug.Assert(GetComponentCount(Comp) == 4);
            Pixel4[] pixels = new Pixel4[Width * Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    GetPixel(x, y,  out pixels[x + Width * y]);
                }
            }

            return pixels;
        }

        public Pixel3[] GetPixelsRGB()
        {
            Debug.Assert(GetComponentCount(Comp) == 3);
            Pixel3[] pixels = new Pixel3[Width * Height];

            int bpp = GetComponentCount(Comp);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    GetPixel(x, y, out pixels[x + Width * y]);
                }
            }

            return pixels;
        }

        public void FlipVertical()
        {
            int row, col;
            for (row = 0; row < Height / 2; row++)
            {
                for (col = 0; col < Width; col++)
                {
                    int topIndex = row * Width + col;
                    int bottomIndex = (Height - row - 1) * Width + col;

                    // Swap the pixels
                    byte temp = _image.Data[topIndex];
                    _image.Data[topIndex] = _image.Data[bottomIndex];
                    _image.Data[bottomIndex] = temp;
                }
            }
        }

        public void FlipHorizontal()
        {
            int row, col;
            for (row = 0; row < Height; row++)
            {
                for (col = 0; col < Width / 2; col++)
                {
                    int leftIndex = row * Width + col;
                    int rightIndex = row * Width + (Width - col - 1);

                    // Swap the pixels
                    byte temp = _image.Data[leftIndex];
                    _image.Data[leftIndex] = _image.Data[rightIndex];
                    _image.Data[rightIndex] = temp;
                }
            }
        }

        private void Transpose()
        {
            int row, col;
            for (row = 0; row < Height; row++)
            {
                for (col = row + 1; col < Width; col++)
                {
                    int index1 = row * Width + col;
                    int index2 = col * Width + row;

                    // Swap the pixels
                    byte temp = _image.Data[index1];
                    _image.Data[index1] = _image.Data[index2];
                    _image.Data[index2] = temp;
                }
            }
        }

        public void RotateRight90()
        {
            Transpose();
            FlipHorizontal();
        }

        public void RotateLeft90()
        {
            Transpose();
            FlipVertical();
        }

        public void RotateRight180()
        {
            int row, col;
            for (row = 0; row < Height / 2; row++)
            {
                for (col = 0; col < Width; col++)
                {
                    // Compute indices for the two pixels to swap
                    int topIndex = row * Width + col;
                    int bottomIndex = (Height - row - 1) * Width + (Width - col - 1);

                    // Swap the pixels
                    byte temp = _image.Data[topIndex];
                    _image.Data[topIndex] = _image.Data[bottomIndex];
                    _image.Data[bottomIndex] = temp;
                }
            }
        }

        public void RotateLeft180()
        {
            int row, col;
            for (row = 0; row < Height / 2; row++)
            {
                for (col = 0; col < Width; col++)
                {
                    int index1 = row * Width + col;
                    int index2 = (Height - row - 1) * Width + (Width - col - 1);

                    // Swap the pixels from opposite corners
                    byte temp = _image.Data[index1];
                    _image.Data[index1] = _image.Data[index2];
                    _image.Data[index2] = temp;
                }
            }
        }

        public void RotateRight270()
        {
            int row, col;
            for (row = 0; row < Height / 2; row++)
            {
                for (col = row; col < Width - row - 1; col++)
                {
                    // Compute indices for the four pixels to swap
                    int topLeftIndex = row * Width + col;
                    int topRightIndex = col * Width + (Width - row - 1);
                    int bottomRightIndex = (Width - row - 1) * Width + (Width - col - 1);
                    int bottomLeftIndex = (Width - col - 1) * Width + row;

                    // Swap the pixels clockwise
                    byte temp = _image.Data[topLeftIndex];
                    _image.Data[topLeftIndex] = _image.Data[bottomLeftIndex];
                    _image.Data[bottomLeftIndex] = _image.Data[bottomRightIndex];
                    _image.Data[bottomRightIndex] = _image.Data[topRightIndex];
                    _image.Data[topRightIndex] = temp;
                }
            }
        }

        public void RotateLeft270()
        {
            int row, col;
            for (row = 0; row < Height / 2; row++)
            {
                for (col = 0; col < (Width + 1) / 2; col++)
                {
                    int topLeftIndex = row * Width + col;
                    int topRightIndex = col * Width + (Width - row - 1);
                    int bottomLeftIndex = (Height - row - 1) * Width + col;
                    int bottomRightIndex = (Height - col - 1) * Width + (Width - row - 1);

                    // Swap the four pixels from opposite corners
                    byte temp = _image.Data[topLeftIndex];
                    _image.Data[topLeftIndex] = _image.Data[topRightIndex];
                    _image.Data[topRightIndex] = _image.Data[bottomRightIndex];
                    _image.Data[bottomRightIndex] = _image.Data[bottomLeftIndex];
                    _image.Data[bottomLeftIndex] = temp;
                }
            }
        }

        // Function to perform bilinear interpolation
        private byte bilinearInterpolation(byte[] image, int width, int height, double x, double y)
        {
            int x1 = (int)x;
            int x2 = x1 + 1;
            int y1 = (int)y;
            int y2 = y1 + 1;

            double dx = x - x1;
            double dy = y - y1;

            byte pixel11 = image[y1 * width + x1];
            byte pixel12 = image[y1 * width + x2];
            byte pixel21 = image[y2 * width + x1];
            byte pixel22 = image[y2 * width + x2];

            double result = (1 - dx) * (1 - dy) * pixel11 +
                            dx * (1 - dy) * pixel12 +
                            (1 - dx) * dy * pixel21 +
                            dx * dy * pixel22;

            return (byte)result;
        }

        // Function to rotate the image byte array at an arbitrary angle (bilinear interpolation)
        public void Rotate(double angle)
        {
            double radianAngle = angle * Math.PI / 180.0;
            double cosAngle = Math.Cos(radianAngle);
            double sinAngle = Math.Sin(radianAngle);
            double centerX = Width / 2.0;
            double centerY = Height / 2.0;

            // Create a copy of the original image buffer
            byte[] originalImage = new byte[_image.Data.Length];
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    originalImage[i * Width + j] = _image.Data[i * Width + j];
                }
            }

            int row, col;
            for (row = 0; row < Height; row++)
            {
                for (col = 0; col < Width; col++)
                {
                    double srcX = cosAngle * (col - centerX) + sinAngle * (row - centerY) + centerX;
                    double srcY = -sinAngle * (col - centerX) + cosAngle * (row - centerY) + centerY;

                    // Perform bilinear interpolation to get the new pixel value after rotation
                    _image.Data[row * Width + col] = bilinearInterpolation(originalImage, Width, Height, srcX, srcY);
                }
            }
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
    }
}