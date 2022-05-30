﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using NUnit.Framework;
using Ultraviolet.TestFramework;

namespace Ultraviolet.TestApplication
{
    /// <summary>
    /// Represents a unit test result containing a bitmap image.
    /// </summary>
    public sealed class BitmapResult
    {
        /// <summary>
        /// Represents the type of threshold that is used to compare bitmap results to expected images.
        /// </summary>
        private enum BitmapResultThresholdType { Percentage, Count };

        /// <summary>
        /// Initializes a new instance of the BitmapResult class.
        /// </summary>
        /// <param name="bitmap">The bitmap being examined.</param>
        //internal BitmapResult(Bitmap bitmap)
        internal BitmapResult(SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> bitmap)
        {
            this.Bitmap = bitmap;
        }

        /// <summary>
        /// Specifies that subsequent comparisons should have the specified percentage threshold value.
        /// The threshold value is the percentage of pixels which must differ from the expected image
        /// in order for the images to be considered not a match.
        /// </summary>
        /// <param name="threshold">The threshold value to set.</param>
        /// <returns>The result object.</returns>
        public BitmapResult WithinPercentageThreshold(Single threshold)
        {
            this.thresholdType = BitmapResultThresholdType.Percentage;
            this.threshold = threshold;
            return this;
        }

        /// <summary>
        /// Specifies the subsequent comparisons should have the specified absolute threshold value.
        /// The threshold value is the number of pixels which must differ from the expected image
        /// in order for the images to be considered not a match.
        /// </summary>
        /// <param name="threshold">The threshold value to set.</param>
        /// <returns>The result object.</returns>
        public BitmapResult WithinAbsoluteThreshold(Int32 threshold)
        {
            this.thresholdType = BitmapResultThresholdType.Count;
            this.threshold = threshold;
            return this;
        }

        /// <summary>
        /// Asserts that the bitmap matches the image in the specified file.
        /// </summary>
        /// <param name="filename">The filename of the image to match against the bitmap.</param>
        public void ShouldMatch(String filename)
        {
            var machineName = UltravioletTestFramework.GetSanitizedMachineName();
            Directory.CreateDirectory(machineName);

            var expected = SixLabors.ImageSharp.Image.Load(filename).CloneAs <SixLabors.ImageSharp.PixelFormats.Rgba32> ();

            var filenameNoExtension = Path.GetFileNameWithoutExtension(filename);

            var filenameExpected = Path.ChangeExtension(filenameNoExtension + "_Expected", "png");
            SaveBitmap(expected, Path.Combine(machineName, filenameExpected));

            var filenameActual = Path.ChangeExtension(filenameNoExtension + "_Actual", "png");
            SaveBitmap(Bitmap, Path.Combine(machineName, filenameActual));

            if (expected.Width != Bitmap.Width || expected.Height != Bitmap.Height)
            {
                Assert.Fail("Images do not match due to differing dimensions");
            }

            var mismatchesFound = 0;
            var mismatchesRequired = (thresholdType == BitmapResultThresholdType.Percentage) ?
                (Int32)((Bitmap.Width * Bitmap.Height) * threshold) : (Int32)threshold;

            using (var diff = new SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>(expected.Width, expected.Height))
            {
                // Ignore pixels that are within about 1% of the expected value.
                const Int32 PixelDiffThreshold = 2;

                for (int y = 0; y < expected.Height; y++)
                {
                    for (int x = 0; x < expected.Width; x++)
                    {
                        var pixelExpected = expected[x, y];
                        var pixelActual = Bitmap[x, y];

                        var diffR = Math.Abs(pixelExpected.R + pixelActual.R - 2 * Math.Min(pixelExpected.R, pixelActual.R));
                        var diffG = Math.Abs(pixelExpected.G + pixelActual.G - 2 * Math.Min(pixelExpected.G, pixelActual.G));
                        var diffB = Math.Abs(pixelExpected.B + pixelActual.B - 2 * Math.Min(pixelExpected.B, pixelActual.B));

                        if (diffR > PixelDiffThreshold || diffG > PixelDiffThreshold || diffB > PixelDiffThreshold)
                        {
                            mismatchesFound++;
                        }

                        diff[x, y] = new SixLabors.ImageSharp.PixelFormats.Rgba32(diffR, diffG, diffB, 255);
                    }
                }

                var filenameDiff = Path.ChangeExtension(filenameNoExtension + "_Diff", "png");
                SaveBitmap(diff, Path.Combine(machineName, filenameDiff));

                if (mismatchesFound > mismatchesRequired)
                {
                    Assert.Fail("Images do not match");
                }
            }
        }

        ///// <summary>
        ///// Asserts that the bitmap matches the image in the specified file.
        ///// </summary>
        ///// <param name="filename">The filename of the image to match against the bitmap.</param>
        //public void ShouldMatch(String filename)
        //{
        //    var machineName = UltravioletTestFramework.GetSanitizedMachineName();
        //    Directory.CreateDirectory(machineName);

        //    var expected = (Bitmap)Image.FromFile(filename);

        //    var filenameNoExtension = Path.GetFileNameWithoutExtension(filename);

        //    var filenameExpected = Path.ChangeExtension(filenameNoExtension + "_Expected", "png");
        //    SaveBitmap(expected, Path.Combine(machineName, filenameExpected));

        //    var filenameActual = Path.ChangeExtension(filenameNoExtension + "_Actual", "png");
        //    SaveBitmap(Bitmap, Path.Combine(machineName, filenameActual));

        //    if (expected.Width != Bitmap.Width || expected.Height != Bitmap.Height)
        //    {
        //        Assert.Fail("Images do not match due to differing dimensions");
        //    }

        //    var mismatchesFound    = 0;
        //    var mismatchesRequired = (thresholdType == BitmapResultThresholdType.Percentage) ?
        //        (Int32)((Bitmap.Width * Bitmap.Height) * threshold) : (Int32)threshold;

        //    using (var diff = new Bitmap(expected.Width, expected.Height))
        //    {
        //        // Ignore pixels that are within about 1% of the expected value.
        //        const Int32 PixelDiffThreshold = 2;

        //        for (int y = 0; y < expected.Height; y++)
        //        {
        //            for (int x = 0; x < expected.Width; x++)
        //            {
        //                var pixelExpected = expected.GetPixel(x, y);
        //                var pixelActual   = Bitmap.GetPixel(x, y);

        //                var diffR = Math.Abs(pixelExpected.R + pixelActual.R - 2 * Math.Min(pixelExpected.R, pixelActual.R));
        //                var diffG = Math.Abs(pixelExpected.G + pixelActual.G - 2 * Math.Min(pixelExpected.G, pixelActual.G));
        //                var diffB = Math.Abs(pixelExpected.B + pixelActual.B - 2 * Math.Min(pixelExpected.B, pixelActual.B));

        //                if (diffR > PixelDiffThreshold || diffG > PixelDiffThreshold || diffB > PixelDiffThreshold)
        //                {
        //                    mismatchesFound++;
        //                }

        //                diff.SetPixel(x, y, System.Drawing.Color.FromArgb(255, diffR, diffG, diffB));
        //            }
        //        }

        //        var filenameDiff = Path.ChangeExtension(filenameNoExtension + "_Diff", "png");
        //        SaveBitmap(diff, Path.Combine(machineName, filenameDiff));

        //        if (mismatchesFound > mismatchesRequired)
        //        {
        //            Assert.Fail("Images do not match");
        //        }
        //    }
        //}

        /// <summary>
        /// Gets the underlying value.
        /// </summary>
        //public Bitmap Bitmap { get; }
        public SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> Bitmap { get; }

        /// <summary>
        /// Saves a bitmap to thhe specified file.
        /// </summary>
        //private void SaveBitmap(Bitmap bitmap, String filename)
        private void SaveBitmap(SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> bitmap, String filename)
        {
            // NOTE: We first open a FileStream because it gives us potentially more
            // useful exception information than "A generic error occurred in GDI+".
            using (var fs = new FileStream(filename, FileMode.Create))
            {
                //bitmap.Save(fs, ImageFormat.Png);
                bitmap.Save(fs, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
            }
        }

        // Image comparison threshold.
        private BitmapResultThresholdType thresholdType = BitmapResultThresholdType.Percentage;
        private Single threshold = 0.0f;
    }
}
