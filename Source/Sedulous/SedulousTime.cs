using System;

namespace Sedulous
{
    /// <summary>
    /// Represents the application's timing state.
    /// </summary>
    public sealed class SedulousTime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SedulousTime"/> class.
        /// </summary>
        public SedulousTime()
        {
            this.ElapsedTime = TimeSpan.Zero;
            this.TotalTime   = TimeSpan.Zero;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SedulousTime"/> class with the specified elapsed and total times.
        /// </summary>
        /// <param name="elapsedTime">The time that has elapsed since the last update.</param>
        /// <param name="totalTime">The total time that has elapsed since the Sedulous context was created.</param>
        public SedulousTime(TimeSpan elapsedTime, TimeSpan totalTime)
        {
            this.ElapsedTime = elapsedTime;
            this.TotalTime   = totalTime;
        }

        /// <summary>
        /// The time that has elapsed since the last update.
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get;
            internal set;
        }

        /// <summary>
        /// The total time that has elapsed since the Sedulous context was created.
        /// </summary>
        public TimeSpan TotalTime
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a value indicating whether the application's main loop is taking longer than its target time.
        /// </summary>
        public Boolean IsRunningSlowly
        {
            get;
            internal set;
        }
    }
}
