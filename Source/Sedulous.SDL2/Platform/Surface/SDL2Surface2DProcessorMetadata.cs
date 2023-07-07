using System;

namespace Sedulous.SDL2.Platform.Surface
{
    /// <summary>
    /// Contains metadata for <see cref="SDL2Surface2DProcessor"/>.
    /// </summary>
    internal sealed class SDL2Surface2DProcessorMetadata
    {
        /// <summary>
        /// Gets or sets a value indicating whether the surface is SRGB encoded. If <see langword="null"/>, the
        /// value specified by the <see cref="FrameworkContextProperties.SrgbDefaultForSurface2D"/> property is used.
        /// </summary>
        public Boolean? SrgbEncoded { get; set; }
    }
}
