using Sedulous.Audio;
using Sedulous.BASS.Audio;
using Sedulous.Core;

namespace Sedulous.BASS
{
    /// <summary>
    /// Represents an Sedulous plugin which registers BASS as the audio subsystem implementation.
    /// </summary>
    public class BASSAudioPlugin : FrameworkPlugin
    {
        /// <inheritdoc/>
        public override void Register(FrameworkConfiguration configuration)
        {
            Contract.Require(configuration, nameof(configuration));

            var asm = typeof(BASSAudioPlugin).Assembly;
            configuration.AudioSubsystemAssembly = $"{asm.GetName().Name}, Version={asm.GetName().Version}, Culture=neutral, PublicKeyToken=78da2f4877323311, processorArchitecture=MSIL";

            base.Register(configuration);
        }

        /// <inheritdoc/>
        public override void Configure(FrameworkContext context, FrameworkFactory factory)
        {
            factory.SetFactoryMethod<SongPlayerFactory>((uv) => new BASSSongPlayer(uv));
            factory.SetFactoryMethod<SoundEffectPlayerFactory>((uv) => new BASSSoundEffectPlayer(uv));

            base.Configure(context, factory);
        }
    }
}
