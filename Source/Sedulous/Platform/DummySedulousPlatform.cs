using System;
using Sedulous.Core;
using Sedulous.Platform;

namespace Sedulous
{
    /// <summary>
    /// Represents a dummy implementation of <see cref="ISedulousPlatform"/>.
    /// </summary>
    public class DummySedulousPlatform : SedulousResource, ISedulousPlatform
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DummySedulousPlatform"/> class.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        public DummySedulousPlatform(SedulousContext uv)
            : base(uv)
        {
            this.clipboard = new DummyClipboardService();
            this.windows = new DummySedulousWindowInfo();
            this.displays = new DummySedulousDisplayInfo();
        }

        /// <inheritdoc/>
        public void Update(SedulousTime time)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            Updating?.Invoke(this, time);
        }

        /// <inheritdoc/>
        public void InitializePrimaryWindow(SedulousConfiguration configuration)
        { }

        /// <inheritdoc/>
        public void ShowMessageBox(MessageBoxType type, String title, String message, ISedulousWindow parent = null)
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
        public ISedulousWindowInfo Windows => windows;

        /// <inheritdoc/>
        public ISedulousDisplayInfo Displays => displays;

        /// <inheritdoc/>
        public event SedulousSubsystemUpdateEventHandler Updating;

        // Property values.
        private ClipboardService clipboard;
        private ISedulousWindowInfo windows;
        private ISedulousDisplayInfo displays;
    }
}
