using System;
using Sedulous.Graphics;
using Sedulous.OpenGL.Bindings;
using Sedulous.OpenGL.Graphics.Caching;

namespace Sedulous.OpenGL.Graphics
{
    /// <summary>
    /// Represents the OpenGL implementation of the BlendState class.
    /// </summary>
    public class OpenGLBlendState : BlendState
    {
        /// <summary>
        /// Initializes a new instance of the OpenGLBlendState class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        public OpenGLBlendState(FrameworkContext context)
            : base(context)
        {

        }

        /// <summary>
        /// Creates an Opaque blend state.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <returns>The blend state that was created.</returns>
        public static OpenGLBlendState CreateOpaque(FrameworkContext context)
        {
            var state = new OpenGLBlendState(context);
            state.AlphaSourceBlend = Blend.One;
            state.AlphaDestinationBlend = Blend.Zero;
            state.ColorSourceBlend = Blend.One;
            state.ColorDestinationBlend = Blend.Zero;
            state.MakeImmutable();
            return state;
        }

        /// <summary>
        /// Creates an AlphaBlend blend state.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <returns>The blend state that was created.</returns>
        public static OpenGLBlendState CreateAlphaBlend(FrameworkContext context)
        {
            var state = new OpenGLBlendState(context);
            state.AlphaSourceBlend = Blend.One;
            state.AlphaDestinationBlend = Blend.InverseSourceAlpha;
            state.ColorSourceBlend = Blend.One;
            state.ColorDestinationBlend = Blend.InverseSourceAlpha;
            state.MakeImmutable();
            return state;
        }

        /// <summary>
        /// Creates an Additive blend state.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <returns>The blend state that was created.</returns>
        public static OpenGLBlendState CreateAdditive(FrameworkContext context)
        {
            var state = new OpenGLBlendState(context);
            state.AlphaSourceBlend = Blend.SourceAlpha;
            state.AlphaDestinationBlend = Blend.One;
            state.ColorSourceBlend = Blend.SourceAlpha;
            state.ColorDestinationBlend = Blend.One;
            state.MakeImmutable();
            return state;
        }

        /// <summary>
        /// Creates a NonPremultiplied blend state.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <returns>The blend state that was created.</returns>
        public static OpenGLBlendState CreateNonPremultiplied(FrameworkContext context)
        {
            var state = new OpenGLBlendState(context);
            state.AlphaSourceBlend = Blend.SourceAlpha;
            state.AlphaDestinationBlend = Blend.InverseSourceAlpha;
            state.ColorSourceBlend = Blend.SourceAlpha;
            state.ColorDestinationBlend = Blend.InverseSourceAlpha;
            state.MakeImmutable();
            return state;
        }

        /// <summary>
        /// Gets a value indicating whether this state uses separate alpha and color blending.
        /// </summary>
        public Boolean IsSeparateBlend
        {
            get
            {
                return
                    AlphaBlendFunction != ColorBlendFunction ||
                    AlphaSourceBlend != ColorSourceBlend ||
                    AlphaDestinationBlend != ColorDestinationBlend;
            }
        }

        /// <summary>
        /// Applies the blend state to the device.
        /// </summary>
        internal void Apply()
        {
            OpenGLState.BlendEnabled = true;
            OpenGLState.BlendColor = BlendFactor;
            OpenGLState.BlendEquation = new CachedBlendEquation(
                GetBlendFunctionGL(ColorBlendFunction),
                GetBlendFunctionGL(AlphaBlendFunction));
            OpenGLState.BlendFunction = new CachedBlendFunction(
                GetBlendGL(ColorSourceBlend, false),
                GetBlendGL(AlphaSourceBlend, true),
                GetBlendGL(ColorDestinationBlend, false),
                GetBlendGL(AlphaDestinationBlend, true));

            OpenGLState.ColorMask = ColorWriteChannels;
        }

        /// <summary>
        /// Converts an Sedulous BlendFunction value to the equivalent OpenGL value.
        /// </summary>
        /// <param name="fn">The Sedulous BlendFunction value to convert.</param>
        /// <returns>The converted OpenGL value.</returns>
        private static UInt32 GetBlendFunctionGL(BlendFunction fn)
        {
            switch (fn)
            {
                case BlendFunction.Add:
                    return GL.GL_FUNC_ADD;
                case BlendFunction.Min:
                    return GL.GL_MIN;
                case BlendFunction.Max:
                    return GL.GL_MAX;
                case BlendFunction.ReverseSubtract:
                    return GL.GL_FUNC_REVERSE_SUBTRACT;
                case BlendFunction.Subtract:
                    return GL.GL_FUNC_SUBTRACT;
            }
            throw new NotSupportedException();
        }

        /// <summary>
        /// Converts an Sedulous Blend value to the equivalent OpenGL value.
        /// </summary>
        /// <param name="blend">The Sedulous Blend value to convert.</param>
        /// <param name="alpha">A value indicating whether alpha blending is enabled.</param>
        /// <returns>The converted OpenGL value.</returns>
        private static UInt32 GetBlendGL(Blend blend, Boolean alpha)
        {
            switch (blend)
            {
                case Blend.Zero:
                    return GL.GL_ZERO;
                case Blend.One:
                    return GL.GL_ONE;
                case Blend.SourceColor:
                    return GL.GL_SRC_COLOR;
                case Blend.InverseSourceColor:
                    return GL.GL_ONE_MINUS_SRC_COLOR;
                case Blend.SourceAlpha:
                    return GL.GL_SRC_ALPHA;
                case Blend.InverseSourceAlpha:
                    return GL.GL_ONE_MINUS_SRC_ALPHA;
                case Blend.DestinationAlpha:
                    return GL.GL_DST_ALPHA;
                case Blend.InverseDestinationAlpha:
                    return GL.GL_ONE_MINUS_DST_ALPHA;
                case Blend.DestinationColor:
                    return GL.GL_DST_COLOR;
                case Blend.InverseDestinationColor:
                    return GL.GL_ONE_MINUS_DST_COLOR;
                case Blend.SourceAlphaSaturation:
                    return GL.GL_SRC_ALPHA_SATURATE;
                case Blend.BlendFactor:
                    return alpha ? GL.GL_CONSTANT_ALPHA : GL.GL_CONSTANT_COLOR;
                case Blend.InverseBlendFactor:
                    return alpha ? GL.GL_ONE_MINUS_CONSTANT_ALPHA : GL.GL_ONE_MINUS_CONSTANT_COLOR;
            }
            throw new NotSupportedException();
        }
    }
}
