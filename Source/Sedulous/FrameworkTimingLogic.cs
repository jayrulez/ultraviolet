using System;
using System.Diagnostics;
using System.Threading;
using Sedulous.Core;

namespace Sedulous
{
    /// <summary>
    /// Contains core functionality for Sedulous host processes.
    /// </summary>
    public sealed partial class FrameworkTimingLogic : IFrameworkTimingLogic
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkTimingLogic"/> class.
        /// </summary>
        /// <param name="host">The Sedulous host.</param>
        public FrameworkTimingLogic(IFrameworkHost host)
        {
            Contract.Require(host, nameof(host));

            this.host = host;
            this.tickTimer.Start();
        }

        /// <inheritdoc/>
        public void ResetElapsed()
        {
            tickTimer.Restart();
            if (!IsFixedTimeStep)
            {
                forceElapsedTimeToZero = true;
            }
        }

        /// <inheritdoc/>
        public void RunOneTickSuspended()
        {
            var uv = host.FrameworkContext;

            UpdateSystemTimerResolution();

            uv.UpdateSuspended();
            if (InactiveSleepTime.Ticks > 0)
            {
                Thread.Sleep(InactiveSleepTime);
            }
        }

        /// <inheritdoc/>
        public void RunOneTick()
        {
            var context = host.FrameworkContext;

            if (SynchronizationContext.Current is FrameworkSynchronizationContext syncContext)
                syncContext.ProcessWorkItems();

            UpdateSystemTimerResolution();

            if (InactiveSleepTime.Ticks > 0 && !host.IsActive)
                Thread.Sleep(InactiveSleepTime);

            var elapsedTicks = tickTimer.Elapsed.Ticks;
            tickTimer.Restart();

            accumulatedElapsedTime += elapsedTicks;
            if (accumulatedElapsedTime > MaxElapsedTime.Ticks)
                accumulatedElapsedTime = MaxElapsedTime.Ticks;

            var ticksToRun = 0;
            var timeDeltaDraw = default(TimeSpan);
            var timeDeltaUpdate = default(TimeSpan);

            if (IsFixedTimeStep)
            {
                ticksToRun = (Int32)(accumulatedElapsedTime / TargetElapsedTime.Ticks);
                if (ticksToRun > 0)
                {
                    lagFrames += (ticksToRun == 1) ? -1 : Math.Max(0, ticksToRun - 1);

                    if (lagFrames == 0)
                        runningSlowly = false;
                    if (lagFrames > 5)
                        runningSlowly = true;

                    timeDeltaUpdate = TargetElapsedTime;
                    timeDeltaDraw = TimeSpan.FromTicks(ticksToRun * TargetElapsedTime.Ticks);
                    accumulatedElapsedTime -= ticksToRun * TargetElapsedTime.Ticks;
                }
                else
                {
                    var frameDelay = (Int32)(TargetElapsedTime.TotalMilliseconds - tickTimer.Elapsed.TotalMilliseconds);
                    if (frameDelay >= 1 + systemTimerPeriod)
                    {
                        Thread.Sleep(frameDelay - 1);
                    }
                    return;
                }
            }
            else
            {
                ticksToRun = 1;
                if (forceElapsedTimeToZero)
                {
                    timeDeltaUpdate = TimeSpan.Zero;
                    forceElapsedTimeToZero = false;
                }
                else
                {
                    timeDeltaUpdate = TimeSpan.FromTicks(elapsedTicks);
                    timeDeltaDraw = timeDeltaUpdate;
                }
                accumulatedElapsedTime = 0;
                runningSlowly = false;
            }

            if (ticksToRun == 0)
                return;

            context.HandleFrameStart();

            for (var i = 0; i < ticksToRun; i++)
            {
                var updateTime = timeTrackerUpdate.Increment(timeDeltaUpdate, runningSlowly);
                if (!UpdateContext(context, updateTime))
                {
                    return;
                }
            }

            if (!host.IsSuspended)
            {
                var drawTime = timeTrackerDraw.Increment(timeDeltaDraw, runningSlowly);
                using (FrameworkProfiler.Section(FrameworkProfilerSections.Draw))
                {
                    context.Draw(drawTime);
                }
            }

            context.HandleFrameEnd();
        }

        /// <inheritdoc/>
        public void Cleanup()
        {
            if (FrameworkContext.Platform == FrameworkPlatform.Windows)
            {
                if (systemTimerPeriod > 0)
                    Win32Native.timeEndPeriod(systemTimerPeriod);

                systemTimerPeriod = 0;
            }
        }

        /// <summary>
        /// Gets the default value for TargetElapsedTime.
        /// </summary>
        public static TimeSpan DefaultTargetElapsedTime { get; } = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 60);

        /// <summary>
        /// Gets the default value for InactiveSleepTime.
        /// </summary>
        public static TimeSpan DefaultInactiveSleepTime { get; } = TimeSpan.FromMilliseconds(20);

        /// <summary>
        /// Gets the default value for IsFixedTimeStep.
        /// </summary>
        public static Boolean DefaultIsFixedTimeStep { get; } = true;

        /// <inheritdoc/>
        public FrameworkContext FrameworkContext
        {
            get { return host.FrameworkContext; }
        }

        /// <inheritdoc/>
        public TimeSpan TargetElapsedTime { get; set; } = DefaultTargetElapsedTime;

        /// <inheritdoc/>
        public TimeSpan InactiveSleepTime { get; set; } = DefaultInactiveSleepTime;

        /// <inheritdoc/>
        public Boolean IsFixedTimeStep { get; set; } = DefaultIsFixedTimeStep;

        /// <summary>
        /// Updates the specified context.
        /// </summary>
        /// <param name="context">The Sedulous context to update.</param>
        /// <param name="time">Time elapsed since the last update.</param>
        /// <returns><see langword="true"/> if the host should continue processing; otherwise, <see langword="false"/>.</returns>
        private Boolean UpdateContext(FrameworkContext context, FrameworkTime time)
        {
            using (FrameworkProfiler.Section(FrameworkProfilerSections.Update))
            {
                context.Update(time);
            }
            return !context.Disposed;
        }

        /// <summary>
        /// Updates the resolution of the system timer on platforms which require it.
        /// </summary>
        private Boolean UpdateSystemTimerResolution()
        {
            if (FrameworkContext.Platform != FrameworkPlatform.Windows)
            {
                systemTimerPeriod = 1u;
                return false;
            }

            var requiredTimerPeriod = Math.Max(1u, host.IsActive ? (IsFixedTimeStep ? 1u : 15u) : (UInt32)InactiveSleepTime.TotalMilliseconds);
            if (requiredTimerPeriod != systemTimerPeriod)
            {
                if (systemTimerPeriod > 0)
                    Win32Native.timeEndPeriod(systemTimerPeriod);

                var result = Win32Native.timeBeginPeriod(requiredTimerPeriod);
                systemTimerPeriod = requiredTimerPeriod;
                return (result == 0);
            }

            return false;
        }

        // The Sedulous host.
        private readonly IFrameworkHost host;

        // Current tick state.
        private static readonly TimeSpan MaxElapsedTime = TimeSpan.FromMilliseconds(500);
        private readonly FrameworkTimeTracker timeTrackerUpdate = new FrameworkTimeTracker();
        private readonly FrameworkTimeTracker timeTrackerDraw = new FrameworkTimeTracker();
        private readonly Stopwatch tickTimer = new Stopwatch();
        private Int64 accumulatedElapsedTime;
        private Int32 lagFrames;
        private Boolean runningSlowly;
        private Boolean forceElapsedTimeToZero;

        // Current system timer resolution.
        private UInt32 systemTimerPeriod;
    }
}
