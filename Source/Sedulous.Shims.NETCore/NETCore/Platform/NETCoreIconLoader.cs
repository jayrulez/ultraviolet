using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Sedulous.Graphics;
using Sedulous.Platform;

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
            public Int32 BitmapSize;

            [FieldOffset(12)]
            public Int32 BitmapOffset;

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

        /// <inheritdoc/>
        public override Surface2D LoadIcon()
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

                var imageData = new byte[header.ImageCount][];

                for (int i = 0; i < header.ImageCount; i++)
                {
                    var directory = directories[i];

                    imageData[i] = new byte[directory.BitmapSize];

                    iconStream.Seek(directory.BitmapOffset, SeekOrigin.Begin);
                    iconStream.Read(imageData[i]);

                    if (!IsPng(imageData[i]))
                    {
                        imageData[i] = MakeBitmap(imageData[i]);
                    }
                }

                var icoData = imageData[2];

                using (var source = SurfaceSource.Create(new MemoryStream(icoData)))
                {
                    return Surface2D.Create(source, SurfaceOptions.SrgbColor);
                }
            }

            return null;
        }

        private byte[] MakeBitmap(byte[] bytes)
        {
            return bytes;
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
