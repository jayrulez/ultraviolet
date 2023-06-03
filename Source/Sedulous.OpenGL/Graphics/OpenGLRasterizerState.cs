using System;
using Sedulous.Graphics;
using Sedulous.OpenGL.Bindings;
using Sedulous.OpenGL.Graphics.Caching;

namespace Sedulous.OpenGL.Graphics
{
    /// <summary>
    /// Represents the OpenGL implementation of the RasterizerState class.
    /// </summary>
    public class OpenGLRasterizerState : RasterizerState
    {
        /// <summary>
        /// Initializes a new instance of the OpenGLRasterizerState class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        public OpenGLRasterizerState(FrameworkContext context)
            : base(context)
        {

        }

        /// <summary>
        /// Creates the CullClockwise rasterizer state.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <returns>The rasterizer state that was created.</returns>
        public static OpenGLRasterizerState CreateCullClockwise(FrameworkContext context)
        {
            var state = new OpenGLRasterizerState(context);
            state.CullMode = CullMode.CullClockwiseFace;
            state.MakeImmutable();
            return state;
        }

        /// <summary>
        /// Creates the CullCounterClockwise rasterizer state.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <returns>The rasterizer state that was created.</returns>
        public static OpenGLRasterizerState CreateCullCounterClockwise(FrameworkContext context)
        {
            var state = new OpenGLRasterizerState(context);
            state.CullMode = CullMode.CullCounterClockwiseFace;
            state.MakeImmutable();
            return state;
        }

        /// <summary>
        /// Creates the CullNone rasterizer state.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <returns>The rasterizer state that was created.</returns>
        public static OpenGLRasterizerState CreateCullNone(FrameworkContext context)
        {
            var state = new OpenGLRasterizerState(context);
            state.CullMode = CullMode.None;
            state.MakeImmutable();
            return state;
        }

        /// <summary>
        /// Applies the rasterizer state to the device.
        /// </summary>
        internal void Apply()
        {
            if (FillMode != FillMode.Solid)
                GL.ThrowIfGLES(OpenGLStrings.UnsupportedFillModeGLES);

            OpenGLState.CullingEnabled = (CullMode != CullMode.None);
            OpenGLState.CulledFace = GL.GL_BACK;
            OpenGLState.FrontFace = GetFrontFaceGL(CullMode);
            OpenGLState.PolygonMode = GetFillModeGL(FillMode);            
            OpenGLState.ScissorTestEnabled = ScissorTestEnable;

            if (DepthBias != 0f && SlopeScaleDepthBias != 0f)
            {
                OpenGLState.PolygonOffsetEnabled = true;
                OpenGLState.PolygonOffset = new CachedPolygonOffset(SlopeScaleDepthBias, DepthBias);
            }
            else
            {
                OpenGLState.PolygonOffsetEnabled = false;
            }
        }

        /// <summary>
        /// Gets the OpenGL face mode that corresponds to the specified CullMode value.
        /// </summary>
        /// <param name="mode">The CullMode value to convert.</param>
        /// <returns>The converted value.</returns>
        private static UInt32 GetFrontFaceGL(CullMode mode)
        {
            switch (mode)
            {
                case CullMode.None:
                case CullMode.CullCounterClockwiseFace:
                    // Cull back faces with counterclockwise vertices, i.e. front is clockwise
                    return GL.GL_CW;

                case CullMode.CullClockwiseFace:
                    // Cull back faces with clockwise vertices, i.e. front is counterclockwise
                    return GL.GL_CCW;
            }
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the OpenGL fill mode value that corresponds to the specified FillMode value.
        /// </summary>
        /// <param name="mode">The FillMode value to convert.</param>
        /// <returns>The converted value.</returns>
        private static UInt32 GetFillModeGL(FillMode mode)
        {
            switch (mode)
            {
                case FillMode.Solid:
                    return GL.GL_FILL;
                case FillMode.Wireframe:
                    return GL.GL_LINE;
            }
            throw new NotSupportedException();
        }
    }
}
