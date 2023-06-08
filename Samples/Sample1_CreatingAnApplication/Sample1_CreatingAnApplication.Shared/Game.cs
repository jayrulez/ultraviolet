using Sedulous;
using Sedulous.BASS;
using Sedulous.OpenGL;
using Sedulous.SDL2;

namespace Sample1_CreatingAnApplication
{
    public partial class Game : FrameworkApplication
    {
        public Game()
            : base("Sedulous", "Sample 1 - Creating an Application")
        { }

        protected override FrameworkContext OnCreatingFrameworkContext()
        {
            var configuration = new SDL2FrameworkConfiguration();
            configuration.Plugins.Add(new OpenGLGraphicsPlugin());
            configuration.Plugins.Add(new BASSAudioPlugin());

            return new SDL2FrameworkContext(this, configuration);
        }
    }
}
