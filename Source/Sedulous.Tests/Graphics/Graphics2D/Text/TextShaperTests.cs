using System;
using NUnit.Framework;
using Sedulous.FreeType2;
using Sedulous.Graphics.Graphics2D.Text;
using Sedulous.TestApplication;

namespace Sedulous.Tests.Graphics.Graphics2D.Text
{
    [TestFixture]
    public class TextShaperTests : FrameworkApplicationTestFramework
    {
        [Test]
        public void TextShaper_AppendsMultipleStrings()
        {
            var textLength = default(Int32);

            GivenAnSedulousApplicationInServiceMode()
                .WithInitialization(uv =>
                {
                    using (var textShaper = new HarfBuzzTextShaper(uv))
                    {
                        textShaper.SetUnicodeProperties(TextDirection.LeftToRight, TextScript.Latin, "en");
                        textShaper.Append("Hello,");
                        textShaper.Append(" ");
                        textShaper.Append("world!");

                        textLength = textShaper.RawLength;
                    }
                })
                .RunForOneFrame();

            TheResultingValue(textLength).ShouldBe(13);
        }
    }
}
