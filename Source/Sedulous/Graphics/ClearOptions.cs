using System;

namespace Sedulous.Abstractions
{
    /// <summary>
    /// Represents the set of buffers which will be cleared by the <see cref="IGraphicsSubsystem.Clear(ClearOptions, Color, double, int)"/> method.
    /// </summary>
    [Flags]
    public enum ClearOptions
    {
        /// <summary>
        /// Clears the render target.
        /// </summary>
        Target = 1,

        /// <summary>
        /// Clears the depth buffer.
        /// </summary>
        DepthBuffer = 2,

        /// <summary>
        /// Clears the stencil buffer.
        /// </summary>
        Stencil = 4,
    }
}
