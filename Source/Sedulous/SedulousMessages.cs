
namespace Sedulous
{
    /// <summary>
    /// Represents the standard set of Sedulous Framework events.
    /// </summary>
    public static partial class SedulousMessages
    {
        /// <summary>
        /// An event indicating that the application should exit.
        /// </summary>
        public static readonly SedulousMessageID Quit = SedulousMessageID.Acquire(nameof(Quit));

        /// <summary>
        /// An event indicating that the screen orientation has changed.
        /// </summary>
        public static readonly SedulousMessageID OrientationChanged = SedulousMessageID.Acquire(nameof(OrientationChanged));

        /// <summary>
        /// An event indicating that the application has been created by the operating system.
        /// </summary>
        public static readonly SedulousMessageID ApplicationCreated = SedulousMessageID.Acquire(nameof(ApplicationCreated));

        /// <summary>
        /// An event indicating that the application is being terminated by the operating system.
        /// </summary>
        public static readonly SedulousMessageID ApplicationTerminating = SedulousMessageID.Acquire(nameof(ApplicationTerminating));

        /// <summary>
        /// An event indicating that the application is about to be suspended.
        /// </summary>
        public static readonly SedulousMessageID ApplicationSuspending = SedulousMessageID.Acquire(nameof(ApplicationSuspending));

        /// <summary>
        /// An event indicating that the application was suspended.
        /// </summary>
        public static readonly SedulousMessageID ApplicationSuspended = SedulousMessageID.Acquire(nameof(ApplicationSuspended));

        /// <summary>
        /// An event indicating that the application is about to resume after being suspended.
        /// </summary>
        public static readonly SedulousMessageID ApplicationResuming = SedulousMessageID.Acquire(nameof(ApplicationResuming));

        /// <summary>
        /// An event indicating that the application was resumed after being suspended.
        /// </summary>
        public static readonly SedulousMessageID ApplicationResumed = SedulousMessageID.Acquire(nameof(ApplicationResumed));

        /// <summary>
        /// An event indicating that the operation system is low on memory.
        /// </summary>
        public static readonly SedulousMessageID LowMemory = SedulousMessageID.Acquire(nameof(LowMemory));

        /// <summary>
        /// An event indicating that the software keyboard was shown.
        /// </summary>
        public static readonly SedulousMessageID SoftwareKeyboardShown = SedulousMessageID.Acquire(nameof(SoftwareKeyboardShown));

        /// <summary>
        /// An event indicating that the software keyboard was hidden.
        /// </summary>
        public static readonly SedulousMessageID SoftwareKeyboardHidden = SedulousMessageID.Acquire(nameof(SoftwareKeyboardHidden));

        /// <summary>
        /// An event indicating that the text input region has been changed.
        /// </summary>
        public static readonly SedulousMessageID TextInputRegionChanged = SedulousMessageID.Acquire(nameof(TextInputRegionChanged));

        /// <summary>
        /// An event indicating that the density settings for a particular display were changed.
        /// </summary>
        public static readonly SedulousMessageID DisplayDensityChanged = SedulousMessageID.Acquire(nameof(DisplayDensityChanged));

        /// <summary>
        /// An event indicating that a window was moved to a display with a different density.
        /// </summary>
        public static readonly SedulousMessageID WindowDensityChanged = SedulousMessageID.Acquire(nameof(WindowDensityChanged));

        /// <summary>
        /// An event indicating that the file source for content assets has changed.
        /// </summary>
        public static readonly SedulousMessageID FileSourceChanged = SedulousMessageID.Acquire(nameof(FileSourceChanged));
    }
}
