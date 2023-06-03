using System;
using Sedulous.Core;
using Sedulous.OpenGL.Bindings;

namespace Sedulous.OpenGL.Graphics
{
    /// <summary>
    /// Represents a fragment shader.
    /// </summary>
    public sealed class OpenGLFragmentShader : FrameworkResource, IOpenGLResource
    {
        /// <summary>
        /// Initializes a new instance of the OpenGLFragmentShader class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="source">The shader source.</param>
        public OpenGLFragmentShader(FrameworkContext context, ShaderSource[] source)
            : base(context)
        {
            Contract.Require(source, nameof(source));

            var shader = 0u;
            var ssmd = default(ShaderSourceMetadata);

            context.QueueWorkItem(state =>
            {
                shader = gl.CreateShader(gl.GL_FRAGMENT_SHADER);
                gl.ThrowIfError();
                
                if (!ShaderCompiler.Compile(shader, source, out var log, out ssmd))
                    throw new InvalidOperationException(log);
            }).Wait();

            this.shader = shader;
            this.ShaderSourceMetadata = ssmd;
        }
        
        /// <summary>
        /// Initializes a new instance of the OpenGLFragmentShader class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="source">The shader source.</param>
        public OpenGLFragmentShader(FrameworkContext context, ShaderSource source)
            : this(context, new[] { source })
        {

        }

        /// <summary>
        /// Gets the OpenGL shader handle.
        /// </summary>
        public UInt32 OpenGLName => shader;

        /// <summary>
        /// Gets the shader source metadata for this shader.
        /// </summary>
        public ShaderSourceMetadata ShaderSourceMetadata { get; }

        /// <summary>
        /// Releases resources associated with this object.
        /// </summary>
        /// <param name="disposing">true if the object is being disposed; false if the object is being finalized.</param>
        protected override void Dispose(Boolean disposing)
        {
            if (Disposed)
                return;

            if (disposing)
            {
                var glname = shader;
                if (glname != 0 && !FrameworkContext.Disposed)
                {
                    FrameworkContext.QueueWorkItem((state) =>
                    {
                        gl.DeleteShader(glname);
                        gl.ThrowIfError();
                    }, this, WorkItemOptions.ReturnNullOnSynchronousExecution);
                }

                shader = 0;
            }

            base.Dispose(disposing);
        }

        // State values.
        private UInt32 shader;
    }
}
