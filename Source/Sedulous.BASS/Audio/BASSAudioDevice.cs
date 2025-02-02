﻿using System;
using Sedulous.Audio;
using Sedulous.Core;

namespace Sedulous.BASS.Audio
{
    /// <summary>
    /// Represents the BASS implementation of the <see cref="IAudioDevice"/> interface.
    /// </summary>
    public sealed class BASSAudioDevice : FrameworkResource, IAudioDevice
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BASSAudioDevice"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="id">The device's BASS identifier.</param>
        /// <param name="name">The device's name.</param>
        public BASSAudioDevice(FrameworkContext context, UInt32 id, String name)
            : base(context)
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
