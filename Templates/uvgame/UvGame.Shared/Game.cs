using System;
using Sedulous;
using Sedulous.BASS;
using Sedulous.Content;
using Sedulous.FreeType2;
using Sedulous.Graphics;
using Sedulous.Graphics.Graphics2D;
using Sedulous.Input;
using Sedulous.OpenGL;
using Sedulous.SDL2;

namespace UvGame
{
    public partial class Game : SedulousApplication
    {
        public Game()
            : base("DEVELOPER_PLACEHOLDER", "APPLICATION_PLACEHOLDER")
        { }

        protected override SedulousContext OnCreatingSedulousContext()
        {
            var configuration = new SDL2SedulousConfiguration();
            configuration.Plugins.Add(new OpenGLGraphicsPlugin());
            configuration.Plugins.Add(new BASSAudioPlugin());
            configuration.Plugins.Add(new FreeTypeFontPlugin());

//-:cnd:noEmit
#if DEBUG
            configuration.Debug = true;
            configuration.DebugLevels = DebugLevels.Error | DebugLevels.Warning;
            configuration.DebugCallback = (uv, level, message) =>
            {
                System.Diagnostics.Debug.WriteLine(message);
            };
#endif
//+:cnd:noEmit

            return new SDL2SedulousContext(this, configuration);
        }

        protected override void OnInitialized()
        {
            UsePlatformSpecificFileSource();
            base.OnInitialized();
        }

        protected override void OnLoadingContent()
        {
            this.contentManager = ContentManager.Create("Content");
            this.spriteBatch = SpriteBatch.Create();
            this.texture = this.contentManager.Load<Texture2D>("desktop_uv256");

            base.OnLoadingContent();
        }

        protected override void OnUpdating(SedulousTime time)
        {
            if (Sedulous.GetInput().GetKeyboard().IsKeyPressed(Key.Escape))
            {
                Exit();
            }
            base.OnUpdating(time);
        }

        protected override void OnDrawing(SedulousTime time)
        {
            var window = Sedulous.GetPlatform().Windows.GetCurrent();
            var position = new Vector2(window.ClientSize.Width / 2f, window.ClientSize.Height / 2f);
            var origin = new Vector2(texture.Width / 2f, texture.Height / 2f);

            this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            this.spriteBatch.Draw(texture, position, null, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
            this.spriteBatch.End();

            base.OnDrawing(time);
        }

        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                if (this.contentManager != null)
                    this.contentManager.Dispose();

                if (this.spriteBatch != null)
                    this.spriteBatch.Dispose();
            }
            base.Dispose(disposing);
        }

        private ContentManager contentManager;
        private SpriteBatch spriteBatch;
        private Texture2D texture;
    }
}
