
namespace Sedulous.SDL2
{
    /// <summary>
    /// Contains the implementation's Sedulous engine events.
    /// </summary>
    public static class SDL2SedulousMessages
    {
        /// <summary>
        /// An event indicating that an SDL event was raised.
        /// </summary>
        public static readonly SedulousMessageID SDLEvent = SedulousMessageID.Acquire("SDLEvent");
    }
}
