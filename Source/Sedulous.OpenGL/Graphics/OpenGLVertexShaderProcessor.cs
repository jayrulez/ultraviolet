using System;
using Sedulous.Content;

namespace Sedulous.OpenGL.Graphics
{
    /// <summary>
    /// Loads vertex shader assets.
    /// </summary>
    [ContentProcessor]
    public sealed class OpenGLVertexShaderProcessor : ContentProcessor<String, OpenGLVertexShader>
    {
        /// <inheritdoc/>
        public override OpenGLVertexShader Process(ContentManager manager, IContentProcessorMetadata metadata, String input)
        {
            var source = ShaderSource.ProcessRawSource(manager, metadata, input, ShaderStage.Vertex);
            return new OpenGLVertexShader(manager.Sedulous, new[] { source });
        }
    }
}
