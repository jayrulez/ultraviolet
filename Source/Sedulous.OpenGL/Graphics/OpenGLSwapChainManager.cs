using System;
using Sedulous.Graphics;
using Sedulous.Platform;

namespace Sedulous.OpenGL.Graphics
{
    /// <summary>
    /// Represents an OpenGL implementation of the <see cref="SwapChainManager"/> class.
    /// </summary>
    public class OpenGLSwapChainManager : SwapChainManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLSwapChainManager"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        public OpenGLSwapChainManager(FrameworkContext context)
            : base(context)
        { }

        /// <inheritdoc/>
        public override void DrawAndSwap(FrameworkTime time, 
            Action<FrameworkContext, FrameworkTime, IFrameworkWindow> onWindowDrawing, 
            Action<FrameworkContext, FrameworkTime, IFrameworkWindow> onWindowDrawn)
        {
            var graphics = (OpenGLGraphicsSubsystem)FrameworkContext.GetGraphics();
            var platform = FrameworkContext.GetPlatform();

            var glenv = graphics.OpenGLEnvironment;
            var glcontext = graphics.OpenGLContext;

            foreach (var window in platform.Windows)
            {
                glenv.DesignateCurrentWindow(window, glcontext);

                window.Compositor.BeginFrame();
                window.Compositor.BeginContext(CompositionContext.Scene);

                onWindowDrawing?.Invoke(FrameworkContext, time, window);

                glenv.DrawFramebuffer(time);

                onWindowDrawn?.Invoke(FrameworkContext, time, window);

                window.Compositor.Compose();
                window.Compositor.Present();

                glenv.SwapFramebuffers();
            }

            glenv.DesignateCurrentWindow(null, glcontext);

            graphics.SetRenderTargetToBackBuffer();
            graphics.UpdateFrameCount();
        }
    }
}
