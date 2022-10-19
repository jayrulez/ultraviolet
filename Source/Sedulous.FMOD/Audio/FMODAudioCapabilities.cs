using System;
using Sedulous.Audio;

namespace Sedulous.FMOD.Audio
{
    /// <inheritdoc/>
    public sealed class FMODAudioCapabilities : AudioCapabilities
    {
        /// <inheritdoc/>
        public override Boolean SupportsPitchShifting { get; } = true;
    }
}
