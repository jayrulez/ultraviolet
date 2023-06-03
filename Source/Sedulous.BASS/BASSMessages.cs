namespace Sedulous.BASS
{
    /// <summary>
    /// Contains the implementation's Sedulous engine events.
    /// </summary>
    public static class BASSMessages
    {
        /// <summary>
        /// An event indicating that the current BASS device has changed.
        /// </summary>
        public static readonly FrameworkMessageID BASSDeviceChanged = FrameworkMessageID.Acquire(nameof(BASSDeviceChanged));
    }
}
