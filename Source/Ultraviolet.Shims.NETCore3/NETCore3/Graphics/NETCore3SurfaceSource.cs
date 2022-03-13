using System;
using System.Buffers;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Ultraviolet.Core;
using Ultraviolet.Graphics;

namespace Ultraviolet.Shims.NETCore3.Graphics
{
    /// <summary>
    /// Represents an implementation of the <see cref="SurfaceSource"/> class for the .NET Core 3.0 platform.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class NETCore3SurfaceSource : SurfaceSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NETCore3SurfaceSource"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> that contains the surface data.</param>
        public NETCore3SurfaceSource(Stream stream)
        {
            Contract.Require(stream, nameof(stream));

            var data = new Byte[stream.Length];
            stream.Read(data, 0, data.Length);

            SixLabors.ImageSharp.Configuration customConfig = SixLabors.ImageSharp.Configuration.Default.Clone();
            customConfig.PreferContiguousImageBuffers = true;
            using (var mstream = new MemoryStream(data)) {
                this.bmp = SixLabors.ImageSharp.Image.Load(customConfig, mstream).CloneAs<SixLabors.ImageSharp.PixelFormats.Rgba32> ();
                if (!this.bmp.DangerousTryGetSinglePixelMemory(out imageMemory))
                {
                    throw new Exception(
                        "This can only happen with multi-GB images or when PreferContiguousImageBuffers is not set to true.");
                }

                this.imageMemoryHandle = imageMemory.Pin();
            }

            //using (var mstream = new MemoryStream(data))
            //{
            //    this.bmp = new Bitmap(mstream);
            //    this.bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            //}
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NETCore3SurfaceSource"/> class.
        /// </summary>
        /// <param name="bmp">The bitmap from which to read surface data.</param>
        public NETCore3SurfaceSource(SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> bmp)
        {
            Contract.Require(bmp, nameof(bmp));

            this.bmp = bmp;
            if (!this.bmp.DangerousTryGetSinglePixelMemory(out imageMemory))
            {
                throw new Exception("This can only happen with multi-GB images or when PreferContiguousImageBuffers is not set to true.");
            }

            this.imageMemoryHandle = imageMemory.Pin();
        }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="NETCore3SurfaceSource"/> class.
        ///// </summary>
        ///// <param name="bmp">The bitmap from which to read surface data.</param>
        //public NETCore3SurfaceSource(Bitmap bmp)
        //{
        //    Contract.Require(bmp, nameof(bmp));

        //    this.bmp = bmp;
        //    this.bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        //}

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

                var pixel = this.bmp[x, y];
                return new Color(pixel.R, pixel.G, pixel.B, pixel.A);
                //unsafe
                //{
                //    var pixel = ((byte*)bmpData.Scan0) + (bmpData.Stride * y) + (x * sizeof(UInt32));
                //    var b = *pixel++;
                //    var g = *pixel++;
                //    var r = *pixel++;
                //    var a = *pixel++;
                //    return new Color(r, g, b, a);
                //}
            }
        }

        /// <inheritdoc/>
        //public override IntPtr Data => bmpData.Scan0;
        public unsafe override IntPtr Data => (IntPtr)imageMemoryHandle.Pointer;

        /// <inheritdoc/>
        //public override Int32 Stride => bmpData.Stride;
        public override Int32 Stride => bmp.Width * 4;

        /// <inheritdoc/>
        public override Int32 Width => bmp.Width;

        /// <inheritdoc/>
        public override Int32 Height => bmp.Height;

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
                //bmp.UnlockBits(bmpData);
                imageMemoryHandle.Dispose();
                bmp.Dispose();
            }

            disposed = true;
        }

        // State values.
        //private readonly Bitmap bmp;
        //private readonly BitmapData bmpData;
        private readonly SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> bmp;
        private readonly MemoryHandle imageMemoryHandle;
        private readonly Memory<SixLabors.ImageSharp.PixelFormats.Rgba32> imageMemory;
        private Boolean disposed;
    }
}
