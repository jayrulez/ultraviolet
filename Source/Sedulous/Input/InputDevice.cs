using System;

namespace Sedulous.Input
{
    /// <summary>
    /// Represents any input device.
    /// </summary>
    public abstract class InputDevice : FrameworkResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputDevice"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        internal InputDevice(FrameworkContext context)
            : base(context)
        {

        }

        /// <summary>
        /// Updates the device's state.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Update(FrameworkTime)"/>.</param>
        public abstract void Update(FrameworkTime time);

        /// <summary>
        /// Gets a value indicating whether the device has been registered with the context
        /// as a result of receiving user input.
        /// </summary>
        public abstract Boolean IsRegistered { get; }
    }
}
