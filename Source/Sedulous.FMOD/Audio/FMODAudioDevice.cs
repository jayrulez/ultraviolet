using System;
using Sedulous.Audio;
using Sedulous.Core;

namespace Sedulous.FMOD.Audio
{
    /// <summary>
    /// Represents the FMOD implementation of the <see cref="IAudioDevice"/> interface.
    /// </summary>
    public sealed class FMODAudioDevice : FrameworkResource, IAudioDevice
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FMODAudioDevice"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="id">The device's FMOD identifier.</param>
        /// <param name="name">The device's name.</param>
        public FMODAudioDevice(FrameworkContext context, Int32 id, String name)
            : base(context)
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
