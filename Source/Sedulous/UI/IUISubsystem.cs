﻿using System;
using Sedulous.Platform;
using Sedulous.UI;

namespace Sedulous
{
    /// <summary>
    /// Represents the Sedulous Framework's UI subsystem.
    /// </summary>
    public interface IUISubsystem : IFrameworkSubsystem
    {
        /// <summary>
        /// Gets the screen stack associated with the primary window.
        /// </summary>
        /// <returns>The <see cref="UIScreenStack"/> associated with the primary window.</returns>
        UIScreenStack GetScreens();

        /// <summary>
        /// Gets the screen stack associated with the specified window.
        /// </summary>
        /// <param name="window">The window for which to retrieve a screen stack, 
        /// or <see langword="null"/> to retrieve the screen stack for the primary window.</param>
        /// <returns>The <see cref="UIScreenStack"/> associated with the specified window.</returns>
        UIScreenStack GetScreens(IFrameworkWindow window);
        
        /// <summary>
        /// Gets a value indicating whether the UI subsystem is watching loaded view files for changes.
        /// </summary>
        Boolean WatchingViewFilesForChanges
        {
            get;
        }
    }
}
