using System;

namespace Sedulous
{
    /// <summary>
    /// Represents a factory method which constructs instances of the <see cref="FrameworkProfilerBase"/> class.
    /// </summary>
    /// <param name="context">The Sedulous context.</param>
    /// <returns>The instance of <see cref="FrameworkProfilerBase"/> that was created.</returns>
    public delegate FrameworkProfilerBase SedulousProfilerFactory(FrameworkContext context);

    /// <summary>
    /// Represents the base class for Sedulous profilers.
    /// </summary>
    public abstract class FrameworkProfilerBase : FrameworkResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkProfilerBase"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        public FrameworkProfilerBase(FrameworkContext context)
            : base(context)
        {

        }

        /// <summary>
        /// Requests that the profiler take a snapshot that contains the entirety of the next frame.
        /// </summary>
        public abstract void TakeSnapshotOfNextFrame();

        /// <summary>
        /// Begins a profiler section.
        /// </summary>
        /// <param name="name">The name of the section to begin.</param>
        public abstract void BeginSection(String name);

        /// <summary>
        /// Ends a profiler section.
        /// </summary>
        /// <param name="name">The name of the section to begin.</param>
        public abstract void EndSection(String name);

        /// <summary>
        /// Begins a profiler snapshot.
        /// </summary>
        public abstract void BeginSnapshot();

        /// <summary>
        /// Ends the current profiler snapshot.
        /// </summary>
        public abstract void EndSnapshot();

        /// <summary>
        /// Enables a profiler section.
        /// </summary>
        /// <param name="name">The name of the section to enable.</param>
        /// <remarks>Only enabled sections will be included in profiler snapshots.</remarks>
        public abstract void EnableSection(String name);

        /// <summary>
        /// Disables a profiler section.
        /// </summary>
        /// <param name="name">The name of the section to enable.</param>
        /// <remarks>Only enabled sections will be included in profiler snapshots.</remarks>
        public abstract void DisableSection(String name);

        /// <summary>
        /// Gets a value indicating whether the profiler is currently taking a snapshot.
        /// </summary>
        public abstract Boolean IsTakingSnapshot
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the profiler is going to take a snapshot of the next frame.
        /// </summary>
        public abstract Boolean IsTakingSnapshotNextFrame
        {
            get;
        }
    }
}
