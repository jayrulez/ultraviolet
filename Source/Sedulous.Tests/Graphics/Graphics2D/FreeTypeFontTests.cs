using NUnit.Framework;
using Sedulous.FreeType2;
using Sedulous.Graphics.Graphics2D.Text;
using Sedulous.TestApplication;

namespace Sedulous.Tests.Graphics.Graphics2D
{
    [TestFixture]
    public class FreeTypeFontTests : FrameworkApplicationTestFramework
    {
        [Test]
        [Category("Content")]
        [Description("Ensures that FreeType2 fonts measure ShapedString instances correctly.")]
        public void FreeTypeFont_MeasuresShapedStringsCorrectly()
        {
            var freetypeFont = default(FreeTypeFont);
            var size = Size2.Zero;

            GivenAnSedulousApplication()
                .WithPlugin(new FreeTypeFontPlugin())
                .WithContent(content =>
                {
                    freetypeFont = content.Load<FreeTypeFont>("Fonts/FiraSans");

                    using (var textShaper = new HarfBuzzTextShaper(content.Sedulous))
                    {
                        textShaper.SetUnicodeProperties(TextDirection.LeftToRight, TextScript.Latin, "en");
                        textShaper.Append("Hello, world!");

                        var str = textShaper.CreateShapedString(freetypeFont.Regular);
                        size = freetypeFont.Regular.MeasureShapedString(str);
                    }
                })
                .RunForOneFrame();

            TheResultingValue(size.Width).ShouldBe(92);
            TheResultingValue(size.Height).ShouldBe(20);
        }

        [Test]
        [Category("Content")]
        [Description("Ensures that FreeType2 fonts measure ShapedStringBuilder instances correctly")]
        public void FreeTypeFont_MeasuresShapedStringBuildersCorrectly()
        {
            var freetypeFont = default(FreeTypeFont);
            var size = Size2.Zero;

            GivenAnSedulousApplication()
                .WithPlugin(new FreeTypeFontPlugin())
                .WithContent(content =>
                {
                    freetypeFont = content.Load<FreeTypeFont>("Fonts/FiraSans");

                    using (var textShaper = new HarfBuzzTextShaper(content.Sedulous))
                    {
                        textShaper.SetUnicodeProperties(TextDirection.LeftToRight, TextScript.Latin, "en");
                        textShaper.Append("Hello, world!");

                        var str = new ShapedStringBuilder();
                        str.Append(textShaper, freetypeFont.Regular);

                        size = freetypeFont.Regular.MeasureShapedString(str);
                    }
                })
                .RunForOneFrame();

            TheResultingValue(size.Width).ShouldBe(92);
            TheResultingValue(size.Height).ShouldBe(20);
        }
    }
}
