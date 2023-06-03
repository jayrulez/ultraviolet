
namespace Sedulous
{
    /// <summary>
    /// Represents the standard set of Sedulous Framework events.
    /// </summary>
    public static partial class FrameworkMessages
    {
        /// <summary>
        /// An event indicating that the application should exit.
        /// </summary>
        public static readonly FrameworkMessageID Quit = FrameworkMessageID.Acquire(nameof(Quit));

        /// <summary>
        /// An event indicating that the screen orientation has changed.
        /// </summary>
        public static readonly FrameworkMessageID OrientationChanged = FrameworkMessageID.Acquire(nameof(OrientationChanged));

        /// <summary>
        /// An event indicating that the application has been created by the operating system.
        /// </summary>
        public static readonly FrameworkMessageID ApplicationCreated = FrameworkMessageID.Acquire(nameof(ApplicationCreated));

        /// <summary>
        /// An event indicating that the application is being terminated by the operating system.
        /// </summary>
        public static readonly FrameworkMessageID ApplicationTerminating = FrameworkMessageID.Acquire(nameof(ApplicationTerminating));

        /// <summary>
        /// An event indicating that the application is about to be suspended.
        /// </summary>
        public static readonly FrameworkMessageID ApplicationSuspending = FrameworkMessageID.Acquire(nameof(ApplicationSuspending));

        /// <summary>
        /// An event indicating that the application was suspended.
        /// </summary>
        public static readonly FrameworkMessageID ApplicationSuspended = FrameworkMessageID.Acquire(nameof(ApplicationSuspended));

        /// <summary>
        /// An event indicating that the application is about to resume after being suspended.
        /// </summary>
        public static readonly FrameworkMessageID ApplicationResuming = FrameworkMessageID.Acquire(nameof(ApplicationResuming));

        /// <summary>
        /// An event indicating that the application was resumed after being suspended.
        /// </summary>
        public static readonly FrameworkMessageID ApplicationResumed = FrameworkMessageID.Acquire(nameof(ApplicationResumed));

        /// <summary>
        /// An event indicating that the operation system is low on memory.
        /// </summary>
        public static readonly FrameworkMessageID LowMemory = FrameworkMessageID.Acquire(nameof(LowMemory));

        /// <summary>
        /// An event indicating that the software keyboard was shown.
        /// </summary>
        public static readonly FrameworkMessageID SoftwareKeyboardShown = FrameworkMessageID.Acquire(nameof(SoftwareKeyboardShown));

        /// <summary>
        /// An event indicating that the software keyboard was hidden.
        /// </summary>
        public static readonly FrameworkMessageID SoftwareKeyboardHidden = FrameworkMessageID.Acquire(nameof(SoftwareKeyboardHidden));

        /// <summary>
        /// An event indicating that the text input region has been changed.
        /// </summary>
        public static readonly FrameworkMessageID TextInputRegionChanged = FrameworkMessageID.Acquire(nameof(TextInputRegionChanged));

        /// <summary>
        /// An event indicating that the density settings for a particular display were changed.
        /// </summary>
        public static readonly FrameworkMessageID DisplayDensityChanged = FrameworkMessageID.Acquire(nameof(DisplayDensityChanged));

        /// <summary>
        /// An event indicating that a window was moved to a display with a different density.
        /// </summary>
        public static readonly FrameworkMessageID WindowDensityChanged = FrameworkMessageID.Acquire(nameof(WindowDensityChanged));

        /// <summary>
        /// An event indicating that the file source for content assets has changed.
        /// </summary>
        public static readonly FrameworkMessageID FileSourceChanged = FrameworkMessageID.Acquire(nameof(FileSourceChanged));
    }
}
