using System;
using Sedulous.Audio;
using Sedulous.Content;

namespace Sedulous.BASS.Audio
{
    /// <summary>
    /// Loads sound effect assets.
    /// </summary>
    [ContentProcessor]
    public sealed class BASSSoundEffectProcessor : ContentProcessor<BASSMediaDescription, SoundEffect>
    {
        /// <inheritdoc/>
        public override SoundEffect Process(ContentManager manager, IContentProcessorMetadata metadata, BASSMediaDescription input)
        {
            return input.IsFilename ?
                new BASSSoundEffect(manager.Sedulous, (String)input.Data) :
                new BASSSoundEffect(manager.Sedulous, (Byte[])input.Data);
        }
    }
}
