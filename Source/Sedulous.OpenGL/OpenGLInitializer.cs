using System;
using Sedulous.Core;
using Sedulous.OpenGL.Bindings;

namespace Sedulous.OpenGL
{
    /// <summary>
    /// Represents an object capable of querying the OpenGL driver for information 
    /// required by Sedulous's OpenGL initialization process.
    /// </summary>
    public sealed class OpenGLInitializer : IOpenGLInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLInitializer"/> class.
        /// </summary>
        /// <param name="environment">The interface with which OpenGL communicates with its underlying platform environment.</param>
        public OpenGLInitializer(OpenGLEnvironment environment)
        {
            Contract.Require(environment, nameof(environment));
            this.environment = environment;
        }

        /// <inheritdoc/>
        public void Prepare() { }

        /// <inheritdoc/>
        public void Cleanup() => environment.ClearErrors();

        /// <inheritdoc/>
        public IntPtr GetProcAddress(String factoryName) => environment.GetProcAddress(factoryName);

        // The interface with which OpenGL communicates with its underlying platform environment.
        private readonly OpenGLEnvironment environment;
    }
}
