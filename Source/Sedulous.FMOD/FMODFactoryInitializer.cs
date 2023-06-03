using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Sedulous.Audio;
using Sedulous.Core;
using Sedulous.FMOD.Audio;

namespace Sedulous.FMOD
{
    /// <summary>
    /// Initializes factory methods for the FMOD implementation of the audio subsystem.
    /// </summary>
    public sealed class FMODFactoryInitializer : IFrameworkFactoryInitializer
    {
        /// <inheritdoc/>
        public void Initialize(FrameworkContext owner, FrameworkFactory factory)
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
        }
    }
}
