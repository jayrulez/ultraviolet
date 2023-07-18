using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using NUnit.Framework;
using Sedulous.TestFramework;

namespace Sedulous.TestApplication
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
        /// <param name="image">The image being examined.</param>
        internal BitmapResult(Image.Image image)
        {
            this.Image = image;
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
            var machineName = FrameworkTestFramework.GetSanitizedMachineName();
            Directory.CreateDirectory(machineName);

            var fileStream = File.Open(filename, FileMode.Open);

            var expected = Sedulous.Image.Image.FromStream(fileStream, Sedulous.Image.Image.ColorComponents.RedGreenBlueAlpha);

            var filenameNoExtension = Path.GetFileNameWithoutExtension(filename);

            var filenameExpected = Path.ChangeExtension(filenameNoExtension + "_Expected", "png");
            SaveBitmap(expected, Path.Combine(machineName, filenameExpected));

            var filenameActual = Path.ChangeExtension(filenameNoExtension + "_Actual", "png");
            SaveBitmap(Image, Path.Combine(machineName, filenameActual));

            if (expected.Width != Image.Width || expected.Height != Image.Height)
            {
                Assert.Fail("Images do not match due to differing dimensions");
            }

            var mismatchesFound    = 0;
            var mismatchesRequired = (thresholdType == BitmapResultThresholdType.Percentage) ?
                (Int32)((Image.Width * Image.Height) * threshold) : (Int32)threshold;

            var diff = new Image.Image(expected.Width, expected.Height, Sedulous.Image.Image.ColorComponents.RedGreenBlueAlpha);
            {
                // Ignore pixels that are within about 1% of the expected value.
                const Int32 PixelDiffThreshold = 2;

                for (int y = 0; y < expected.Height; y++)
                {
                    for (int x = 0; x < expected.Width; x++)
                    {
                        expected.GetPixel(x, y, out Image.Image.Pixel4 pixelExpected);
                        Image.GetPixel(x, y, out Image.Image.Pixel4 pixelActual);

                        var diffR = Math.Abs(pixelExpected.R + pixelActual.R - 2 * Math.Min(pixelExpected.R, pixelActual.R));
                        var diffG = Math.Abs(pixelExpected.G + pixelActual.G - 2 * Math.Min(pixelExpected.G, pixelActual.G));
                        var diffB = Math.Abs(pixelExpected.B + pixelActual.B - 2 * Math.Min(pixelExpected.B, pixelActual.B));

                        if (diffR > PixelDiffThreshold || diffG > PixelDiffThreshold || diffB > PixelDiffThreshold)
                        {
                            mismatchesFound++;
                        }

                        diff.SetPixel(x, y, (byte)diffR, (byte)diffG, (byte)diffB, 255);
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

        /// <summary>
        /// Gets the underlying value.
        /// </summary>
        public Image.Image Image { get; }

        /// <summary>
        /// Saves a bitmap to thhe specified file.
        /// </summary>
        private void SaveBitmap(Image.Image bitmap, String filename)
        {
            // NOTE: We first open a FileStream because it gives us potentially more
            // useful exception information than "A generic error occurred in GDI+".
            using (var fs = new FileStream(filename, FileMode.Create))
            {
                bitmap.SaveAsPng(fs);
            }
        }

        // Image comparison threshold.
        private BitmapResultThresholdType thresholdType = BitmapResultThresholdType.Percentage;
        private Single threshold = 0.0f;
    }
}
