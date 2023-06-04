using Sedulous.Audio;
using Sedulous.Core;
using System.IO;
using System.Reflection;
using System;
using Sedulous.FMOD.Audio;
using System.Linq;

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

            var asm = typeof(FMODAudioPlugin).Assembly;
            configuration.AudioSubsystemAssembly = $"{asm.GetName().Name}, Version={asm.GetName().Version}, Culture=neutral, PublicKeyToken=78da2f4877323311, processorArchitecture=MSIL";

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
                throw new InvalidCompatibilityShimException(FrameworkStrings.MissingCompatibilityShim.Format(e.FileName));
            }

            base.Configure(context, factory);
        }
    }
}
