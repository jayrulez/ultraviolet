using System;
using Sedulous.Platform;

namespace Sedulous.Graphics
{
    /// <summary>
    /// Represents a dummy implementation of <see cref="GraphicsCapabilities"/>.
    /// </summary>
    public sealed class DummySwapChainManager : SwapChainManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DummySwapChainManager"/> class.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        public DummySwapChainManager(SedulousContext uv) : base(uv) { }

        /// <inheritdoc/>
        public override void DrawAndSwap(SedulousTime time,
            Action<SedulousContext, SedulousTime, ISedulousWindow> onWindowDrawing,
            Action<SedulousContext, SedulousTime, ISedulousWindow> onWindowDrawn)
        { }
    }
}
