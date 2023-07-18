using Sedulous.Core;
using Sedulous.Platform;
using System;
using System.Buffers;
using System.IO;

namespace Sedulous.Shims.NETCore.Graphics
{
    /// <summary>
    /// Represents an implementation of the <see cref="SurfaceSource"/> class for the .NET Core 3.0 platform.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class NETCoreSurfaceSource : SurfaceSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NETCoreSurfaceSource"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> that contains the surface data.</param>
        public NETCoreSurfaceSource(Stream stream)
        {
            Contract.Require(stream, nameof(stream));

            var data = new Byte[stream.Length];
            stream.Read(data, 0, data.Length);

            using (var mstream = new MemoryStream(data))
            {
                this.image = Image.Image.FromStream(mstream, Image.Image.ColorComponents.RedGreenBlueAlpha);
                imageMemory = new Memory<byte>(this.image.Data);

                this.imageMemoryHandle = imageMemory.Pin();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NETCoreSurfaceSource"/> class.
        /// </summary>
        /// <param name="image">The bitmap from which to read surface data.</param>
        public NETCoreSurfaceSource(Image.Image image)
        {
            Contract.Require(image, nameof(image));

            this.image = Image.Image.FromMemory(image.Data, Image.Image.ColorComponents.RedGreenBlueAlpha);

            imageMemory = new Memory<byte>(this.image.Data);

            this.imageMemoryHandle = imageMemory.Pin();
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public override Color this[int x, int y]
        {
            get
            {
                Contract.EnsureNotDisposed(this, disposed);

                this.image.GetPixel(x, y, out Image.Image.Pixel4 pixel);
                return new Color(pixel.R, pixel.G, pixel.B, pixel.A);
            }
        }

        /// <inheritdoc/>
        public unsafe override IntPtr Data => (IntPtr)imageMemoryHandle.Pointer;

        /// <inheritdoc/>
        public override Int32 Stride => image.GetStride();

        /// <inheritdoc/>
        public override Int32 Width => image.Width;

        /// <inheritdoc/>
        public override Int32 Height => image.Height;

        /// <inheritdoc/>
        public override SurfaceSourceDataFormat DataFormat => SurfaceSourceDataFormat.RGBA;

        /// <summary>
        /// Releases resources associated with the object.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if the object is being disposed; <see langword="false"/> if the object is being finalized.</param>
        private void Dispose(Boolean disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                imageMemoryHandle.Dispose();
            }

            disposed = true;
        }

        // State values.
        private readonly Image.Image image;
        private readonly MemoryHandle imageMemoryHandle;
        private readonly Memory<byte> imageMemory;
        private Boolean disposed;
    }
}
