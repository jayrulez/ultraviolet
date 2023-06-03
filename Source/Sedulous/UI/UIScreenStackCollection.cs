using System;
using System.Collections.Generic;
using Sedulous.Core;
using Sedulous.Graphics.Graphics2D;
using Sedulous.Platform;

namespace Sedulous.UI
{
    /// <summary>
    /// Represents a collection of screen stacks organized by window.
    /// </summary>
    public sealed class UIScreenStackCollection : FrameworkCollection<UIScreenStack>, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UIScreenStackCollection"/> class.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        public UIScreenStackCollection(FrameworkContext uv)
        {
            Contract.Require(uv, nameof(uv));

            this.uv = uv;

            var windows = uv.GetPlatform().Windows;

            foreach (var window in windows)
            {
                CreateScreenStack(window);
            }

            windows.WindowCreated += WindowInfo_WindowCreated;
            windows.WindowDestroyed += WindowInfo_WindowDestroyed;

            this.spriteBatch = SpriteBatch.Create();
        }

        /// <summary>
        /// Releases resources associated with the object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the screen stack associated with the specified window.
        /// </summary>
        /// <param name="window">The window for which to retrieve a screen stack.</param>
        /// <returns>The <see cref="UIScreenStack"/> associated with the specified window.</returns>
        public UIScreenStack this[IFrameworkWindow window]
        {
            get
            {
                Contract.Require(window, nameof(window));

                UIScreenStack stack;
                if (!screenStacks.TryGetValue(window, out stack))
                    throw new ArgumentException(FrameworkStrings.InvalidWindow);

                return stack;
            }
        }

        /// <summary>
        /// Handles a window's DrawingUI event.
        /// </summary>
        /// <param name="window">The window being drawn.</param>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Draw(FrameworkTime)"/>.</param>
        private void Window_DrawingUI(IFrameworkWindow window, FrameworkTime time)
        {
            screenStacks[window].Draw(time, spriteBatch);
        }

        /// <summary>
        /// Handles the window manager's WindowCreated event.
        /// </summary>
        /// <param name="window">The window that was created.</param>
        private void WindowInfo_WindowCreated(IFrameworkWindow window)
        {
            CreateScreenStack(window);
        }

        /// <summary>
        /// Handles the window manager's WindowDestroyed event.
        /// </summary>
        /// <param name="window">The window that was destroyed.</param>
        private void WindowInfo_WindowDestroyed(IFrameworkWindow window)
        {
            DestroyScreenStack(window);
        }

        /// <summary>
        /// Creates the specified window's screen stack.
        /// </summary>
        /// <param name="window">The window being created.</param>
        private void CreateScreenStack(IFrameworkWindow window)
        {
            var stack = new UIScreenStack(uv, window);

            AddInternal(stack);
            screenStacks.Add(window, stack);

            window.DrawingUI += Window_DrawingUI;
        }

        /// <summary>
        /// Destroys the specified window's screen stack.
        /// </summary>
        /// <param name="window">The window being destroyed.</param>
        private void DestroyScreenStack(IFrameworkWindow window)
        {
            var stack = screenStacks[window];
            stack.Dispose();

            window.DrawingUI -= Window_DrawingUI;

            screenStacks.Remove(window);
            RemoveInternal(stack);
        }

        /// <summary>
        /// Releases resources associated with this object.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if the object is being disposed; <see langword="false"/> if the object is being finalized.</param>
        private void Dispose(Boolean disposing)
        {
            if (disposing && !disposed)
            {
                foreach (var kvp in screenStacks)
                    kvp.Value.Dispose();

                SafeDispose.Dispose(spriteBatch);
            }

            disposed = true;
        }

        // The sprite batch with which screens are drawn.
        private readonly FrameworkContext uv;
        private readonly SpriteBatch spriteBatch;
        private Boolean disposed;

        // The registry of screen stacks for each window.
        private readonly Dictionary<IFrameworkWindow, UIScreenStack> screenStacks = 
            new Dictionary<IFrameworkWindow, UIScreenStack>();
    }
}
