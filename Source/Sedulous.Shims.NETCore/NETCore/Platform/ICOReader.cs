using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using static Sedulous.Shims.NETCore.Platform.NETCoreIconLoader;

namespace Sedulous.Shims.NETCore.NETCore.Platform
{
    struct IconDirectoryEntry
    {
        public Byte Width;
        public Byte Height;
        public Byte ColorCount;
        public Byte Reserved;
        public UInt16 Planes;
        public UInt16 BitCount;
        public UInt32 BytesInRes;
        public UInt32 ImageOffset;

        public static IconDirectoryEntry Read(Stream stream)
        {
            IconDirectoryEntry directory = new IconDirectoryEntry();
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                directory.Width = reader.ReadByte();

                directory.Height = reader.ReadByte();

                directory.ColorCount = reader.ReadByte();
                directory.Reserved = reader.ReadByte();

                directory.Planes = reader.ReadUInt16();
                directory.BitCount = reader.ReadUInt16();

                directory.BytesInRes = reader.ReadUInt32();
                directory.ImageOffset = reader.ReadUInt32();
            }
            return directory;
        }
    }

    struct IconHeader
    {
        public UInt16 Reserved;
        public UInt16 Type;
        public UInt16 Count;

        public static IconHeader Read(Stream stream)
        {
            IconHeader header = new IconHeader();
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                header.Reserved = reader.ReadUInt16();
                header.Type = reader.ReadUInt16();
                header.Count = reader.ReadUInt16();
            }
            return header;
        }
    }

    internal class ICOReader
    {
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
        /// This block of bytes tells the application detailed information 
        /// about the image, which will be used to display the image on 
        /// the screen.
        /// </summary>
        struct BmpInfoHeader
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

            public static BmpInfoHeader Read(Stream stream)
            {
                BmpInfoHeader header = new BmpInfoHeader();
                using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
                {
                    header.HeaderSize = reader.ReadInt32();
                    header.Width = reader.ReadInt32();
                    header.Height = reader.ReadInt32();
                    header.Planes = reader.ReadInt16();
                    header.BitsPerPixel = reader.ReadInt16();
                    header.Compression = (BmpCompression)reader.ReadInt32();
                    header.ImageSize = reader.ReadInt32();
                    header.XPelsPerMeter = reader.ReadInt32();
                    header.YPelsPerMeter = reader.ReadInt32();
                    header.ClrUsed = reader.ReadInt32();
                    header.ClrImportant = reader.ReadInt32();
                }
                return header;
            }
        }

        //private UInt32 _imageOffset;
        private readonly Stream _stream;

        private IconHeader _header;
        private IconDirectoryEntry[] _directories;

        public ICOReader(Stream stream)
        {
            _stream = stream;
            //_imageOffset = 0;
        }

        public void Decode()
        {
            _stream.Seek(0, SeekOrigin.Begin);

            _header = IconHeader.Read(_stream);

            _directories = new IconDirectoryEntry[_header.Count];

            for (int i = 0; i < _header.Count; i++)
            {
                _directories[i] = IconDirectoryEntry.Read(_stream);
            }

            var images = new Image.Image[_header.Count];

            for (int i = 0; i < _header.Count; i++)
            {
                var directory = _directories[i];

                var iconData = new byte[directory.BytesInRes];

                _stream.Seek(directory.BytesInRes, SeekOrigin.Begin);
                _stream.Read(iconData);

                if (IsPng(iconData))
                {
                    images[i] = Image.Image.FromStream(new MemoryStream(iconData));
                }
                else
                {
                    var bmpInfoHeader = BmpInfoHeader.Read(_stream);
                    int bpp = bmpInfoHeader.BitsPerPixel;

                    if (bpp <= 24)
                    {

                    }
                    else
                    {

                    }
                }
            }
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
    }
}
