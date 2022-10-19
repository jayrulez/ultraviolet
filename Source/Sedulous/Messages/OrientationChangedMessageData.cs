using Sedulous.Core.Messages;
using Sedulous.Platform;

namespace Sedulous.Messages
{
    /// <summary>
    /// Represents the message data for an Orientation Changed message.
    /// </summary>
    public sealed class OrientationChangedMessageData : MessageData
    {
        /// <summary>
        /// Gets or sets the display which changed orientation.
        /// </summary>
        public ISedulousDisplay Display
        {
            get;
            set;
        }
    }
}
