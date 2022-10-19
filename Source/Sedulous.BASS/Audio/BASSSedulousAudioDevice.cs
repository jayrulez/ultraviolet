using System;
using Sedulous.Audio;
using Sedulous.Core;

namespace Sedulous.BASS.Audio
{
    /// <summary>
    /// Represents the BASS implementation of the <see cref="ISedulousAudioDevice"/> interface.
    /// </summary>
    public sealed class BASSSedulousAudioDevice : SedulousResource, ISedulousAudioDevice
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BASSSedulousAudioDevice"/> class.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        /// <param name="id">The device's BASS identifier.</param>
        /// <param name="name">The device's name.</param>
        public BASSSedulousAudioDevice(SedulousContext uv, UInt32 id, String name)
            : base(uv)
        {
            Contract.Require(name, nameof(name));

            this.ID = id;
            this.Name = name;
        }

        /// <summary>
        /// Gets the device's BASS identifier.
        /// </summary>
        public UInt32 ID { get; }

        /// <inheritdoc/>
        public String Name { get; }

        /// <inheritdoc/>
        public Boolean IsDefault { get; internal set; }

        /// <inheritdoc/>
        public Boolean IsValid { get; internal set; }
    }
}
