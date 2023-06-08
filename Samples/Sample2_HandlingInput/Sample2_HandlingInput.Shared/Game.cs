using System;
using Sedulous.SDL2;
using System.IO;
using Sample2_HandlingInput.Input;
using Sedulous;
using Sedulous.BASS;
using Sedulous.OpenGL;

namespace Sample2_HandlingInput
{
    public partial class Game : FrameworkApplication
    {
        public Game()
            : base("Sedulous", "Sample 2 - Handling Input")
        { }

        protected override FrameworkContext OnCreatingFrameworkContext()
        {
            var configuration = new SDL2FrameworkConfiguration();
            configuration.Plugins.Add(new OpenGLGraphicsPlugin());
            configuration.Plugins.Add(new BASSAudioPlugin());

            return new SDL2FrameworkContext(this, configuration);
        }

        protected override void OnLoadingContent()
        {
            LoadInputBindings();

            base.OnLoadingContent();
        }

        protected override void OnShutdown()
        {
            SaveInputBindings();

            base.OnShutdown();
        }

        protected override void OnUpdating(FrameworkTime time)
        {
            if (FrameworkContext.GetInput().GetActions().ExitApplication.IsPressed())
            {
                Exit();
            }
            base.OnUpdating(time);
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
    }
}
