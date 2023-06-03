using Sedulous.Core;
using Sedulous.Graphics;
using Sedulous.Graphics.Graphics2D;

namespace Sedulous.OpenGL.Graphics.Graphics2D
{
    /// <summary>
    /// Represents the OpenGL implementation of the sprite batch custom effect.
    /// </summary>
    public sealed class OpenGLSpriteBatchEffect : SpriteBatchEffect
    {
        /// <summary>
        /// Initializes a new instance of the OpenGLSpriteBatchEffect class.
        /// </summary>
        public OpenGLSpriteBatchEffect(FrameworkContext context)
            : base(CreateEffectImplementation(context))
        {

        }

        /// <summary>
        /// Creates the effect implementation.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <returns>The effect implementation.</returns>
        private static EffectImplementation CreateEffectImplementation(FrameworkContext context)
        {
            Contract.Require(context, nameof(context));

            var programs = new[] { new OpenGLShaderProgram(context, vertShader, fragShader, false) };
            var passes = new[] { new OpenGLEffectPass(context, null, programs) };
            var techniques = new[] { new OpenGLEffectTechnique(context, null, passes) };
            return new OpenGLEffectImplementation(context, techniques);
        }

        // The shaders that make up this effect.
        private static readonly FrameworkSingleton<OpenGLVertexShader> vertShader = 
            new FrameworkSingleton<OpenGLVertexShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy, uv => { 
                return new OpenGLVertexShader(uv, ResourceUtil.ReadShaderResourceString("SpriteBatchEffect.vert")); });
        private static readonly FrameworkSingleton<OpenGLFragmentShader> fragShader = 
            new FrameworkSingleton<OpenGLFragmentShader>(FrameworkSingletonFlags.DisabledInServiceMode | FrameworkSingletonFlags.Lazy, uv => {
                return new OpenGLFragmentShader(uv, ResourceUtil.ReadShaderResourceString("SpriteBatchEffect.frag")); });
    }
}
