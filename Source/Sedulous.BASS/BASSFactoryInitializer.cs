using Sedulous.Audio;
using Sedulous.BASS.Audio;

namespace Sedulous.BASS
{
    /// <summary>
    /// Initializes factory methods for the BASS implementation of the audio subsystem.
    /// </summary>
    public sealed class BASSFactoryInitializer : IFrameworkFactoryInitializer
    {
        /// <inheritdoc/>
        public void Initialize(FrameworkContext owner, FrameworkFactory factory)
        {
            factory.SetFactoryMethod<SongPlayerFactory>((uv) => new BASSSongPlayer(uv));
            factory.SetFactoryMethod<SoundEffectPlayerFactory>((uv) => new BASSSoundEffectPlayer(uv));
        }
    }
}
