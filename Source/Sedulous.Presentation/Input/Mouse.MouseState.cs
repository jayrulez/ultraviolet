using Sedulous.Input;

namespace Sedulous.Presentation.Input
{
    partial class Mouse
    {
        /// <summary>
        /// Represents the mouse state of the current Sedulous context.
        /// </summary>
        private class MouseState : FrameworkResource
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MouseState"/> class.
            /// </summary>
            /// <param name="uv">The Sedulous context.</param>
            public MouseState(FrameworkContext uv)
                : base(uv)
            {

            }

            /// <summary>
            /// Gets the primary mouse input device.
            /// </summary>
            public MouseDevice PrimaryDevice
            {
                get { return Sedulous.GetInput().GetMouse(); }
            }
        }
    }
}
