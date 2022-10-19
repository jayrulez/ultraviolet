using NUnit.Framework;
using Sedulous.FreeType2;
using Sedulous.Graphics.Graphics2D;
using Sedulous.TestApplication;

namespace Sedulous.Tests.Content
{
    [TestFixture]
    public class ContentManagerTests : SedulousApplicationTestFramework
    {
        [Test]
        [Category("Content")]
        public void ContentManager_ProcessesMetadataFiles_WhenAssetIsAStream()
        {
            var contentStream = typeof(ContentManagerTests).Assembly.GetManifestResourceStream("Sedulous.Tests.Resources.Content.Fonts.FiraSansEmbedded.uvmeta");

            GivenAnSedulousApplicationWithNoWindow()
                .WithPlugin(new FreeTypeFontPlugin())
                .WithContent(content =>
                {
                    var font = content.LoadFromStream<SedulousFont>(contentStream, "uvmeta");

                    TheResultingString(font.Regular.ToString())
                        .ShouldBe("Fira Sans Regular 32pt");
                })
                .RunForOneFrame();
        }
    }
}
