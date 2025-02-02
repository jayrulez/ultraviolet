﻿namespace Sedulous
{
    /// <summary>
    /// Represents the types of message box which can be displayed by the <see cref="IPlatformSubsystem.ShowMessageBox"/> method.
    /// </summary>
    public enum MessageBoxType
    {
        /// <summary>
        /// An informational dialog.
        /// </summary>
        Information,

        /// <summary>
        /// A warning dialog.
        /// </summary>
        Warning,

        /// <summary>
        /// An error dialog.
        /// </summary>
        Error,
    }
}
