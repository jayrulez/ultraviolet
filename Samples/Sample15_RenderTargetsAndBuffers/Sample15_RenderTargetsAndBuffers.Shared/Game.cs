using System;
using System.IO;
using Sample15_RenderTargetsAndBuffers.Assets;
using Sample15_RenderTargetsAndBuffers.Input;
using Sedulous;
using Sedulous.Audio;
using Sedulous.BASS;
using Sedulous.Content;
using Sedulous.Core;
using Sedulous.Graphics;
using Sedulous.Graphics.Graphics2D;
using Sedulous.OpenGL;
using Sedulous.SDL2;

namespace Sample15_RenderTargetsAndBuffers
{
    public partial class Game : FrameworkApplication
    {
        public Game()
            : base("Sedulous", "Sample 15 - Render Targets and Buffers")
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

            this.spriteBatch = SpriteBatch.Create();

            // A render target is composed of one or more render buffers. Each render buffer represents a particular
            // output channel from a pixel shader, such as color or depth.
            this.rbufferColor = Texture2D.CreateRenderBuffer(RenderBufferFormat.Color, 256, 256);
            this.rbufferDepth = Texture2D.CreateRenderBuffer(RenderBufferFormat.Depth16, 256, 256);
            this.rtarget = RenderTarget2D.Create(256, 256);

            // Render buffers must be explicitly attached to a render target before they can be used.
            this.rtarget.Attach(this.rbufferColor);
            this.rtarget.Attach(this.rbufferDepth);

            base.OnLoadingContent();
        }

        protected override void OnUpdating(FrameworkTime time)
        {
            // ACTION: Save Image
            if (FrameworkContext.GetInput().GetActions().SaveImage.IsPressed() || (FrameworkContext.GetInput().GetPrimaryTouchDevice()?.WasTapped() ?? false))
            {
                content.Load<SoundEffect>(GlobalSoundEffectID.Shutter).Play();

                // The SurfaceSaver class contains platform-specific functionality needed to write image
                // data to streams. We can pass a render target directly to the SaveAsPng() or SaveAsJpg() methods.
                var saver = SurfaceSaver.Create();

                // The Android and iOS platforms have restrictions on where you can save files, so we'll just
                // save to the photo gallery on those devices. We'll use a partial method to implement
                // this platform-specific behavior.
                SaveImage(saver, rtarget);

                // Alternatively, we could populate an array with the target's data using the GetData() method...
                //     var data = new Color[rtarget.Width * rtarget.Height];
                //     rtarget.GetData(data);
            }

            // ACTION: Exit Application
            if (FrameworkContext.GetInput().GetActions().ExitApplication.IsPressed())
            {
                Exit();
            }

            // Fade out save confirmation message
            if (confirmMsgOpacity > 0)
                confirmMsgOpacity -= (1.0 / 4.0) * time.ElapsedTime.TotalSeconds;

            base.OnUpdating(time);
        }

        protected override void OnDrawing(FrameworkTime time)
        {
            // We specify that we want to start drawing to a render target with the SetRenderTarget() method.
            FrameworkContext.GetGraphics().SetRenderTarget(rtarget);

            // IMPORTANT NOTE! 
            // When a render target is set for rendering, Sedulous will automatically clear it to a lovely shade of dark purple.
            // You can change this behavior by passing RenderTargetUsage.PreserveContents to the render target constructor.
            FrameworkContext.GetGraphics().Clear(Color.Black);

            var effect = content.Load<Effect>(GlobalEffectID.Noise);
            var effectTime = (Single)time.TotalTime.TotalSeconds * 0.1f;
            effect.Parameters["Time"].SetValue(effectTime);

            var blank = content.Load<Texture2D>(GlobalTextureID.Blank);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, effect);
            spriteBatch.Draw(blank, new RectangleF(0, 0, rtarget.Width, rtarget.Height), Color.White);
            spriteBatch.End();

            // When we finish drawing to a render target, we can revert to the compositor target by passing
            // null to the SetRenderTarget() method.
            FrameworkContext.GetGraphics().SetRenderTarget(null);
            FrameworkContext.GetGraphics().Clear(Color.CornflowerBlue);

            // IMPORTANT NOTE!
            // A render target (including its buffers) CANNOT BE BOUND FOR READING AND WRITING SIMULTANEOUSLY.
            // You MUST revert to a different render target before trying to draw your buffers!
            var compositor = FrameworkContext.GetPlatform().Windows.GetPrimary().Compositor;
            var compWidth = compositor.Width;
            var compHeight = compositor.Height;

            var font = content.Load<SpriteFont>(GlobalFontID.SegoeUI);

            spriteBatch.Begin(SpriteSortMode.Deferred, null);
            spriteBatch.Draw(rbufferColor, new Vector2(
                (compWidth - rbufferColor.Width) / 2,
                (compHeight - rbufferColor.Height) / 2), Color.White);

            var instruction = FrameworkContext.Platform == FrameworkPlatform.Android || FrameworkContext.Platform == FrameworkPlatform.iOS ?
                "Tap to save the image to the gallery" :
                "Press F1 to save the image to a file";

            spriteBatch.DrawString(font, instruction,
                new Vector2(8f, 8f), Color.White);
            spriteBatch.DrawString(font, confirmMsgText ?? String.Empty,
                new Vector2(8f, 8f + font.Regular.LineSpacing), Color.White * (Single)confirmMsgOpacity);

            spriteBatch.End();

            // IMPORTANT NOTE!
            // Remember, we can't be bound for both reading and writing. We're currently bound for reading,
            // so we need to remember to unbind our buffers before we write to them again in the next frame.
            // The UnbindTexture() method is provided for convenience; if you know which sampler your texture
            // is bound to, you can either unbind the sampler manually, or bind another texture to it using SetTexture().
            FrameworkContext.GetGraphics().UnbindTexture(rbufferColor);

            base.OnDrawing(time);
        }

        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                if (this.rbufferDepth != null)
                    this.rbufferDepth.Dispose();

                if (this.rbufferColor != null)
                    this.rbufferColor.Dispose();

                if (this.rtarget != null)
                    this.rtarget.Dispose();

                if (this.content != null)
                    this.content.Dispose();

                if (this.spriteBatch != null)
                    this.spriteBatch.Dispose();
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

            FrameworkContext.GetContent().Manifests["Global"]["Textures"].PopulateAssetLibrary(typeof(GlobalTextureID));
            FrameworkContext.GetContent().Manifests["Global"]["Fonts"].PopulateAssetLibrary(typeof(GlobalFontID));
            FrameworkContext.GetContent().Manifests["Global"]["Effects"].PopulateAssetLibrary(typeof(GlobalEffectID));
            FrameworkContext.GetContent().Manifests["Global"]["SoundEffects"].PopulateAssetLibrary(typeof(GlobalSoundEffectID));
        }

        partial void SaveImage(SurfaceSaver surfaceSaver, RenderTarget2D target);

        // Application resources
        private ContentManager content;
        private SpriteBatch spriteBatch;
        private RenderTarget2D rtarget;
        private RenderBuffer2D rbufferColor;
        private RenderBuffer2D rbufferDepth;

        // Save confirmation message state
        private Double confirmMsgOpacity;
        private String confirmMsgText;
    }
}
