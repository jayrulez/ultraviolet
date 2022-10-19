using System;
using Sedulous.Audio;
using Sedulous.Core;

namespace Sedulous.FMOD.Audio
{
    /// <summary>
    /// Represents the FMOD implementation of the <see cref="ISedulousAudioDevice"/> interface.
    /// </summary>
    public sealed class FMODSedulousAudioDevice : SedulousResource, ISedulousAudioDevice
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FMODSedulousAudioDevice"/> class.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        /// <param name="id">The device's FMOD identifier.</param>
        /// <param name="name">The device's name.</param>
        public FMODSedulousAudioDevice(SedulousContext uv, Int32 id, String name)
            : base(uv)
        {
            Contract.Require(name, nameof(name));

            this.ID = id;
            this.Name = name;
        }

        /// <summary>
        /// Gets the device's FMOD identifier.
        /// </summary>
        public Int32 ID { get; }

        /// <inheritdoc/>
        public String Name { get; }

        /// <inheritdoc/>
        public Boolean IsDefault { get; internal set; }

        /// <inheritdoc/>
        public Boolean IsValid { get; internal set; }
    }
}
