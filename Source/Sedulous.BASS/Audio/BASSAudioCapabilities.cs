using System;
using Sedulous.Audio;

namespace Sedulous.BASS.Audio
{
    /// <inheritdoc/>
    public sealed class BASSAudioCapabilities : AudioCapabilities
    {
        /// <inheritdoc/>
        public override Boolean SupportsPitchShifting { get; } = false;
    }
}
