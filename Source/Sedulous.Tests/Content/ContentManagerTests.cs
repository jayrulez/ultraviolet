﻿using NUnit.Framework;
using Sedulous.FreeType2;
using Sedulous.Graphics.Graphics2D;
using Sedulous.TestApplication;

namespace Sedulous.Tests.Content
{
    [TestFixture]
    public class ContentManagerTests : FrameworkApplicationTestFramework
    {
        [Test]
        [Category("Content")]
        public void ContentManager_ProcessesMetadataFiles_WhenAssetIsAStream()
        {
            var contentStream = typeof(ContentManagerTests).Assembly.GetManifestResourceStream("Sedulous.Tests.Resources.Content.Fonts.FiraSansEmbedded.uvmeta");

            GivenAFrameworkApplicationWithNoWindow()
                .WithPlugin(new FreeTypeFontPlugin())
                .WithContent(content =>
                {
                    var font = content.LoadFromStream<FrameworkFont>(contentStream, "uvmeta");

                    TheResultingString(font.Regular.ToString())
                        .ShouldBe("Fira Sans Regular 32pt");
                })
                .RunForOneFrame();
        }
    }
}
