using System;
using Sedulous.Audio;
using Sedulous.Content;

namespace Sedulous.FMOD.Audio
{
    /// <summary>
    /// Loads sound effect assets.
    /// </summary>
    //[ContentProcessor]
    public sealed class FMODSoundEffectProcessor : ContentProcessor<FMODMediaDescription, SoundEffect>
    {
        /// <inheritdoc/>
        public override SoundEffect Process(ContentManager manager, IContentProcessorMetadata metadata, FMODMediaDescription input)
        {
            if (!input.IsFilename)
                throw new NotSupportedException();

            return new FMODSoundEffect(manager.FrameworkContext, (String)input.Data);
        }
    }
}
