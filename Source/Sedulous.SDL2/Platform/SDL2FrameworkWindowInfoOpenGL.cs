using System;
using Sedulous.Core;
using Sedulous.Platform;
using static Sedulous.SDL2.Native.SDL_GLattr;
using static Sedulous.SDL2.Native.SDLNative;
using Sedulous.OpenGL;

namespace Sedulous.SDL2.Platform
{
    /// <summary>
    /// Represents the SDL2 implementation of the <see cref="IFrameworkWindowInfo"/> interface when
    /// using the OpenGL rendering API.
    /// </summary>
    public sealed class SDL2FrameworkWindowInfoOpenGL : SDL2FrameworkWindowInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SDL2FrameworkWindowInfoOpenGL"/> class.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        /// <param name="configuration">The Sedulous configuration settings for the current context.</param>
        public SDL2FrameworkWindowInfoOpenGL(FrameworkContext uv, FrameworkConfiguration configuration)
            : base(uv, configuration)
        {

        }
        
        /// <summary>
        /// Makes the specified window the current window.
        /// </summary>
        /// <param name="window">The window to make current.</param>
        /// <param name="context">The OpenGL context.</param>
        public void DesignateCurrent(IFrameworkWindow window, IntPtr context)
        {
            if (Current != window)
            {
                OnCurrentWindowChanging();

                UpdateIsCurrentWindow(GetCurrent(), false);
                Current = window;
                UpdateIsCurrentWindow(GetCurrent(), true);

                if (window != null && (window != glwin || window.SynchronizeWithVerticalRetrace != VSync))
                    BindOpenGLContextToWindow(window, context);

                OnCurrentWindowChanged();
            }

            if (Windows.Count == 0 || (window == null && context == IntPtr.Zero))
                BindOpenGLContextToWindow(null, context);
        }

        /// <inheritdoc/>
        protected override Boolean InitializeRenderingAPI(FrameworkGraphicsConfiguration graphicsConfig, Int32 attempt)
        {
            var glGraphicsConfig = graphicsConfig as OpenGLGraphicsConfiguration;
            if (glGraphicsConfig == null)
                throw new InvalidOperationException(FrameworkStrings.MissingGraphicsConfiguration);

            if (SDL_GL_SetAttribute(SDL_GL_FRAMEBUFFER_SRGB_CAPABLE, glGraphicsConfig.SrgbBuffersEnabled ? 1 : 0) < 0)
                throw new SDL2Exception();

            if (SDL_GL_SetAttribute(SDL_GL_MULTISAMPLEBUFFERS, glGraphicsConfig.MultiSampleBuffers) < 0)
                throw new SDL2Exception();

            if (SDL_GL_SetAttribute(SDL_GL_MULTISAMPLESAMPLES, glGraphicsConfig.MultiSampleSamples) < 0)
                throw new SDL2Exception();

            switch (attempt)
            {
                case 0:
                    // Attempt #0: Try with specified configurations.
                    return true;

                case 1:
                    // Attempt #1: Try turning off sRGB.  
                    if (glGraphicsConfig.SrgbBuffersEnabled)
                    {
                        if (SDL_GL_SetAttribute(SDL_GL_FRAMEBUFFER_SRGB_CAPABLE, 0) < 0)
                            throw new SDL2Exception();

                        return true;
                    }
                    else goto case 2;

                case 2:
                    // Attempt #2: Try turning off multisampling.
                    if (glGraphicsConfig.MultiSampleBuffers > 0 || glGraphicsConfig.MultiSampleSamples > 0)
                    {
                        if (SDL_GL_SetAttribute(SDL_GL_MULTISAMPLEBUFFERS, 0) < 0)
                            throw new SDL2Exception();

                        if (SDL_GL_SetAttribute(SDL_GL_MULTISAMPLESAMPLES, 0) < 0)
                            throw new SDL2Exception();

                        return true;
                    }
                    else goto case 3;

                case 3:
                    // Attempt #3: Try turning off sRGB and multisampling.
                    if (glGraphicsConfig.SrgbBuffersEnabled)
                    {
                        if (SDL_GL_SetAttribute(SDL_GL_FRAMEBUFFER_SRGB_CAPABLE, 0) < 0)
                            throw new SDL2Exception();

                        if (SDL_GL_SetAttribute(SDL_GL_MULTISAMPLEBUFFERS, 0) < 0)
                            throw new SDL2Exception();

                        if (SDL_GL_SetAttribute(SDL_GL_MULTISAMPLESAMPLES, 0) < 0)
                            throw new SDL2Exception();

                        return true;
                    }
                    else goto default;

                default:
                    return false;
            }
        }

        /// <inheritdoc/>
        protected override void OnWindowCleanup(IFrameworkWindow window)
        {
            if (window == glwin && glcontext != IntPtr.Zero)
                BindOpenGLContextToWindow(null, glcontext);

            base.OnWindowCleanup(window);
        }

        /// <inheritdoc/>
        protected override void OnWindowCreated(IFrameworkWindow window)
        {
            base.OnWindowCreated(window);
        }

        /// <inheritdoc/>
        protected override void OnWindowDestroyed(IFrameworkWindow window)
        {
            base.OnWindowDestroyed(window);
        }

        /// <inheritdoc/>
        protected override SDL2PlatformRenderingAPI RenderingApi { get; } = SDL2PlatformRenderingAPI.OpenGL;

        /// <summary>
        /// Updates the value of the <see cref="SDL2FrameworkWindow.IsCurrentWindow"/> property for the specified window.
        /// </summary>
        private void UpdateIsCurrentWindow(IFrameworkWindow window, Boolean value)
        {
            var win = window as SDL2FrameworkWindow;
            if (win == null)
                return;

            win.IsCurrentWindow = value;
        }

        /// <summary>
        /// Binds the OpenGL context to the specified window.
        /// </summary>
        private void BindOpenGLContextToWindow(IFrameworkWindow window, IntPtr context)
        {
            var shuttingDown = (window == null && context == IntPtr.Zero);

            if (context == IntPtr.Zero)
            {
                if (glcontext == IntPtr.Zero)
                {
                    return;
                }
                context = glcontext;
            }

            var win = (SDL2FrameworkWindow)(window ?? GetMaster());
            var winptr = (IntPtr)win;
            if (SDL_GL_MakeCurrent(winptr, context) < 0)
                throw new SDL2Exception();

            if (SDL_GL_SetSwapInterval(win.SynchronizeWithVerticalRetrace ? 1 : 0) < 0 && Sedulous.Platform != FrameworkPlatform.iOS)
            {
                if (!shuttingDown)
                    throw new SDL2Exception();
            }

            VSync = win.SynchronizeWithVerticalRetrace;

            if (glwin != null)
                glwin.IsBoundForRendering = false;

            glwin = win;
            glcontext = context;

            if (glwin != null)
                glwin.IsBoundForRendering = true;
        }

        // OpenGL context state.
        private SDL2FrameworkWindow glwin;
        private IntPtr glcontext;
    }
}
