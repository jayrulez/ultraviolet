using Sandbox3D.Input;
using Sandbox3D.UI;
using Sedulous;
using Sedulous.BASS;
using Sedulous.Content;
using Sedulous.Core;
using Sedulous.Core.Text;
using Sedulous.FreeType2;
using Sedulous.Graphics;
using Sedulous.Graphics.Graphics3D;
using Sedulous.OpenGL;
using Sedulous.Platform;
using Sedulous.Presentation;
using Sedulous.Presentation.Controls.Primitives;
using Sedulous.Presentation.Styles;
using Sedulous.SDL2;
using SharpGLTF.Runtime;
using System;
using System.IO;
using System.Reflection;

namespace Sandbox3D
{
    /// <summary>
    /// Represents the main application object.
    /// </summary>
    public partial class Game : FrameworkApplication
    {
        /// <summary>
        /// Initializes a new instance of the Game 
        /// </summary>
        public Game() : base("Sedulous", "Sandbox3D")
        {
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
            contextConfig.WatchViewFilesForChanges = ShouldDynamicallyReloadContent();
            contextConfig.Plugins.Add(new OpenGLGraphicsPlugin(graphicsConfig));
            contextConfig.Plugins.Add(new BASSAudioPlugin());
            contextConfig.Plugins.Add(new FreeTypeFontPlugin());
            contextConfig.Plugins.Add(new PresentationFoundationPlugin());
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
            if (!SetFileSourceFromManifestIfExists("Sandbox3D.Content.uvarc"))
                UsePlatformSpecificFileSource();

            base.OnInitialized();
        }

        /// <summary>
        /// Called when the application is loading content.
        /// </summary>
        protected override void OnLoadingContent()
        {
            ContentManager.GloballySuppressDependencyTracking = !ShouldDynamicallyReloadContent();
            this.content = ContentManager.Create("Content");


            LoadLocalizationPlugins();
            LoadLocalizationDatabases();
            LoadInputBindings();
            LoadContentManifests();
            LoadPresentation();
            LoadStaticModel();
            LoadSkinnedModel();

            this.screenService = new UIScreenService(content);

            GC.Collect(2);

            base.OnLoadingContent();
        }

        /// <summary>
        /// Loads the application's localization plugins.
        /// </summary>
        protected void LoadLocalizationPlugins()
        {
            var fss = FileSystemService.Create();
            var plugins = content.GetAssetFilePathsInDirectory(Path.Combine("Localization", "Plugins"), "*.dll");
            foreach (var plugin in plugins)
            {
                try
                {
                    var asm = Assembly.Load(plugin);
                    Localization.LoadPlugins(asm);
                }
                catch (Exception e) when (e is BadImageFormatException || e is FileLoadException) { }
            }
        }

        /// <summary>
        /// Loads the application's localization databases.
        /// </summary>
        protected void LoadLocalizationDatabases()
        {
            var fss = FileSystemService.Create();
            var databases = content.GetAssetFilePathsInDirectory("Localization", "*.xml");
            foreach (var database in databases)
            {
                using (var stream = fss.OpenRead(database))
                {
                    Localization.Strings.LoadFromStream(stream);
                }
            }
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

        /// <summary>
        /// Loads files necessary for the Presentation Foundation.
        /// </summary>
        protected void LoadPresentation()
        {
#if !IMGUI
            var upf = FrameworkContext.GetUI().GetPresentationFoundation();
            upf.RegisterKnownTypes(GetType().Assembly);

            globalStyleSheet = GlobalStyleSheet.Create();
            globalStyleSheet.Append(content, "UI/DefaultUIStyles");
            globalStyleSheet.Append(content, "UI/GameStyles");
            upf.SetGlobalStyleSheet(globalStyleSheet);

            upf.LoadCompiledExpressions();
#endif
        }

        private void LoadStaticModel()
        {
            this.model = this.content.Load<Model>("Models/glTF/Frog.gltf");

            modelSceneRenderer = new ModelSceneRenderer();
        }

        private void RenderStaticModel(ref Matrix worldMatrix)
        {
            modelSceneRenderer.Draw(model.Scenes.DefaultScene, perspectiveCamera, ref worldMatrix);
        }

        private void LoadSkinnedModel()
        {
            this.skinnedModel = this.content.Load<SkinnedModel>("Models/glTF/Orc.gltf");
            this.modelInstance = new SkinnedModelInstance(skinnedModel);

            track = modelInstance.PlayAnimation(SkinnedAnimationMode.Loop, "Idle");

            skinnedModelSceneRenderer = new SkinnedModelSceneRenderer();
        }

        private void RenderSkinnedModel(ref Matrix worldMatrix)
        {
            skinnedModelSceneRenderer.Draw(modelInstance.Scenes.DefaultScene, perspectiveCamera, ref worldMatrix);
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
            //var gfx = FrameworkContext.GetGraphics();
            //var window = FrameworkContext.GetPlatform().Windows.GetCurrent();
            //var aspectRatio = window.DrawableSize.Width / (Single)window.DrawableSize.Height;

            //effect.World = Matrix.CreateRotationY((float)(2.0 * Math.PI * (time.TotalTime.TotalSeconds / 10.0)));
            //effect.View = Matrix.CreateLookAt(new Vector3(0, 3, 6), new Vector3(0, 0.75f, 0), Vector3.Up);
            //effect.Projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f, aspectRatio, 1f, 1000f);

            if(perspectiveCamera == null)
            {
                perspectiveCamera = PerspectiveCamera.Create();
                perspectiveCamera.Position = new Vector3(8, 0, -10);
                perspectiveCamera.Target = new Vector3(0, 0, 0);
                perspectiveCamera.Update();
            }

            var worldMatrix = Matrix.Identity;

            RenderStaticModel(ref worldMatrix);

            //RenderSkinnedModel(ref worldMatrix);

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
                SafeDispose.DisposeRef(ref screenService);
                SafeDispose.DisposeRef(ref globalStyleSheet);
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

        // State values.
        private GlobalStyleSheet globalStyleSheet;
        private UIScreenService screenService;

        // 3D geometry testing.

        private PerspectiveCamera perspectiveCamera;

        private ModelSceneRenderer modelSceneRenderer;
        private Model model;

        private SkinnedModelSceneRenderer skinnedModelSceneRenderer;
        private SkinnedModel skinnedModel;
        private SkinnedModelInstance modelInstance;
        private SkinnedAnimationTrack track;
    }
}
