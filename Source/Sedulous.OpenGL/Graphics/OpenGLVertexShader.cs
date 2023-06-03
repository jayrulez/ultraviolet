using System;
using Sedulous.Core;
using Sedulous.OpenGL.Bindings;

namespace Sedulous.OpenGL.Graphics
{
    /// <summary>
    /// Represents a vertex shader.
    /// </summary>
    public unsafe sealed class OpenGLVertexShader : FrameworkResource, IOpenGLResource
    {
        /// <summary>
        /// Initializes a new instance of the OpenGLVertexShader class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="source">The shader source.</param>
        public OpenGLVertexShader(FrameworkContext context, ShaderSource[] source)
            : base(context)
        {
            Contract.Require(source, nameof(source));

            var shader = 0u;
            var ssmd = default(ShaderSourceMetadata);

            context.QueueWorkItem(state =>
            {
                shader = gl.CreateShader(gl.GL_VERTEX_SHADER);
                gl.ThrowIfError();
                
                if (!ShaderCompiler.Compile(shader, source, out var log, out ssmd))
                    throw new InvalidOperationException(log);                
            }).Wait();

            this.OpenGLName = shader;
            this.ShaderSourceMetadata = ssmd;
        }

        /// <summary>
        /// Initializes a new instance of the OpenGLVertexShader class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="source">The shader source.</param>
        public OpenGLVertexShader(FrameworkContext context, ShaderSource source)
            : this(context, new[] { source })
        {

        }

        /// <summary>
        /// Gets the OpenGL shader handle.
        /// </summary>
        public UInt32 OpenGLName { get; private set; }

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
                var glname = OpenGLName;
                if (glname != 0 && !FrameworkContext.Disposed)
                {
                    FrameworkContext.QueueWorkItem((state) =>
                    {
                        gl.DeleteShader(glname);
                        gl.ThrowIfError();
                    }, this, WorkItemOptions.ReturnNullOnSynchronousExecution);
                }

                OpenGLName = 0;
            }

            base.Dispose(disposing);
        }
    }
}
