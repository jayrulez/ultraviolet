using Sedulous.Audio;
using Sedulous.BASS.Audio;
using Sedulous.Content;
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
        public override void RegisterContentImporters(ContentImporterRegistry importers)
        {
            importers.RegisterImporter<BASSMediaImporter>(".mp3");
            importers.RegisterImporter<BASSMediaImporter>(".ogg");
            importers.RegisterImporter<BASSMediaImporter>(".wav");

            base.RegisterContentImporters(importers);
        }

        /// <inheritdoc/>
        public override void RegisterContentProcessors(ContentProcessorRegistry processors)
        {
            processors.RegisterProcessor<BASSSongProcessor>();
            processors.RegisterProcessor<BASSSoundEffectProcessor>();

            base.RegisterContentProcessors(processors);
        }
    }
}
