using System;
using System.Buffers;
//using System.Drawing;
//using System.Drawing.Imaging;
using System.IO;
using Ultraviolet.Core;
using Ultraviolet.Graphics;
using GDIRect = System.Drawing.Rectangle;

namespace Ultraviolet.Shims.NETCore3.Graphics
{
    /// <summary>
    /// Represents an implementation of the <see cref="SurfaceSaver"/> class for the .NET Core 3.0 platform.
    /// </summary>
    public sealed class NETCore3SurfaceSaver : SurfaceSaver
    {
        /// <inheritdoc/>
        public override void SaveAsPng(Surface2D surface, Stream stream)
        {
            Contract.Require(surface, nameof(surface));
            Contract.Require(stream, nameof(stream));

            //Save(surface, stream, ImageFormat.Png);
            Save(surface, stream, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
        }

        /// <inheritdoc/>
        public override void SaveAsJpeg(Surface2D surface, Stream stream)
        {
            Contract.Require(surface, nameof(surface));
            Contract.Require(stream, nameof(stream));

            Save(surface, stream, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder());
        }

        /// <inheritdoc/>
        public override void SaveAsPng(RenderTarget2D renderTarget, Stream stream)
        {
            Contract.Require(renderTarget, nameof(renderTarget));
            Contract.Require(stream, nameof(stream));

            Save(renderTarget, stream, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
        }

        /// <inheritdoc/>
        public override void SaveAsJpeg(RenderTarget2D renderTarget, Stream stream)
        {
            Contract.Require(renderTarget, nameof(renderTarget));
            Contract.Require(stream, nameof(stream));

            Save(renderTarget, stream, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder());
        }

        /// <summary>
        /// Saves the specified surface as an image with the specified format.
        /// </summary>
        /// <param name="surface">The surface to save.</param>
        /// <param name="stream">The stream to which to save the surface data.</param>
        /// <param name="format">The format with which to save the image.</param>
        //private void Save(Surface2D surface, Stream stream, ImageFormat format)
        //private void Save(Surface2D surface, Stream stream, ImageFormat format)
        private void Save(Surface2D surface, Stream stream, SixLabors.ImageSharp.Formats.IImageEncoder format)
        {
            var data = new Color[surface.Width * surface.Height];
            surface.GetData(data);

            Save(data, surface.Width, surface.Height, stream, format);
        }

        /// <summary>
        /// Saves the specified render target as an image with the specified format.
        /// </summary>
        /// <param name="renderTarget">The render target to save.</param>
        /// <param name="stream">The stream to which to save the render target data.</param>
        /// <param name="format">The format with which to save the image.</param>
        //private void Save(RenderTarget2D renderTarget, Stream stream, ImageFormat format)
        //private void Save(RenderTarget2D renderTarget, Stream stream, SixLabors.ImageSharp.Formats.IImageFormat format)
        private void Save(RenderTarget2D renderTarget, Stream stream, SixLabors.ImageSharp.Formats.IImageEncoder format)
        {
            var data = new Color[renderTarget.Width * renderTarget.Height];
            renderTarget.GetData(data);

            Save(data, renderTarget.Width, renderTarget.Height, stream, format);
        }



        /// <summary>
        /// Saves the specified color data as an image with the specified format.
        /// </summary>
        /// <param name="data">An array containing the image's color data.</param>
        /// <param name="width">The width of the image in pixels.</param>
        /// <param name="height">The height of the image in pixels.</param>
        /// <param name="stream">The stream to which to save the image data.</param>
        /// <param name="format">The format with which to save the image.</param>
        private unsafe void Save(Color[] data, Int32 width, Int32 height, Stream stream, SixLabors.ImageSharp.Formats.IImageEncoder format)
        {
            SixLabors.ImageSharp.Configuration customConfig = SixLabors.ImageSharp.Configuration.Default.Clone();
            customConfig.PreferContiguousImageBuffers = true;

            using (var image = new SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>(customConfig, width, height))
            {
                if (!image.DangerousTryGetSinglePixelMemory(out Memory<SixLabors.ImageSharp.PixelFormats.Rgba32> memory))
                {
                    throw new Exception("This can only happen with multi-GB images or when PreferContiguousImageBuffers is not set to true.");
                }

                using (MemoryHandle pinHandle = memory.Pin())
                {
                    IntPtr ptr = (IntPtr)pinHandle.Pointer;

                    fixed (Color* pData = data)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            var pSrc = pData + (y * width);
                            var pDst = (Byte*)ptr + (y * image.Width);

                            for (int x = 0; x < width; x++)
                            {
                                var color = *pSrc++;
                                *pDst++ = color.R;
                                *pDst++ = color.G;
                                *pDst++ = color.B;
                                *pDst++ = color.A;
                            }
                        }
                    }

                    image.Save(stream, format);
                    pinHandle.Dispose();
                }
            }
        }

        ///// <summary>
        ///// Saves the specified color data as an image with the specified format.
        ///// </summary>
        ///// <param name="data">An array containing the image's color data.</param>
        ///// <param name="width">The width of the image in pixels.</param>
        ///// <param name="height">The height of the image in pixels.</param>
        ///// <param name="stream">The stream to which to save the image data.</param>
        ///// <param name="format">The format with which to save the image.</param>
        //private unsafe void Save(Color[] data, Int32 width, Int32 height, Stream stream, ImageFormat format)
        //{
        //    using (var bmp = new Bitmap(width, height))
        //    {
        //        var bmpData = bmp.LockBits(new GDIRect(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

        //        fixed (Color* pData = data)
        //        {
        //            for (int y = 0; y < height; y++)
        //            {
        //                var pSrc = pData + (y * width);
        //                var pDst = (Byte*)bmpData.Scan0 + (y * bmpData.Stride);

        //                for (int x = 0; x < width; x++)
        //                {
        //                    var color = *pSrc++;
        //                    *pDst++ = color.B;
        //                    *pDst++ = color.G;
        //                    *pDst++ = color.R;
        //                    *pDst++ = color.A;
        //                }
        //            }
        //        }

        //        bmp.UnlockBits(bmpData);
        //        bmp.Save(stream, format);
        //    }
        //}
    }
}
