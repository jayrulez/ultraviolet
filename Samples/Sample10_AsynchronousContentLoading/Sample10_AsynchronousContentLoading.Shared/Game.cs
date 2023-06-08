using System;
using System.IO;
using Sample10_AsynchronousContentLoading.Assets;
using Sample10_AsynchronousContentLoading.Input;
using Sample10_AsynchronousContentLoading.UI;
using Sample10_AsynchronousContentLoading.UI.Screens;
using Sedulous;
using Sedulous.BASS;
using Sedulous.Content;
using Sedulous.Graphics.Graphics2D;
using Sedulous.OpenGL;
using Sedulous.SDL2;

namespace Sample10_AsynchronousContentLoading
{
    public partial class Game : FrameworkApplication
    {
        public Game()
            : base("Sedulous", "Sample 10 - Asynchronous Content Loading")
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

            var screenService = new UIScreenService(content);
            var screen = screenService.Get<LoadingScreen>();

            screen.SetContentManager(this.content);
            screen.AddLoadingStep("Loading, please wait...");
            screen.AddLoadingStep(FrameworkContext.GetContent().Manifests["Global"]);
            screen.AddLoadingDelay(2500);
            screen.AddLoadingStep("Loading interface...");
            screen.AddLoadingStep(screenService.Load);
            screen.AddLoadingDelay(2500);
            screen.AddLoadingStep("Reticulating splines...");
            screen.AddLoadingDelay(2500);

            FrameworkContext.GetUI().GetScreens().Open(screen, TimeSpan.Zero);

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

            FrameworkContext.GetContent().Manifests["Global"]["Fonts"].PopulateAssetLibrary(typeof(GlobalFontID));
            FrameworkContext.GetContent().Manifests["Global"]["Textures"].PopulateAssetLibrary(typeof(GlobalTextureID));
        }

        private ContentManager content;
    }
}
