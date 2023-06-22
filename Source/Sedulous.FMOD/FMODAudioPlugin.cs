using Sedulous.Audio;
using Sedulous.Core;
using System.IO;
using System.Reflection;
using System;
using Sedulous.FMOD.Audio;
using System.Linq;
using Sedulous.Content;

namespace Sedulous.FMOD
{
    /// <summary>
    /// Represents an Sedulous plugin which registers FMOD as the audio subsystem implementation.
    /// </summary>
    public class FMODAudioPlugin : FrameworkPlugin
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
            factory.SetFactoryMethod<SongPlayerFactory>((uv) => new FMODSongPlayer(uv));
            factory.SetFactoryMethod<SoundEffectPlayerFactory>((uv) => new FMODSoundEffectPlayer(uv));

            try
            {
                if (FrameworkPlatformInfo.CurrentPlatform == FrameworkPlatform.Android)
                {
                    var shim = Assembly.Load("Sedulous.Shims.Android.FMOD.dll");
                    var type = shim.GetTypes().Where(x => x.IsClass && !x.IsAbstract && typeof(FMODPlatformSpecificImplementationDetails).IsAssignableFrom(x)).SingleOrDefault();
                    if (type == null)
                        throw new InvalidOperationException(FMODStrings.CannotFindPlatformShimClass);

                    factory.SetFactoryMethod<FMODPlatformSpecificImplementationDetailsFactory>(
                        (uv) => (FMODPlatformSpecificImplementationDetails)Activator.CreateInstance(type));
                }
            }
            catch (FileNotFoundException e)
            {
                throw new Exception(FrameworkStrings.MissingCompatibilityShim.Format(e.FileName));
            }

            factory.SetFactoryMethod<FrameworkAudioFactory>((uv, configuration) => new FMODAudioSubsystem(uv, configuration));

            base.Configure(context, factory);
        }

        /// <inheritdoc/>
        public override void Initialize(FrameworkContext context, FrameworkFactory factory)
        {
            var importers = context.GetContent().Importers;
            {
                importers.RegisterImporter<FMODMediaImporter>(".aif");
                importers.RegisterImporter<FMODMediaImporter>(".aiff");
                importers.RegisterImporter<FMODMediaImporter>(".flac");
                importers.RegisterImporter<FMODMediaImporter>(".it");
                importers.RegisterImporter<FMODMediaImporter>(".m3u");
                importers.RegisterImporter<FMODMediaImporter>(".mid");
                importers.RegisterImporter<FMODMediaImporter>(".mod");
                importers.RegisterImporter<FMODMediaImporter>(".mp2");
                importers.RegisterImporter<FMODMediaImporter>(".mp3");
                importers.RegisterImporter<FMODMediaImporter>(".ogg");
                importers.RegisterImporter<FMODMediaImporter>(".s3m");
                importers.RegisterImporter<FMODMediaImporter>(".wav");
            }

            var processors = context.GetContent().Processors;
            {
                processors.RegisterProcessor<FMODSongProcessor>();
                processors.RegisterProcessor<FMODSoundEffectProcessor>();
            }

            base.Initialize(context, factory);
        }
    }
}
