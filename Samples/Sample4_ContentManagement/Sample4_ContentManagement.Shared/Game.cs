using System;
using System.IO;
using Sample4_ContentManagement.Assets;
using Sample4_ContentManagement.Input;
using Sedulous;
using Sedulous.BASS;
using Sedulous.Content;
using Sedulous.Graphics;
using Sedulous.OpenGL;
using Sedulous.SDL2;

namespace Sample4_ContentManagement
{
    public partial class Game : FrameworkApplication
    {
        public Game()
            : base("Sedulous", "Sample 4 - Content Management")
        { }

        protected override FrameworkContext OnCreatingFrameworkContext()
        {
            var configuration = new SDL2FrameworkConfiguration();
            configuration.Plugins.Add(new OpenGLGraphicsPlugin());
            configuration.Plugins.Add(new BASSAudioPlugin());
            
            return new SDL2FrameworkContext(this, configuration);
        }

        protected override void OnInitialized()
        {
            UsePlatformSpecificFileSource();
            LoadInputBindings();

            base.OnInitialized();
        }

        protected override void OnShutdown()
        {
            SaveInputBindings();

            base.OnShutdown();
        }

        protected override void OnLoadingContent()
        {
            this.content = ContentManager.Create("Content");
            LoadContentManifests();

            this.texture = this.content.Load<Texture2D>(GlobalTextureID.Triangle);

            this.effect = BasicEffect.Create();

            this.vbuffer = VertexBuffer.Create<VertexPositionTexture>(3);
            this.vbuffer.SetData(new[]
            {
                new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1))
            });

            this.geometryStream = GeometryStream.Create();
            this.geometryStream.Attach(this.vbuffer);

            base.OnLoadingContent();
        }

        protected override void OnUpdating(FrameworkTime time)
        {
            if (FrameworkContext.GetInput().GetActions().ExitApplication.IsPressed())
            {
                Exit();
            }

            base.OnUpdating(time);
        }

        protected override void OnDrawing(FrameworkTime time)
        {
            var gfx = FrameworkContext.GetGraphics();
            var window = FrameworkContext.GetPlatform().Windows.GetPrimary();
            var aspectRatio = window.DrawableSize.Width / (float)window.DrawableSize.Height;

            effect.World = Matrix.CreateRotationY((float)(2.0 * Math.PI * time.TotalTime.TotalSeconds));
            effect.View = Matrix.CreateLookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f, aspectRatio, 1f, 1000f);
            effect.TextureEnabled = true;
            effect.Texture = texture;
            effect.VertexColorEnabled = false;

            foreach (var pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                gfx.SetRasterizerState(RasterizerState.CullNone);
                gfx.SetGeometryStream(geometryStream);
                gfx.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
            }

            base.OnDrawing(time);
        }

        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                if (this.content != null)
                    this.content.Dispose();

                if (this.effect != null)
                    this.effect.Dispose();

                if (this.vbuffer != null)
                    this.vbuffer.Dispose();

                if (this.geometryStream != null)
                    this.geometryStream.Dispose();
            }
            base.Dispose(disposing);
        }

        private String GetInputBindingsPath()
        {
            return Path.Combine(GetRoamingApplicationSettingsDirectory(), "InputBindings.xml");
        }

        private void LoadInputBindings()
        {
            FrameworkContext.GetInput().GetActions().Load(GetInputBindingsPath(), throwIfNotFound: false);
        }

        private void SaveInputBindings()
        {
            FrameworkContext.GetInput().GetActions().Save(GetInputBindingsPath());
        }

        private void LoadContentManifests()
        {
            var uvContent = FrameworkContext.GetContent();

            var contentManifestFiles = this.content.GetAssetFilePathsInDirectory("Manifests");
            uvContent.Manifests.Load(contentManifestFiles);

            uvContent.Manifests["Global"]["Textures"].PopulateAssetLibrary(typeof(GlobalTextureID));
        }

        private ContentManager content;
        private Texture2D texture;
        private BasicEffect effect;
        private VertexBuffer vbuffer;
        private GeometryStream geometryStream;
    }
}
