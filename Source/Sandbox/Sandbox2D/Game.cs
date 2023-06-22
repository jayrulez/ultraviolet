using Sandbox2D.Input;
using Sedulous;
using Sedulous.BASS;
using Sedulous.Content;
using Sedulous.Core;
using Sedulous.Core.Text;
using Sedulous.FreeType2;
using Sedulous.Graphics;
using Sedulous.Graphics.Graphics2D;
using Sedulous.Graphics.Graphics2D.Text;
using Sedulous.OpenGL;
using Sedulous.Platform;
using Sedulous.Presentation;
using Sedulous.SDL2;
using System;
using System.IO;
using System.Reflection;

namespace Sandbox2D
{
    public class Tile
    {
        public Texture2D texture;
    }

    /// <summary>
    /// Represents the main application object.
    /// </summary>
    public partial class Game : FrameworkApplication
    {
        /// <summary>
        /// Initializes a new instance of the Game 
        /// </summary>
        public Game() : base("Sedulous", "Sandbox2D")
        {
            IsFixedTimeStep = false;
            Diagnostics.DrawDiagnosticsVisuals = true;
            PlatformSpecificInitialization();
        }



        /// <summary>
        /// Called when the application is creating its Sedulous context.
        /// </summary>
        /// <returns>The Sedulous context.</returns>
        protected override FrameworkContext OnCreatingFrameworkContext()
        {
            var graphicsConfig = OpenGLGraphicsConfiguration.Default;
            graphicsConfig.MultiSampleBuffers = 1;
            graphicsConfig.MultiSampleSamples = 8;
            graphicsConfig.SrgbBuffersEnabled = false;
            graphicsConfig.SrgbDefaultForTexture2D = false;

            var contextConfig = new SDL2FrameworkConfiguration();
            contextConfig.SupportsHighDensityDisplayModes = true;
            contextConfig.EnableServiceMode = false;
            contextConfig.WatchViewFilesForChanges = false;
            contextConfig.Plugins.Add(new OpenGLGraphicsPlugin(graphicsConfig));
            contextConfig.Plugins.Add(new BASSAudioPlugin());
            contextConfig.Plugins.Add(new FreeTypeFontPlugin());
            PopulateConfiguration(contextConfig);

#if DEBUG
            contextConfig.Debug = true;
            contextConfig.DebugLevels = DebugLevels.Error | DebugLevels.Warning;
            contextConfig.DebugCallback = (uv, level, message) =>
            {
                System.Diagnostics.Debug.WriteLine(message);
            };
#endif
            return new SDL2FrameworkContext(this, contextConfig);
        }

        /// <summary>
        /// Called after the application has been initialized.
        /// </summary>
        protected override void OnInitialized()
        {
            if (!SetFileSourceFromManifestIfExists("Sandbox2D.Content.uvarc"))
                UsePlatformSpecificFileSource();

            base.OnInitialized();

            FrameworkContext.GetPlatform().Windows.GetPrimary().ClientSize = new Size2(1920, 1080);
        }

        /// <summary>
        /// Called when the application is loading content.
        /// </summary>
        protected override void OnLoadingContent()
        {
            ContentManager.GloballySuppressDependencyTracking = !ShouldDynamicallyReloadContent();
            this.content = ContentManager.Create("Content");

            LoadInputBindings();
            LoadContentManifests();

            LoadTeturesAndFonts();

            GC.Collect(2);

            base.OnLoadingContent();
        }

        /// <summary>
        /// Loads the game's input bindings.
        /// </summary>
        protected void LoadInputBindings()
        {
            var inputBindingsPath = Path.Combine(GetRoamingApplicationSettingsDirectory(), "InputBindings.xml");
            FrameworkContext.GetInput().GetActions().Load(inputBindingsPath, throwIfNotFound: false);
        }

        /// <summary>
        /// Saves the game's input bindings.
        /// </summary>
        protected void SaveInputBindings()
        {
            var inputBindingsPath = Path.Combine(GetRoamingApplicationSettingsDirectory(), "InputBindings.xml");
            FrameworkContext.GetInput().GetActions().Save(inputBindingsPath);
        }

        /// <summary>
        /// Loads the game's content manifest files.
        /// </summary>
        protected void LoadContentManifests()
        {
            var uvContent = FrameworkContext.GetContent();

            var contentManifestFiles = this.content.GetAssetFilePathsInDirectory("Manifests");
            uvContent.Manifests.Load(contentManifestFiles);
        }

        private Texture2D GenerateTexture(Color color)
        {
            Texture2D texture = Texture2D.CreateTexture(16, 16);
            var cdata = new Color[16 * 16];
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    cdata[i * 16 + j] = color;
                }
            }
            texture.SetData(cdata);
            return texture;
        }

        private void LoadTeturesAndFonts()
        {
            textRenderer = new TextRenderer();
            spriteBatch = SpriteBatch.Create();

            t1 = GenerateTexture(Color.DarkGreen);
            t2 = GenerateTexture(Color.DarkCyan);
            font = content.Load<FrameworkFont>("Fonts/SegoeUI");

            int c = 0;
            tiles = new Tile[sizeX, sizeY];
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    tiles[x, y] = new Tile();
                    tiles[x, y].texture = (c % 2) == 0 ? t1 : t2;
                    c++;
                }
            }

        }


        /// <summary>
        /// Called when the application state is being updated.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to Update.</param>
        protected override void OnUpdating(FrameworkTime time)
        {
            if (FrameworkContext.GetInput().GetActions().ExitApplication.IsPressed())
            {
                Exit();
            }
            base.OnUpdating(time);
        }

        /// <summary>
        /// Called when the application's scene is being drawn.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to Draw.</param>
        protected override void OnDrawing(FrameworkTime time)
        {
            var gfx = FrameworkContext.GetGraphics();
            //var window = FrameworkContext.GetPlatform().Windows.GetCurrent();
            //var aspectRatio = window.DrawableSize.Width / (Single)window.DrawableSize.Height;

            //var world = Matrix.CreateRotationY((float)(2.0 * Math.PI * (time.TotalTime.TotalSeconds / 10.0)));
            //var view = Matrix.CreateLookAt(new Vector3(0, 3, 6), new Vector3(0, 0.75f, 0), Vector3.Up);
            //var projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f, aspectRatio, 1f, 1000f);

            spriteBatch.Begin();
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    spriteBatch.Draw(tiles[x, y].texture, new Vector2(x * 16, y * 16), Color.White);
                }
            }

            double fps = 1.0 / time.ElapsedTime.TotalSeconds;
            var settingsTopLeft = new TextLayoutSettings(font, 200, 100, TextFlags.AlignTop | TextFlags.AlignLeft);
            textRenderer.Draw(spriteBatch, fps.ToString(), Vector2.Zero, Color.Red, settingsTopLeft);

            spriteBatch.End();
            base.OnDrawing(time);
        }

        /// <summary>
        /// Called when the application is being shut down.
        /// </summary>
        protected override void OnShutdown()
        {
            SaveInputBindings();

            base.OnShutdown();
        }

        /// <summary>
        /// Releases resources associated with the object.
        /// </summary>
        /// <param name="disposing">true if the object is being disposed; false if the object is being finalized.</param>
        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                SafeDispose.DisposeRef(ref content);
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Performs any platform-specific initialization.
        /// </summary>
        partial void PlatformSpecificInitialization();

        /// <summary>
        /// Gets a value indicating whether the game should enable dynamic reloading of content assets.
        /// </summary>
        private Boolean ShouldDynamicallyReloadContent()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }

        // The global content manager.  Manages any content that should remain loaded for the duration of the game's execution.
        private ContentManager content;

        private SpriteBatch spriteBatch;

        private Texture2D t1;
        private Texture2D t2;
        private FrameworkFont font;

        private int sizeX = 120;
        private int sizeY = 67;
        private Tile[,] tiles;
        private TextRenderer textRenderer;
    }
}
