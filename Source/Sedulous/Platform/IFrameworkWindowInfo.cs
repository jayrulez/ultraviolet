using System;
using System.Collections.Generic;

namespace Sedulous.Platform
{
    /// <summary>
    /// Represents an event that occurs when Sedulous updates its window information.
    /// </summary>
    /// <param name="window">The window that was updated.</param>
    public delegate void SedulousWindowInfoEventHandler(IFrameworkWindow window);

    /// <summary>
    /// Provides access to information concerning the context's attached windows.
    /// </summary>
    public interface IFrameworkWindowInfo : IEnumerable<IFrameworkWindow>
    {
        /// <summary>
        /// Designates the specified window as the primary window.
        /// </summary>
        /// <param name="window">The window to designate as the primary window, or <see langword="null"/> to clear the primary window.</param>
        void DesignatePrimary(IFrameworkWindow window);

        /// <summary>
        /// Gets the window with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the window to retrieve.</param>
        /// <returns>The window with the specified identifier, or <see langword="null"/> if no such window exists.</returns>
        IFrameworkWindow GetByID(Int32 id);

        /// <summary>
        /// Gets the context's primary window.
        /// </summary>
        /// <returns>The context's primary window, or <see langword="null"/> if the context is headless.</returns>
        IFrameworkWindow GetPrimary();

        /// <summary>
        /// Gets the context's current window.
        /// </summary>
        /// <returns>The context's current window.</returns>
        IFrameworkWindow GetCurrent();
        
        /// <summary>
        /// Creates a new window and attaches it to the current context.
        /// </summary>
        /// <param name="caption">The window's caption text.</param>
        /// <param name="x">The x-coordinate at which to position the window's top-left corner.</param>
        /// <param name="y">The y-coordinate at which to position the window's top-left corner.</param>
        /// <param name="width">The width of the window's client area in pixels.</param>
        /// <param name="height">The height of the window's client area in pixels.</param>
        /// <param name="flags">A set of <see cref="WindowFlags"/> values indicating how to create the window.</param>
        /// <returns>The Sedulous window that was created.</returns>
        IFrameworkWindow Create(String caption, Int32 x, Int32 y, Int32 width, Int32 height, WindowFlags flags = WindowFlags.None);

        /// <summary>
        /// Creates a new Sedulous window from the specified native window and attaches it to the current context.
        /// </summary>
        /// <param name="ptr">A pointer that represents the native window to attach to the context.</param>
        /// <returns>The Sedulous window that was created.</returns>
        IFrameworkWindow CreateFromNativePointer(IntPtr ptr);

        /// <summary>
        /// Destroys the specified window.
        /// </summary>
        /// <remarks>Windows which were created from native pointers are disassociated from the current context,
        /// but are not actually destroyed. To destroy such windows, use the native framework which created them.</remarks>
        /// <param name="window">The Sedulous window to destroy.</param>
        /// <returns><see langword="true"/> if the window was destroyed; <see langword="false"/> if the window was closed.</returns>
        Boolean Destroy(IFrameworkWindow window);

        /// <summary>
        /// Destroys the window with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the window to destroy.</param>
        /// <returns><see langword="true"/> if the window was destroyed; <see langword="false"/> if the window was closed.</returns>
        Boolean DestroyByID(Int32 id);

        /// <summary>
        /// Gets the collection's enumerator.
        /// </summary>
        /// <returns>The collection's enumerator.</returns>
        new List<IFrameworkWindow>.Enumerator GetEnumerator();

        /// <summary>
        /// Occurs after a window has been created.
        /// </summary>
        event SedulousWindowInfoEventHandler WindowCreated;

        /// <summary>
        /// Occurs when a window is about to be destroyed.
        /// </summary>
        event SedulousWindowInfoEventHandler WindowDestroyed;

        /// <summary>
        /// Occurs when the primary window is about to change.
        /// </summary>
        event SedulousWindowInfoEventHandler PrimaryWindowChanging;

        /// <summary>
        /// Occurs when the primary window changes.
        /// </summary>
        event SedulousWindowInfoEventHandler PrimaryWindowChanged;

        /// <summary>
        /// Occurs when the current window is about to change.
        /// </summary>
        event SedulousWindowInfoEventHandler CurrentWindowChanging;

        /// <summary>
        /// Occurs when the current window changes.
        /// </summary>
        event SedulousWindowInfoEventHandler CurrentWindowChanged;
    }
}
