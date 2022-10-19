using System;
using System.Collections.Generic;
using System.IO;
using Sedulous.Content;
using Sedulous.Graphics;

namespace Sedulous.OpenGL.Graphics
{
    /// <summary>
    /// Loads shader effect assets from shader source files.
    /// </summary>
    [ContentProcessor]
    public sealed partial class OpenGLEffectImplementationProcessorFromShaderSource : EffectImplementationProcessor<String>
    {
        /// <inheritdoc/>
        public override EffectImplementation Process(ContentManager manager, IContentProcessorMetadata metadata, String input)
        {
            var isFragShader = (metadata.Extension == ".frag");
            var isVertShader = (metadata.Extension == ".vert");

            if (!isFragShader && !isVertShader)
                throw new InvalidDataException(OpenGLStrings.ImplicitEffectsMustLoadFromShaders.Format(metadata.AssetPath));

            var vertShaderFilePath = isFragShader ? Path.ChangeExtension(metadata.AssetPath, "vert") : metadata.AssetPath;
            var fragShaderFilePath = isFragShader ? metadata.AssetPath : Path.ChangeExtension(metadata.AssetPath, "frag");
            metadata.AddAssetDependency(isFragShader ? vertShaderFilePath : fragShaderFilePath);

            var vertShaderSource = ShaderSource.ProcessExterns(manager.Load<ShaderSource>(vertShaderFilePath), Externs);
            var vertShader = new OpenGLVertexShader(manager.Sedulous, new[] { vertShaderSource });

            var fragShaderSource = ShaderSource.ProcessExterns(manager.Load<ShaderSource>(fragShaderFilePath), Externs);
            var fragShader = new OpenGLFragmentShader(manager.Sedulous, new[] { fragShaderSource });

            var program = new OpenGLShaderProgram(manager.Sedulous, vertShader, fragShader, false);
            var pass = new OpenGLEffectPass(manager.Sedulous, "Default", new[] { program });
            var technique = new OpenGLEffectTechnique(manager.Sedulous, "Default", new[] { pass });

            return new OpenGLEffectImplementation(manager.Sedulous, new[] { technique });
        }

        /// <inheritdoc/>
        public override Boolean SupportsPreprocessing => false;        
    }
}
