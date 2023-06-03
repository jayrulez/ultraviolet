using System;
using Sedulous.Content;

namespace Sedulous.OpenGL.Graphics
{
    /// <summary>
    /// Loads fragment shader assets.
    /// </summary>
    [ContentProcessor]
    public sealed class OpenGLFragmentShaderProcessor : ContentProcessor<String, OpenGLFragmentShader>
    {
        /// <inheritdoc/>
        public override OpenGLFragmentShader Process(ContentManager manager, IContentProcessorMetadata metadata, String input)
        {
            var source = ShaderSource.ProcessRawSource(manager, metadata, input, ShaderStage.Fragment);
            return new OpenGLFragmentShader(manager.FrameworkContext, new[] { source });
        }
    }
}
