using System;
using Sedulous.Platform;

namespace Sedulous
{
    /// <summary>
    /// Represents the Sedulous Framework's platform interop subsystem.
    /// </summary>
    public interface IPlatformSubsystem : IFrameworkSubsystem
    {
        /// <summary>
        /// Instructs the Platform subsystem to initialize the application's primary window, if it hasn't already been initialized.
        /// This method should be called by the Graphics subsystem after it has performed any preliminary configuration for the 
        /// underlying rendering API which must be done prior to window creation.
        /// </summary>
        /// <param name="configuration">The configuration settings for the Framework context.</param>
        void InitializePrimaryWindow(FrameworkConfiguration configuration);

        /// <summary>
        /// Displays a platform-specific message box with the specified text.
        /// </summary>
        /// <param name="type">A <see cref="MessageBoxType"/> value specifying the type of message box to display.</param>
        /// <param name="title">The message box's title text.</param>
        /// <param name="message">The message box's message text.</param>
        /// <param name="parent">The message box's parent window, or <see langword="null"/> to use the primary window.</param>
        void ShowMessageBox(MessageBoxType type, String title, String message, IFrameworkWindow parent = null);

        /// <summary>
        /// Gets a value indicating whether the application's primary window has been initialized.
        /// </summary>
        Boolean IsPrimaryWindowInitialized { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the mouse cursor is visible.
        /// </summary>
        Boolean IsCursorVisible { get; set; }

        /// <summary>
        /// Gets or sets the current cursor.
        /// </summary>
        /// <remarks>Setting this property to <see langword="null"/> will restore the default cursor.</remarks>
        Cursor Cursor { get; set; }

        /// <summary>
        /// Gets the system clipboard manager.
        /// </summary>
        ClipboardService Clipboard { get; }

        /// <summary>
        /// Gets the window information manager.
        /// </summary>
        IFrameworkWindowInfo Windows { get; }

        /// <summary>
        /// Gets the display information manager.
        /// </summary>
        IFrameworkDisplayInfo Displays { get; }
    }
}
