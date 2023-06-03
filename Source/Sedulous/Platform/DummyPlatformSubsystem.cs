using System;
using Sedulous.Core;
using Sedulous.Platform;

namespace Sedulous
{
    /// <summary>
    /// Represents a dummy implementation of <see cref="IPlatformSubsystem"/>.
    /// </summary>
    public class DummyPlatformSubsystem : FrameworkResource, IPlatformSubsystem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DummyPlatformSubsystem"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        public DummyPlatformSubsystem(FrameworkContext context)
            : base(context)
        {
            this.clipboard = new DummyClipboardService();
            this.windows = new DummyFrameworkWindowInfo();
            this.displays = new DummyFrameworkDisplayInfo();
        }

        /// <inheritdoc/>
        public void Update(FrameworkTime time)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            Updating?.Invoke(this, time);
        }

        /// <inheritdoc/>
        public void InitializePrimaryWindow(FrameworkConfiguration configuration)
        { }

        /// <inheritdoc/>
        public void ShowMessageBox(MessageBoxType type, String title, String message, IFrameworkWindow parent = null)
        {
            Contract.EnsureNotDisposed(this, Disposed);
        }

        /// <inheritdoc/>
        public Boolean IsPrimaryWindowInitialized
        {
            get { return true; }
        }

        /// <inheritdoc/>
        public Boolean IsCursorVisible
        {
            get { return false; }
            set { }
        }

        /// <inheritdoc/>
        public Cursor Cursor
        {
            get { return null; }
            set { }
        }

        /// <inheritdoc/>
        public ClipboardService Clipboard => clipboard;

        /// <inheritdoc/>
        public IFrameworkWindowInfo Windows => windows;

        /// <inheritdoc/>
        public IFrameworkDisplayInfo Displays => displays;

        /// <inheritdoc/>
        public event SedulousSubsystemUpdateEventHandler Updating;

        // Property values.
        private ClipboardService clipboard;
        private IFrameworkWindowInfo windows;
        private IFrameworkDisplayInfo displays;
    }
}
