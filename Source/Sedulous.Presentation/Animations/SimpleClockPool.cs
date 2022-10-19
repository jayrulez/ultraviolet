using System;
using Sedulous.Core;

namespace Sedulous.Presentation.Animations
{
    /// <summary>
    /// Represents a pool of <see cref="SimpleClock"/> instances.
    /// </summary>
    internal partial class SimpleClockPool : SedulousResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleClockPool"/> class.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        private SimpleClockPool(SedulousContext uv)
            : base(uv)
        {
            uv.GetUI().Updating += SimpleClockPool_Updating;
        }

        /// <summary>
        /// Retrieves a clock from the pool.
        /// </summary>
        /// <param name="owner">The object that owns the clock.</param>
        /// <returns>The clock that was retrieved.</returns>
        public UpfPool<SimpleClock>.PooledObject Retrieve(Object owner)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            Initialize();

            var clock = pool.Retrieve(owner);
            var clockValue = clock.Value;

            clockValue.PooledObject = clock;
            return clock;
        }

        /// <summary>
        /// Releases a clock into the pool.
        /// </summary>
        /// <param name="clock">The clock to release into the pool.</param>
        public void Release(UpfPool<SimpleClock>.PooledObject clock)
        {
            Contract.Require(clock, nameof(clock));
            Contract.EnsureNotDisposed(this, Disposed);

            Initialize();

            clock.Value.PooledObject = null;
            pool.Release(clock);
        }

        /// <summary>
        /// Releases a clock into the pool.
        /// </summary>
        /// <param name="clock">The clock to release into the pool.</param>
        public void ReleaseRef(ref UpfPool<SimpleClock>.PooledObject clock)
        {
            Contract.Require(clock, nameof(clock));
            Contract.EnsureNotDisposed(this, Disposed);

            Initialize();

            clock.Value.PooledObject = null;
            pool.Release(clock);

            clock = null;
        }

        /// <summary>
        /// Initializes the pool.
        /// </summary>
        public void Initialize()
        {
            if (pool != null)
                return;

            this.pool = new PoolImpl(Sedulous, 32, 256, () => new SimpleClock(LoopBehavior.None, TimeSpan.Zero));
        }

        /// <summary>
        /// Gets the pool's singleton instance.
        /// </summary>
        public static SimpleClockPool Instance
        {
            get { return instance.Value; }
        }

        /// <summary>
        /// Gets the number of active objects which have been allocated from the pool.
        /// </summary>
        public Int32 Active
        {
            get { return (pool == null) ? 0 : pool.Active; }
        }

        /// <summary>
        /// Gets the number of available objects in the pool.
        /// </summary>
        public Int32 Available
        {
            get { return (pool == null) ? 0 : pool.Available; }
        }

        /// <inheritdoc/>
        protected override void Dispose(Boolean disposing)
        {
            if (disposing && !Sedulous.Disposed)
            {
                Sedulous.GetUI().Updating -= SimpleClockPool_Updating;

                SafeDispose.DisposeRef(ref pool);
            }

            base.Dispose(disposing);
        }
        
        /// <summary>
        /// Updates the active clock instances when the UI subsystem is updated.
        /// </summary>
        private void SimpleClockPool_Updating(ISedulousSubsystem subsystem, SedulousTime time)
        {
            if (pool == null)
                return;

            var upf = Sedulous.GetUI().GetPresentationFoundation();
            upf.PerformanceStats.BeginUpdate();

            pool.Update(time, (value, state) =>
            {
                value.Value.Update((SedulousTime)state);
            });

            upf.PerformanceStats.EndUpdate();
        }
        
        // The pool of available clock objects.
        private UpfPool<SimpleClock> pool;

        // The singleton instance of the clock pool.
        private static SedulousSingleton<SimpleClockPool> instance = 
            new SedulousSingleton<SimpleClockPool>(uv => new SimpleClockPool(uv));
    }
}
