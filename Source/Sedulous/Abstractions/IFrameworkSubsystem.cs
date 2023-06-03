using System;

namespace Sedulous
{
    /// <summary>
    /// Represents the method that is called when an Sedulous subsystem updates its state.
    /// </summary>
    /// <param name="subsystem">The Sedulous subsystem.</param>
    /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Update(FrameworkTime)"/>.</param>
    public delegate void SedulousSubsystemUpdateEventHandler(IFrameworkSubsystem subsystem, FrameworkTime time);

    /// <summary>
    /// Represents one of Sedulous's subsystems.
    /// </summary>
    public interface IFrameworkSubsystem : IFrameworkComponent, IDisposable
    {
        /// <summary>
        /// Updates the subsystem's state.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Update(FrameworkTime)"/>.</param>
        void Update(FrameworkTime time);

        /// <summary>
        /// Gets a value indicating whether the object has been disposed.
        /// </summary>
        Boolean Disposed { get; }

        /// <summary>
        /// Occurs when the subsystem is updating its state.
        /// </summary>
        event SedulousSubsystemUpdateEventHandler Updating;
    }
}
