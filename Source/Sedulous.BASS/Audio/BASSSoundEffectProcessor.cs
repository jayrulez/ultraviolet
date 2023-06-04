using System;
using Sedulous.Audio;
using Sedulous.Content;

namespace Sedulous.BASS.Audio
{
    /// <summary>
    /// Loads sound effect assets.
    /// </summary>
    //[ContentProcessor]
    public sealed class BASSSoundEffectProcessor : ContentProcessor<BASSMediaDescription, SoundEffect>
    {
        /// <inheritdoc/>
        public override SoundEffect Process(ContentManager manager, IContentProcessorMetadata metadata, BASSMediaDescription input)
        {
            return input.IsFilename ?
                new BASSSoundEffect(manager.FrameworkContext, (String)input.Data) :
                new BASSSoundEffect(manager.FrameworkContext, (Byte[])input.Data);
        }
    }
}
