using System;
using System.IO;
using Sample9_ManagingStateWithUIScreens.Assets;
using Sample9_ManagingStateWithUIScreens.Input;
using Sample9_ManagingStateWithUIScreens.UI;
using Sample9_ManagingStateWithUIScreens.UI.Screens;
using Sedulous;
using Sedulous.BASS;
using Sedulous.Content;
using Sedulous.OpenGL;
using Sedulous.SDL2;

namespace Sample9_ManagingStateWithUIScreens
{
    public partial class Game : FrameworkApplication
    {
        public Game()
            : base("Sedulous", "Sample 9 - Managing State with UI Screens")
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
            var screen = screenService.Get<SampleScreen1>();
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

            FrameworkContext.GetContent().Manifests["Global"]["Textures"].PopulateAssetLibrary(typeof(GlobalTextureID));
        }

        private ContentManager content;
    }
}
