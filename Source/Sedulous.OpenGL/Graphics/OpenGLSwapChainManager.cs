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
        /// <param name="uv">The Sedulous context.</param>
        public OpenGLSwapChainManager(FrameworkContext uv)
            : base(uv)
        { }

        /// <inheritdoc/>
        public override void DrawAndSwap(FrameworkTime time, 
            Action<FrameworkContext, FrameworkTime, IFrameworkWindow> onWindowDrawing, 
            Action<FrameworkContext, FrameworkTime, IFrameworkWindow> onWindowDrawn)
        {
            var graphics = (OpenGLGraphicsSubsystem)Sedulous.GetGraphics();
            var platform = Sedulous.GetPlatform();

            var glenv = graphics.OpenGLEnvironment;
            var glcontext = graphics.OpenGLContext;

            foreach (var window in platform.Windows)
            {
                glenv.DesignateCurrentWindow(window, glcontext);

                window.Compositor.BeginFrame();
                window.Compositor.BeginContext(CompositionContext.Scene);

                onWindowDrawing?.Invoke(Sedulous, time, window);

                glenv.DrawFramebuffer(time);

                onWindowDrawn?.Invoke(Sedulous, time, window);

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
