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

            base.Register(configuration);
        }

        /// <inheritdoc/>
        public override void Configure(FrameworkContext context, FrameworkFactory factory)
        {
            factory.SetFactoryMethod<SongPlayerFactory>((uv) => new BASSSongPlayer(uv));
            factory.SetFactoryMethod<SoundEffectPlayerFactory>((uv) => new BASSSoundEffectPlayer(uv));

            factory.SetFactoryMethod<FrameworkAudioFactory>((uv, configuration) => new BASSAudioSubsystem(uv));

            base.Configure(context, factory);
        }

        /// <inheritdoc/>
        public override void Initialize(FrameworkContext context, FrameworkFactory factory)
        {
            var importers = context.GetContent().Importers;
            {
                importers.RegisterImporter<BASSMediaImporter>(".mp3");
                importers.RegisterImporter<BASSMediaImporter>(".ogg");
                importers.RegisterImporter<BASSMediaImporter>(".wav");
            }

            var processors = context.GetContent().Processors;
            {
                processors.RegisterProcessor<BASSSongProcessor>();
                processors.RegisterProcessor<BASSSoundEffectProcessor>();
            }

            base.Initialize(context, factory);
        }
    }
}
