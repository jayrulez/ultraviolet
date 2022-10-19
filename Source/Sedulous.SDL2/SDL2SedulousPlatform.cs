using System;
using System.Linq;
using Sedulous.Core;
using Sedulous.Platform;
using Sedulous.SDL2.Platform;
using static Sedulous.SDL2.Native.SDLNative;

namespace Sedulous.SDL2
{
    /// <summary>
    /// Represents the SDL2 implementation of the <see cref="ISedulousPlatform"/> interface.
    /// </summary>
    public sealed class SDL2SedulousPlatform : SedulousResource, ISedulousPlatform
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SDL2SedulousPlatform"/> class.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        /// <param name="configuration">The Sedulous Framework's configuration settings.</param>
        public SDL2SedulousPlatform(SedulousContext uv, SedulousConfiguration configuration)
            : base(uv)
        {
            this.clipboard = ClipboardService.Create();
            this.messageBoxService = MessageBoxService.Create();
            this.windows = new SDL2SedulousWindowInfoOpenGL(uv, configuration);
            this.displays = new SDL2SedulousDisplayInfo(uv);
            this.isCursorVisible = SDL_ShowCursor(SDL_QUERY) != 0;
        }

        /// <inheritdoc/>
        public void Update(SedulousTime time)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            this.displays.Update(time);
            this.windows.Update(time);

            OnUpdating(time);
        }

        /// <inheritdoc/>
        public void InitializePrimaryWindow(SedulousConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (IsPrimaryWindowInitialized)
                throw new InvalidOperationException();

            this.windows.InitializePrimaryWindow(Sedulous, configuration);
            this.IsPrimaryWindowInitialized = true;
        }

        /// <inheritdoc/>
        public void ShowMessageBox(MessageBoxType type, String title, String message, ISedulousWindow parent = null)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            if (parent == null)
                parent = Windows.GetPrimary();

            var window = (parent == null) ? IntPtr.Zero : (IntPtr)((SDL2SedulousWindow)parent);
            messageBoxService.ShowMessageBox(type, title, message, window);
        }

        /// <inheritdoc/>
        public Boolean IsPrimaryWindowInitialized
        {
            get;
            private set;
        }

        /// <inheritdoc/>
        public Boolean IsCursorVisible
        {
            get { return isCursorVisible; }
            set
            {
                if (value != isCursorVisible)
                {
                    var result = SDL_ShowCursor(value ? SDL_ENABLE : SDL_DISABLE);
                    if (result < 0)
                        throw new SDL2Exception();

                    isCursorVisible = SDL_ShowCursor(SDL_QUERY) != 0;
                }
            }
        }

        /// <inheritdoc/>
        public Cursor Cursor
        {
            get => cursor;
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                cursor = value;

                unsafe
                {
                    var sdlCursor = (value is SDL2Cursor sdl2cursor) ? sdl2cursor.Native : null;
                    if (sdlCursor == null)
                        sdlCursor = SDL_GetDefaultCursor();

                    SDL_SetCursor(sdlCursor);
                }
            }
        }

        /// <inheritdoc/>
        public ClipboardService Clipboard => clipboard;

        /// <inheritdoc/>
        public ISedulousWindowInfo Windows => windows;

        /// <inheritdoc/>
        public ISedulousDisplayInfo Displays => displays;

        /// <inheritdoc/>
        public event SedulousSubsystemUpdateEventHandler Updating;

        /// <inheritdoc/>
        protected override void Dispose(Boolean disposing)
        {
            if (disposing && !Disposed)
            {
                windows.DesignateCurrent(null, IntPtr.Zero);
                foreach (SDL2SedulousWindow window in windows.ToList())
                {
                    windows.Destroy(window);
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Raises the Updating event.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to Update.</param>
        private void OnUpdating(SedulousTime time) =>
            Updating?.Invoke(this, time);

        // Property values.
        private Boolean isCursorVisible = true;
        private Cursor cursor;
        private readonly ClipboardService clipboard;
        private readonly MessageBoxService messageBoxService;
        private readonly SDL2SedulousWindowInfoOpenGL windows;
        private readonly SDL2SedulousDisplayInfo displays;
    }
}
