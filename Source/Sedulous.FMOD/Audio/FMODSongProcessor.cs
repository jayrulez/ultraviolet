using System;
using Sedulous.Audio;
using Sedulous.Content;

namespace Sedulous.FMOD.Audio
{
    /// <summary>
    /// Loads song assets.
    /// </summary>
    [ContentProcessor]
    public sealed class FMODSongProcessor : ContentProcessor<FMODMediaDescription, Song>
    {
        /// <inheritdoc/>
        public override Song Process(ContentManager manager, IContentProcessorMetadata metadata, FMODMediaDescription input)
        {
            if (!input.IsFilename)
                throw new NotSupportedException();

            return new FMODSong(manager.FrameworkContext, (String)input.Data);
        }
    }
}
