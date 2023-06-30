using System;

namespace Sedulous.Graphics.Graphics2D
{
    /// <summary>
    /// Contains methods for coordinating the operations of multiple sprite batches.
    /// </summary>
    internal class SpriteBatchCoordinator : FrameworkResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteBatchCoordinator"/> class.
        /// </summary>
        /// <param name="context">The Framework context.</param>
        private SpriteBatchCoordinator(FrameworkContext context)
            : base(context)
        { }

        /// <summary>
        /// Demands the right to operate in immediate mode.  If the right is denied, an InvalidOperationException is thrown.
        /// </summary>
        public void DemandImmediate()
        {
            if (immediate > 0 || deferred > 0)
                throw new InvalidOperationException(FrameworkStrings.SpriteBatchNestingError);

            immediate++;
        }

        /// <summary>
        /// Demands the right to operate in deferred mode.  If the right is denied, an InvalidOperationException is thrown.
        /// </summary>
        public void DemandDeferred()
        {
            if (immediate > 0)
                throw new InvalidOperationException(FrameworkStrings.SpriteBatchNestingError);

            deferred++;
        }

        /// <summary>
        /// Relinquishes the right to operate in immediate mode.
        /// </summary>
        public void RelinquishImmediate()
        {
            if (--immediate < 0)
                throw new InvalidOperationException(FrameworkStrings.SpriteBatchDemandImbalance);
        }

        /// <summary>
        /// Relinquishes the right to operate in deferred mode.
        /// </summary>
        public void RelinquishDeferred()
        {
            if (--deferred < 0)
                throw new InvalidOperationException(FrameworkStrings.SpriteBatchDemandImbalance);
        }

        /// <summary>
        /// Gets the singleton instance of the <see cref="SpriteBatchCoordinator"/> class.
        /// </summary>
        public static SpriteBatchCoordinator Instance => instance.Value;

        // The coordinator's singleton instance.
        private static FrameworkSingleton<SpriteBatchCoordinator> instance =
            new FrameworkSingleton<SpriteBatchCoordinator>(FrameworkSingletonFlags.DisabledInServiceMode, uv => new SpriteBatchCoordinator(uv));

        // Track how many batches are operating in each mode.
        private Int32 immediate;
        private Int32 deferred;
    }
}
