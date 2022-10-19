using System;
using Sedulous.Audio;
using Sedulous.Content;

namespace Sedulous.BASS.Audio
{
    /// <summary>
    /// Loads song assets.
    /// </summary>
    [ContentProcessor]
    public sealed class BASSSongProcessor : ContentProcessor<BASSMediaDescription, Song>
    {
        /// <inheritdoc/>
        public override Song Process(ContentManager manager, IContentProcessorMetadata metadata, BASSMediaDescription input)
        {
            if (!input.IsFilename)
                throw new NotSupportedException();

            return new BASSSong(manager.Sedulous, (String)input.Data);
        }
    }
}
