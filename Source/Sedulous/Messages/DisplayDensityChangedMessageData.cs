using Sedulous.Core.Messages;
using Sedulous.Platform;

namespace Sedulous.Messages
{
    /// <summary>
    /// Represents the message data for a Display Density Changed message.
    /// </summary>
    public sealed class DisplayDensityChangedMessageData : MessageData
    {
        /// <summary>
        /// Gets or sets the display which changed density.
        /// </summary>
        public ISedulousDisplay Display
        {
            get;
            set;
        }
    }
}
