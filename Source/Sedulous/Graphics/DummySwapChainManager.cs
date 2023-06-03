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
        public DummySwapChainManager(FrameworkContext uv) : base(uv) { }

        /// <inheritdoc/>
        public override void DrawAndSwap(FrameworkTime time,
            Action<FrameworkContext, FrameworkTime, IFrameworkWindow> onWindowDrawing,
            Action<FrameworkContext, FrameworkTime, IFrameworkWindow> onWindowDrawn)
        { }
    }
}
