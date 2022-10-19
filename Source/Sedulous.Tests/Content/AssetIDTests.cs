using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;
using Sedulous.Content;
using Sedulous.TestApplication;

namespace Sedulous.Tests.Content
{
    [TestFixture]
    public class AssetIDTests : SedulousApplicationTestFramework
    {
        [Test]
        [Category("Content")]
        public void AssetID_SerializesToJson()
        {
            GivenAnSedulousApplicationWithNoWindow()
                .WithContent(content =>
                {
                    content.Sedulous.GetContent().Manifests.Load(Path.Combine("Resources", "Content", "Manifests", "Test.manifest"));
                    
                    var id = content.Sedulous.GetContent().Manifests["Test"]["Textures"]["Triangle"].CreateAssetID();
                    var json = JsonConvert.SerializeObject(id, 
                        SedulousJsonSerializerSettings.Instance);

                    TheResultingString(json)
                        .ShouldBe(@"""#Test:Textures:Triangle""");
                })
                .RunForOneFrame();
        }

        [Test]
        [Category("Content")]
        public void AssetID_DeserializesFromJson_WithXmlManifest()
        {
            GivenAnSedulousApplicationWithNoWindow()
                .WithContent(content =>
                {
                    content.Sedulous.GetContent().Manifests.Load(Path.Combine("Resources", "Content", "Manifests", "Test.manifest"));
                    
                    var id = JsonConvert.DeserializeObject<AssetID>(@"""#Test:Textures:Triangle""", 
                        SedulousJsonSerializerSettings.Instance);

                    TheResultingValue(id)
                        .ShouldBe(AssetID.Parse("#Test:Textures:Triangle"));
                })
                .RunForOneFrame();
        }

        [Test]
        [Category("Content")]
        public void AssetID_DeserializesFromJson_WithJsonManifest()
        {
            GivenAnSedulousApplicationWithNoWindow()
                .WithContent(content =>
                {
                    content.Sedulous.GetContent().Manifests.Load(Path.Combine("Resources", "Content", "Manifests", "TestJson.jsmanifest"));

                    var id = JsonConvert.DeserializeObject<AssetID>(@"""#Test:Textures:Triangle""",
                        SedulousJsonSerializerSettings.Instance);

                    TheResultingValue(id)
                        .ShouldBe(AssetID.Parse("#Test:Textures:Triangle"));
                })
                .RunForOneFrame();
        }
    }
}
