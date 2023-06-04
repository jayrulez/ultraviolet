using System;
using Sedulous.Content;
using Sedulous.Core;
using Sedulous.Platform;

namespace Sedulous.UI
{
    /// <summary>
    /// Represents the core implementation of the Sedulous UI subsystem.
    /// </summary>
    public sealed class UISubsystem : FrameworkResource, IUISubsystem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UISubsystem"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="configuration">The Sedulous Framework configuration settings for the current context.</param>
        public UISubsystem(FrameworkContext context, FrameworkConfiguration configuration)
            : base(context)
        {
            screenStacks = new UIScreenStackCollection(context);

            if (ContentManager.IsWatchedContentSupported)
                WatchingViewFilesForChanges = configuration.WatchViewFilesForChanges;
        }

        /// <summary>
        /// Updates the subsystem's state.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Update(FrameworkTime)"/>.</param>
        public void Update(FrameworkTime time)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            foreach (var stack in screenStacks)
            {
                stack.Update(time);
            }

            OnUpdating(time);
        }

        /// <inheritdoc/>
        public UIScreenStack GetScreens()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            var primary = FrameworkContext.GetPlatform().Windows.GetPrimary();
            if (primary == null)
                throw new InvalidOperationException(FrameworkStrings.NoPrimaryWindow);

            return screenStacks[primary];
        }

        /// <inheritdoc/>
        public UIScreenStack GetScreens(IFrameworkWindow window)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return (window == null) ? GetScreens() : screenStacks[window];
        }

        /// <inheritdoc/>
        public Boolean WatchingViewFilesForChanges
        {
            get;
            private set;
        }

        /// <summary>
        /// Occurs when the subsystem is updating its state.
        /// </summary>
        public event FrameworkSubsystemUpdateEventHandler Updating;

        /// <summary>
        /// Releases resources associated with this object.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if the object is being disposed; <see langword="false"/> if the object is being finalized.</param>
        protected override void Dispose(Boolean disposing)
        {
            if (disposing && !Disposed)
            {
                SafeDispose.Dispose(screenStacks);
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Raises the Updating event.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Update(FrameworkTime)"/>.</param>
        private void OnUpdating(FrameworkTime time) =>
            Updating?.Invoke(this, time);

        // The collection of screens associated with each window.
        private readonly UIScreenStackCollection screenStacks;
    }
}
