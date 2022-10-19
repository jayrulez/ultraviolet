using Sedulous.Core.Messages;
using Sedulous.Platform;

namespace Sedulous.Messages
{
    /// <summary>
    /// Represents the message data for a Window Density Changed message.
    /// </summary>
    public sealed class WindowDensityChangedMessageData : MessageData
    {
        /// <summary>
        /// Gets or sets the window which changed density.
        /// </summary>
        public ISedulousWindow Window
        {
            get;
            set;
        }
    }
}
