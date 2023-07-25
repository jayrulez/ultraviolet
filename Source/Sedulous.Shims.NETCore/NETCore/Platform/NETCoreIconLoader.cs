using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Sedulous.Graphics;
using Sedulous.Platform;
using static Sedulous.Core.CommonBoxedValues;

namespace Sedulous.Shims.NETCore.Platform
{
    /// <summary>
    /// Represents an implementation of the <see cref="IconLoader"/> class for the .NET Core 3.0 platform.
    /// </summary>
    public sealed class NETCoreIconLoader : IconLoader
    {
        [StructLayout(LayoutKind.Explicit)]
        struct ICOHeader
        {
            [FieldOffset(0)]
            // reserved
            public Int16 Reserved;

            [FieldOffset(2)]
            // image type: 1 for icon (.ICO) image, 2 for cursor (.CUR) image. Other values are invalid.
            public Int16 Type;

            [FieldOffset(4)]
            // number of images in the file.
            public Int16 ImageCount;

            public static ICOHeader Read(BinaryReader reader)
            {
                var header = new ICOHeader();
                header.Reserved = reader.ReadInt16();
                header.Type = reader.ReadInt16();
                header.ImageCount = reader.ReadInt16();

                return header;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        struct ICODirectoryEntry
        {
            [FieldOffset(0)]
            public Byte Width;

            [FieldOffset(1)]
            public Byte Height;

            [FieldOffset(2)]
            public Byte ColorCount;

            [FieldOffset(3)]
            public Byte Reserved;

            [FieldOffset(4)]
            public Int16 ColorPlanes;

            [FieldOffset(6)]
            public Int16 BitsPerPixel;

            [FieldOffset(8)]
            public System.Int32 BitmapSize;

            [FieldOffset(12)]
            public System.Int32 BitmapOffset;

            public static ICODirectoryEntry Read(BinaryReader reader)
            {
                var directory = new ICODirectoryEntry();

                directory.Width = reader.ReadByte();
                //if (directory.Width == 0)
                //    directory.Width = 256;

                directory.Height = reader.ReadByte();
                //if (directory.Height == 0)
                //    directory.Height = 256;

                directory.ColorCount = reader.ReadByte();
                directory.Reserved = reader.ReadByte();

                directory.ColorPlanes = reader.ReadInt16();
                directory.BitsPerPixel = reader.ReadInt16();

                directory.BitmapSize = reader.ReadInt32();
                directory.BitmapOffset = reader.ReadInt32();

                return directory;
            }
        }

        /// <summary>
        /// Defines how the compression type of the image data
        /// in the bitmap file.
        /// </summary>
        enum BmpCompression : int
        {
            /// <summary>
            /// Each image row has a multiple of four elements. If the 
            /// row has less elements, zeros will be added at the right side.
            /// The format depends on the number of bits, stored in the info header.
            /// If the number of bits are one, four or eight each pixel data is 
            /// a index to the palette. If the number of bits are sixteen, 
            /// twenty-four or thirtee-two each pixel contains a color.
            /// </summary>
            RGB = 0,
            /// <summary>
            /// Two bytes are one data record. If the first byte is not zero, the 
            /// next two half bytes will be repeated as much as the value of the first byte.
            /// If the first byte is zero, the record has different meanings, depending
            /// on the second byte. If the second byte is zero, it is the end of the row,
            /// if it is one, it is the end of the image.
            /// Not supported at the moment.
            /// </summary>
            RLE8 = 1,
            /// <summary>
            /// Two bytes are one data record. If the first byte is not zero, the 
            /// next byte will be repeated as much as the value of the first byte.
            /// If the first byte is zero, the record has different meanings, depending
            /// on the second byte. If the second byte is zero, it is the end of the row,
            /// if it is one, it is the end of the image.
            /// Not supported at the moment.
            /// </summary>
            RLE4 = 2,
            /// <summary>
            /// Each image row has a multiple of four elements. If the 
            /// row has less elements, zeros will be added at the right side.
            /// Not supported at the moment.
            /// </summary>
            BitFields = 3,
            /// <summary>
            /// The bitmap contains a JPG image. 
            /// Not supported at the moment.
            /// </summary>
            JPEG = 4,
            /// <summary>
            /// The bitmap contains a PNG image. 
            /// Not supported at the moment.
            /// </summary>
            PNG = 5
        }

        /// <summary>
        /// Stores general information about the Bitmap file.
        /// </summary>
        /// <remarks>
        /// The first two bytes of the Bitmap file format
        /// (thus the Bitmap header) are stored in big-endian order.
        /// All of the other integer values are stored in little-endian format
        /// (i.e. least-significant byte first).
        /// </remarks>
        class BmpFileHeader
        {
            /// <summary>
            /// Defines of the data structure in the bitmap file.
            /// </summary>
            public const int Size = 14;

            /// <summary>
            /// The magic number used to identify the bitmap file: 0x42 0x4D 
            /// (Hex code points for B and M)
            /// </summary>
            public short Type;
            /// <summary>
            /// The size of the bitmap file in bytes.
            /// </summary>
            public int FileSize;
            /// <summary>
            /// Reserved; actual value depends on the application 
            /// that creates the image.
            /// </summary>
            public int Reserved;
            /// <summary>
            /// The offset, i.e. starting address, of the byte where 
            /// the bitmap data can be found.
            /// </summary>
            public int Offset;
        }

        /// <summary>
        /// This block of bytes tells the application detailed information 
        /// about the image, which will be used to display the image on 
        /// the screen.
        /// </summary>
        class BmpInfoHeader
        {
            /// <summary>
            /// Defines of the data structure in the bitmap file.
            /// </summary>
            public const int Size = 40;

            /// <summary>
            /// The size of this header (40 bytes)
            /// </summary>
            public int HeaderSize;
            /// <summary>
            /// The bitmap width in pixels (signed integer).
            /// </summary>
            public int Width;
            /// <summary>
            /// The bitmap height in pixels (signed integer).
            /// </summary>
            public int Height;
            /// <summary>
            /// The number of color planes being used. Must be set to 1.
            /// </summary>
            public short Planes;
            /// <summary>
            /// The number of bits per pixel, which is the color depth of the image. 
            /// Typical values are 1, 4, 8, 16, 24 and 32.
            /// </summary>
            public short BitsPerPixel;
            /// <summary>
            /// The compression method being used. 
            /// See the next table for a list of possible values.
            /// </summary>
            public BmpCompression Compression;
            /// <summary>
            /// The image size. This is the size of the raw bitmap data (see below), 
            /// and should not be confused with the file size.
            /// </summary>
            public int ImageSize;
            /// <summary>
            /// The horizontal resolution of the image. 
            /// (pixel per meter, signed integer)
            /// </summary>
            public int XPelsPerMeter;
            /// <summary>
            /// The vertical resolution of the image. 
            /// (pixel per meter, signed integer)
            /// </summary>
            public int YPelsPerMeter;
            /// <summary>
            /// The number of colors in the color palette, 
            /// or 0 to default to 2^n.
            /// </summary>
            public int ClrUsed;
            /// <summary>
            /// The number of important colors used, 
            /// or 0 when every color is important; generally ignored.
            /// </summary>
            public int ClrImportant;
        }

        /// <summary>
        /// Image decoder for generating an image out of an windows 
        /// bitmap _stream.
        /// </summary>
        /// <remarks>
        /// Does not support the following formats at the moment:
        /// <list type="bullet">
        /// 	<item>JPG</item>
        /// 	<item>PNG</item>
        /// 	<item>RLE4</item>
        /// 	<item>RLE8</item>
        /// 	<item>BitFields</item>
        /// </list>
        /// Formats will be supported in a later realease. We advice always 
        /// to use only 24 Bit windows bitmaps.
        /// </remarks>
        public class BmpDecoder
        {
            /// <summary>
            /// 
            /// </summary>
            public int HeaderSize => 2;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="extension"></param>
            /// <returns></returns>
            public bool IsSupportedFileExtension(string extension)
            {
                if (extension.StartsWith(".")) extension = extension.Substring(1);
                return extension.Equals("BMP", StringComparison.OrdinalIgnoreCase) ||
                       extension.Equals("DIP", StringComparison.OrdinalIgnoreCase);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="header"></param>
            /// <returns></returns>
            public bool IsSupportedFileFormat(byte[] header)
            {
                bool isBmp = false;
                if (header.Length >= 2)
                {
                    isBmp =
                        header[0] == 0x42 && // B
                        header[1] == 0x4D;   // M
                }

                return isBmp;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="stream"></param>
            /// <returns></returns>
#pragma warning disable CS3002 // Return type is not CLS-compliant
            public Image.Image Decode(Stream stream) => new BmpDecoderCore().Decode(stream);
#pragma warning restore CS3002 // Return type is not CLS-compliant

            struct BmpDecoderCore
            {
                /// <summary>
                /// The mask for the red part of the color for 16 bit rgb bitmaps.
                /// </summary>
                private const int Rgb16RMask = 0x00007C00;
                /// <summary>
                /// The mask for the green part of the color for 16 bit rgb bitmaps.
                /// </summary>
                private const int Rgb16GMask = 0x000003E0;
                /// <summary>
                /// The mask for the blue part of the color for 16 bit rgb bitmaps.
                /// </summary>
                private const int Rgb16BMask = 0x0000001F;

                private Stream _stream;
                private BmpFileHeader _fileHeader;
                private BmpInfoHeader _infoHeader;

                public Image.Image Decode(Stream stream)
                {
                    _stream = stream;

                    try
                    {
                        //ReadFileHeader(); // ico does not have bitmap header
                        ReadInfoHeader();

                        int colorMapSize = -1;

                        if (_infoHeader.ClrUsed == 0)
                        {
                            if (_infoHeader.BitsPerPixel == 1 ||
                                _infoHeader.BitsPerPixel == 4 ||
                                _infoHeader.BitsPerPixel == 8)
                            {
                                colorMapSize = (int)Math.Pow(2, _infoHeader.BitsPerPixel) * 4;
                            }
                        }
                        else
                        {
                            colorMapSize = _infoHeader.ClrUsed * 4;
                        }

                        byte[] palette = null;

                        if (colorMapSize > 0)
                        {
                            if (colorMapSize > 255 * 4)
                            {
                                throw new Exception($"Invalid bmp colormap size '{colorMapSize}'");
                            }

                            palette = new byte[colorMapSize];

                            _stream.Read(palette, 0, colorMapSize);
                        }

                        //if (_infoHeader.Width > Image.MaxWidth || _infoHeader.Height > Image.MaxHeight)
                        //{
                        //    throw new ArgumentOutOfRangeException(
                        //        $"The input bitmap '{_infoHeader.Width}x{_infoHeader.Height}' is bigger then the max allowed size '{Image.MaxWidth}x{Image.MaxHeight}'");
                        //}

                        _infoHeader.Height = _infoHeader.Height / 2; // hack, clean this up
                        byte[] pixels = new byte[_infoHeader.Width * _infoHeader.Height * 4];
                        byte[] maskPixels = new byte[_infoHeader.Width * _infoHeader.Height * 4];

                        switch (_infoHeader.Compression)
                        {
                            case BmpCompression.RGB:
                                if (_infoHeader.HeaderSize != 40)
                                {
                                    throw new Exception(
                                        $"Header Size value '{_infoHeader.HeaderSize}' is not valid.");
                                }

                                if (_infoHeader.BitsPerPixel == 32)
                                {
                                    ReadRgb32(pixels, _infoHeader.Width, _infoHeader.Height);
                                }
                                else if (_infoHeader.BitsPerPixel == 24)
                                {
                                    ReadRgb24(pixels, _infoHeader.Width, _infoHeader.Height);
                                }
                                else if (_infoHeader.BitsPerPixel == 16)
                                {
                                    ReadRgb16(pixels, _infoHeader.Width, _infoHeader.Height);
                                }
                                else if (_infoHeader.BitsPerPixel <= 8)
                                {
                                    ReadRgbPalette(pixels, palette,
                                        _infoHeader.Width,
                                        _infoHeader.Height,
                                        _infoHeader.BitsPerPixel);
                                }
                                break;
                            default:
                                throw new NotSupportedException("Does not support this kind of bitmap files.");
                        }

                        _stream.Read(maskPixels, 0, maskPixels.Length);

                        var image = new Image.Image(_infoHeader.Width, _infoHeader.Height, Image.Image.ColorComponents.RedGreenBlueAlpha, pixels);
                        var imageMask = new Image.Image(_infoHeader.Width, _infoHeader.Height, Image.Image.ColorComponents.RedGreenBlueAlpha, maskPixels);
                        imageMask.FlipVertical();

                        for (int x = 0; x < image.Width; x++)
                        {
                            for (int y = 0; y < image.Height; y++)
                            {
                                image.GetPixel(x, y, out Image.Image.Pixel4 pixel);
                                imageMask.GetPixel(x, y, out Image.Image.Pixel4 maskPixel);
                                var outputPixel = pixel & maskPixel;
                                //image.SetPixel(x, y,  outputPixel);
                            }
                        }

                        //// apply the AND and XOR masks
                        //int maskwdt = ((_infoHeader.Width + 31) / 32) * 4;   //line width of AND mask (always 1 Bpp)
                        //int masksize = _infoHeader.Height * maskwdt;             //size of mask
                        //byte[] mask = new byte[masksize];

                        //_stream.Read(mask, 0, masksize);

                        //bool bGoodMask = false;
                        //for (int im = 0; im < masksize; im++)
                        //{
                        //    if (mask[im] != 255)
                        //    {
                        //        bGoodMask = true;
                        //        break;
                        //    }
                        //}


                        //if (bGoodMask)
                        //{ 
                            
                        //}
                        //else
                        //{
                        //    for (int w = 0; w < image.Width; w++)
                        //    {
                        //        for (var h = 0; h < image.Height; h++)
                        //        {
                        //            image.GetPixel(w, h, out Image.Image.Pixel4 pixel);
                        //            if (pixel.R == 0 && pixel.G == 0 && pixel.B == 0 && pixel.A == 0)
                        //            {
                        //                image.SetPixel(w, h, 255, 255, 255, 0);
                        //            }
                        //        }
                        //    }
                        //}

                        return image;
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        throw new Exception("Bitmap does not have a valid format.", e);
                    }
                }

                private void ReadRgbPalette(byte[] imageData, byte[] colors, int width, int height, int bits)
                {
                    // Pixels per byte (bits per pixel)
                    int ppb = 8 / bits;

                    int arrayWidth = (width + ppb - 1) / ppb;

                    // Bit mask
                    int mask = (0xFF >> (8 - bits));

                    byte[] data = new byte[(arrayWidth * height)];

                    _stream.Read(data, 0, data.Length);

                    // Rows are aligned on 4 byte boundaries
                    int alignment = arrayWidth % 4;
                    if (alignment != 0)
                    {
                        alignment = 4 - alignment;
                    }

                    int offset, row, rowOffset, colOffset, arrayOffset;

                    for (int y = 0; y < height; y++)
                    {
                        rowOffset = y * (arrayWidth + alignment);

                        for (int x = 0; x < arrayWidth; x++)
                        {
                            offset = rowOffset + x;

                            // Revert the y value, because bitmaps are saved from down to top
                            row = Invert(y, height);

                            colOffset = x * ppb;

                            for (int shift = 0; shift < ppb && (colOffset + shift) < width; shift++)
                            {
                                int colorIndex = ((data[offset]) >> (8 - bits - (shift * bits))) & mask;

                                arrayOffset = (row * width + (colOffset + shift)) * 4;
                                imageData[arrayOffset + 0] = colors[colorIndex * 4 + 0];
                                imageData[arrayOffset + 1] = colors[colorIndex * 4 + 1];
                                imageData[arrayOffset + 2] = colors[colorIndex * 4 + 2];

                                imageData[arrayOffset + 3] = (byte)255;

                            }
                        }
                    }
                }

                private void ReadRgb16(byte[] imageData, int width, int height)
                {
                    byte r, g, b;

                    int scaleR = 256 / 32;
                    int scaleG = 256 / 64;

                    int alignment = 0;
                    byte[] data = GetImageArray(width, height, 2, ref alignment);

                    int offset, row, rowOffset, arrayOffset;

                    for (int y = 0; y < height; y++)
                    {
                        rowOffset = y * (width * 2 + alignment);

                        // Revert the y value, because bitmaps are saved from down to top
                        row = Invert(y, height);

                        for (int x = 0; x < width; x++)
                        {
                            offset = rowOffset + x * 2;

                            short temp = BitConverter.ToInt16(data, offset);

                            r = (byte)(((temp & Rgb16RMask) >> 11) * scaleR);
                            g = (byte)(((temp & Rgb16GMask) >> 5) * scaleG);
                            b = (byte)(((temp & Rgb16BMask)) * scaleR);

                            arrayOffset = (row * width + x) * 4;
                            imageData[arrayOffset + 0] = b;
                            imageData[arrayOffset + 1] = g;
                            imageData[arrayOffset + 2] = r;

                            imageData[arrayOffset + 3] = (byte)255;
                        }
                    }
                }

                private void ReadRgb24(byte[] imageData, int width, int height)
                {
                    int alignment = 0;
                    byte[] data = GetImageArray(width, height, 3, ref alignment);

                    int offset, row, rowOffset, arrayOffset;

                    for (int y = 0; y < height; y++)
                    {
                        rowOffset = y * (width * 3 + alignment);

                        // Revert the y value, because bitmaps are saved from down to top
                        row = Invert(y, height);

                        for (int x = 0; x < width; x++)
                        {
                            offset = rowOffset + x * 3;

                            arrayOffset = (row * width + x) * 4;
                            imageData[arrayOffset + 0] = data[offset + 0];
                            imageData[arrayOffset + 1] = data[offset + 1];
                            imageData[arrayOffset + 2] = data[offset + 2];

                            imageData[arrayOffset + 3] = (byte)255;
                        }
                    }
                }

                private void ReadRgb32(byte[] imageData, int width, int height)
                {
                    int alignment = 0;
                    byte[] data = GetImageArray(width, height, 4, ref alignment);

                    int offset, row, rowOffset, arrayOffset;

                    for (int y = 0; y < height; y++)
                    {
                        rowOffset = y * (width * 4 + alignment);

                        // Revert the y value, because bitmaps are saved from down to top
                        row = Invert(y, height);

                        for (int x = 0; x < width; x++)
                        {
                            offset = rowOffset + x * 4;

                            arrayOffset = (row * width + x) * 4;
                            imageData[arrayOffset + 0] = data[offset + 0];
                            imageData[arrayOffset + 1] = data[offset + 1];
                            imageData[arrayOffset + 2] = data[offset + 2];

                            imageData[arrayOffset + 3] = (byte)255;
                        }
                    }
                }

                private static int Invert(int y, int height)
                {
                    int row = 0;

                    if (height > 0)
                    {
                        row = (height - y - 1);
                    }
                    else
                    {
                        row = y;
                    }

                    return row;
                }

                private byte[] GetImageArray(int width, int height, int bytes, ref int alignment)
                {
                    int dataWidth = width;

                    alignment = (width * bytes) % 4;

                    if (alignment != 0)
                    {
                        alignment = 4 - alignment;
                    }

                    int size = (dataWidth * bytes + alignment) * height;

                    byte[] data = new byte[size];

                    _stream.Read(data, 0, size);

                    return data;
                }

                private void ReadInfoHeader()
                {
                    byte[] data = new byte[BmpInfoHeader.Size];

                    _stream.Read(data, 0, BmpInfoHeader.Size);

                    _infoHeader = new BmpInfoHeader();
                    _infoHeader.HeaderSize = BitConverter.ToInt32(data, 0);
                    _infoHeader.Width = BitConverter.ToInt32(data, 4);
                    _infoHeader.Height = BitConverter.ToInt32(data, 8);
                    _infoHeader.Planes = BitConverter.ToInt16(data, 12);
                    _infoHeader.BitsPerPixel = BitConverter.ToInt16(data, 14);
                    _infoHeader.ImageSize = BitConverter.ToInt32(data, 20);
                    _infoHeader.XPelsPerMeter = BitConverter.ToInt32(data, 24);
                    _infoHeader.YPelsPerMeter = BitConverter.ToInt32(data, 28);
                    _infoHeader.ClrUsed = BitConverter.ToInt32(data, 32);
                    _infoHeader.ClrImportant = BitConverter.ToInt32(data, 36);
                    _infoHeader.Compression = (BmpCompression)BitConverter.ToInt32(data, 16);
                }

                private void ReadFileHeader()
                {
                    byte[] data = new byte[BmpFileHeader.Size];

                    _stream.Read(data, 0, BmpFileHeader.Size);

                    _fileHeader = new BmpFileHeader();
                    _fileHeader.Type = BitConverter.ToInt16(data, 0);
                    _fileHeader.FileSize = BitConverter.ToInt32(data, 2);
                    _fileHeader.Reserved = BitConverter.ToInt32(data, 6);
                    _fileHeader.Offset = BitConverter.ToInt32(data, 10);
                }
            }
        }

        /// <inheritdoc/>
        public unsafe override Surface2D LoadIcon()
        {
            var asmEntry = Assembly.GetEntryAssembly();
            var asmLoader = typeof(NETCoreIconLoader).Assembly;

            var asmResourceNames = asmEntry.GetManifestResourceNames();
            var asmResourcePrefix = GetLongestCommonResourcePrefix(asmResourceNames);
            var asmResourceIcon = String.IsNullOrEmpty(asmResourcePrefix) && asmResourceNames.Length == 1 && asmResourceNames[0].EndsWith(".icon.ico") ?
                asmResourceNames[0] : $"{asmResourcePrefix}.icon.ico";

            var iconStream =
                asmEntry.GetManifestResourceStream(asmResourceIcon) ??
                asmLoader.GetManifestResourceStream($"Sedulous.Shims.NETCore.icon.ico");

            if (iconStream != null)
            {
                var reader = new BinaryReader(iconStream);

                var header = ICOHeader.Read(reader);

                if (header.ImageCount < 1)
                    return null;

                var directories = new ICODirectoryEntry[header.ImageCount];

                for (int i = 0; i < header.ImageCount; i++)
                {
                    directories[i] = ICODirectoryEntry.Read(reader);
                }

                var icons = new Image.Image[header.ImageCount];

                for (int i = 0; i < header.ImageCount; i++)
                {
                    var directory = directories[i];

                    var iconData = new byte[directory.BitmapSize];

                    iconStream.Seek(directory.BitmapOffset, SeekOrigin.Begin);
                    iconStream.Read(iconData);

                    if (IsPng(iconData))
                    {
                        icons[i] = Image.Image.FromStream(new MemoryStream(iconData));
                    }
                    else
                    {
                        var bmpDecoder = new BmpDecoder();
                        icons[i] = bmpDecoder.Decode(new MemoryStream(iconData));
                    }
                }

                var icon = icons[8];

                var memory = new Memory<byte>(icon.Data);
                using (var handle = memory.Pin())
                {
                    var surface = Surface2D.Create(icon.Width, icon.Height);
                    surface.SetRawData(new IntPtr(handle.Pointer), 0, 0, icon.Data.Length);

                    return surface;
                }
            }

            return null;
        }

        private static bool IsPng(byte[] bytes)
        {
            return bytes[0] == 137
                && bytes[1] == 80
                && bytes[2] == 78
                && bytes[3] == 71
                && bytes[4] == 13
                && bytes[5] == 10
                && bytes[6] == 26
                && bytes[7] == 10
                ;
        }

        /// <summary>
        /// Determines the common prefix which is shared by all of the specified manifest resource names.
        /// </summary>
        private static String GetLongestCommonResourcePrefix(String[] resourceNames)
        {
            if (resourceNames == null || resourceNames.Length <= 1)
                return String.Empty;

            var resourceNamesSplit = resourceNames.Select(x => x.Split('.')).ToArray();
            var resourceNamesMinLength = resourceNamesSplit.Min(x => x.Length);

            var commonComponents = 0;

            for (int i = 0; i < resourceNamesMinLength; i++)
            {
                if (resourceNamesSplit.All(x => x[i] == resourceNamesSplit[0][i]))
                    commonComponents++;
            }

            if (commonComponents == 0)
                return String.Empty;

            return String.Join(".", resourceNamesSplit[0].Take(commonComponents));
        }
    }
}
