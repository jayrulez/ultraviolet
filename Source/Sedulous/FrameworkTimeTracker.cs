using System;

namespace Sedulous
{
    /// <summary>
    /// Contains methods for tracking the amount of time that has passed since an Framework context was created.
    /// </summary>
    public sealed class FrameworkTimeTracker
    {
        /// <summary>
        /// Resets the time.
        /// </summary>
        /// <returns>The Sedulous time value after the reset has been applied.</returns>
        public FrameworkTime Reset()
        {
            time.ElapsedTime     = TimeSpan.Zero;
            time.TotalTime       = TimeSpan.Zero;
            time.IsRunningSlowly = false;
            return time;
        }

        /// <summary>
        /// Increments the time.
        /// </summary>
        /// <param name="ts">The amount by which to increment the time.</param>
        /// <param name="isRunningSlowly">A value indicating whether the application's main loop is taking longer than its target time.</param>
        /// <returns>The Sedulous time value after the increment has been applied.</returns>
        public FrameworkTime Increment(TimeSpan ts, Boolean isRunningSlowly)
        {
            time.ElapsedTime = ts;
            time.TotalTime = time.TotalTime.Add(ts);
            time.IsRunningSlowly = isRunningSlowly;
            return time;
        }

        /// <summary>
        /// Gets the current Sedulous time value.
        /// </summary>
        public FrameworkTime Time
        {
            get { return time; }
        }

        // The Sedulous time value for the current context.
        private readonly FrameworkTime time = new FrameworkTime();
    }
}
