namespace Sedulous.BASS
{
    /// <summary>
    /// Contains the implementation's Sedulous engine events.
    /// </summary>
    public static class BASSSedulousMessages
    {
        /// <summary>
        /// An event indicating that the current BASS device has changed.
        /// </summary>
        public static readonly SedulousMessageID BASSDeviceChanged = SedulousMessageID.Acquire(nameof(BASSDeviceChanged));
    }
}
