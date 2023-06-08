using System;
using System.IO;
using Sample3_RenderingGeometry.Input;
using Sedulous;
using Sedulous.BASS;
using Sedulous.Graphics;
using Sedulous.OpenGL;
using Sedulous.SDL2;

namespace Sample3_RenderingGeometry
{
    public partial class Game : FrameworkApplication
    {
        public Game()
            : base("Sedulous", "Sample 3 - Rendering Geometry")
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
            this.effect = BasicEffect.Create();

            this.vbuffer = VertexBuffer.Create<VertexPositionColor>(3);
            this.vbuffer.SetData(new[]
            {
                new VertexPositionColor(new Vector3(0, 1, 0), Color.Red),
                new VertexPositionColor(new Vector3(1, -1, 0), Color.Lime),
                new VertexPositionColor(new Vector3(-1, -1, 0), Color.Blue),
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
            effect.VertexColorEnabled = true;

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

        private BasicEffect effect;
        private VertexBuffer vbuffer;
        private GeometryStream geometryStream;
    }
}
