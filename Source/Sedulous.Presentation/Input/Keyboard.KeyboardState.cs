using Sedulous.Input;

namespace Sedulous.Presentation.Input
{
    partial class Keyboard
    {
        /// <summary>
        /// Represents the keyboard state of the current Sedulous context.
        /// </summary>
        private class KeyboardState : SedulousResource
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="KeyboardState"/> class.
            /// </summary>
            /// <param name="uv">The Sedulous context.</param>
            public KeyboardState(SedulousContext uv)
                : base(uv)
            {

            }

            /// <summary>
            /// Gets the primary keyboard input device.
            /// </summary>
            public KeyboardDevice PrimaryDevice
            {
                get { return Sedulous.GetInput().GetKeyboard(); }
            }
        }
    }
}
