using System;
using System.IO;
using Sample12_UPF.Input;
using Sample12_UPF.UI;
using Sample12_UPF.UI.Screens;
using Sedulous;
using Sedulous.BASS;
using Sedulous.Content;
using Sedulous.FreeType2;
using Sedulous.Input;
using Sedulous.OpenGL;
using Sedulous.Presentation;
using Sedulous.Presentation.Styles;
using Sedulous.SDL2;

namespace Sample12_UPF
{
    public partial class Game : FrameworkApplication
    {
        public Game()
            : this(GameFlags.None)
        { }

        public Game(GameFlags flags)
            : base("Sedulous", "Sample 12 - UPF")
        {
            this.flags = flags;
        }

        protected override FrameworkContext OnCreatingFrameworkContext()
        {
            var configuration = new SDL2FrameworkConfiguration();
            configuration.EnableServiceMode = ShouldRunInServiceMode();
            configuration.WatchViewFilesForChanges = ShouldDynamicallyReloadContent();
            configuration.Plugins.Add(new OpenGLGraphicsPlugin());
            configuration.Plugins.Add(new BASSAudioPlugin());
            configuration.Plugins.Add(new FreeTypeFontPlugin());
            configuration.Plugins.Add(new PresentationFoundationPlugin());

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
            LoadPresentation();

            if (FrameworkContext.IsRunningInServiceMode)
            {
                CompileBindingExpressions();
                Environment.Exit(0);
            }
            else
            {
                this.screenService = new UIScreenService(content);

                var screen = screenService.Get<ExampleScreen>();
                FrameworkContext.GetUI().GetScreens().Open(screen);
            }

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

        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                if (this.screenService != null)
                    this.screenService.Dispose();

                if (this.globalStyleSheet != null)
                    this.globalStyleSheet.Dispose();

                if (this.content != null)
                    this.content.Dispose();
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
        }

        private void LoadPresentation()
        {
            var upf = FrameworkContext.GetUI().GetPresentationFoundation();
            upf.RegisterKnownTypes(GetType().Assembly);

            if (!ShouldRunInServiceMode())
            {
                globalStyleSheet = GlobalStyleSheet.Create();
                globalStyleSheet.Append(content, "UI/DefaultUIStyles");
                upf.SetGlobalStyleSheet(globalStyleSheet);

                CompileBindingExpressions();
                upf.LoadCompiledExpressions();
            }
        }

        private Boolean ShouldRunInServiceMode()
        {
            return (flags & GameFlags.CompileExpressions) == GameFlags.CompileExpressions;
        }

        private Boolean ShouldCompileBindingExpressions()
        {
#if DEBUG
            return true;
#else
            return (flags & GameFlags.CompileExpressions) == GameFlags.CompileExpressions || System.Diagnostics.Debugger.IsAttached;
#endif
        }

        private Boolean ShouldDynamicallyReloadContent()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }

        private void CompileBindingExpressions()
        {
            if (!ShouldCompileBindingExpressions())
                return;

            var compilationFlags = CompileExpressionsFlags.None;
            if ((this.flags & GameFlags.CompileExpressions) == GameFlags.CompileExpressions)
            {
                compilationFlags |= CompileExpressionsFlags.ResolveContentFiles;
                compilationFlags |= CompileExpressionsFlags.IgnoreCache;
            }

            var upf = FrameworkContext.GetUI().GetPresentationFoundation();
            upf.CompileExpressionsIfSupported("Content", compilationFlags);
        }

        // The global content manager.  Manages any content that should remain loaded for the duration of the game's execution.
        private ContentManager content;

        // State values.
        private readonly GameFlags flags;
        private GlobalStyleSheet globalStyleSheet;
        private UIScreenService screenService;
    }
}
