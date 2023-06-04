using System;
using System.IO;
using SharpGLTF.Schema2;
using Sedulous.Content;

namespace Sedulous.Graphics.Graphics3D
{
    /// <summary>
    /// Represents a content importer which loads GLB model files.
    /// </summary>
    //[ContentImporter(".glb"), CLSCompliant(false)]
    [CLSCompliant(false)]
    public class GlbModelImporter : ContentImporter<ModelRoot>
    {
        /// <inheritdoc/>
        public override ModelRoot Import(IContentImporterMetadata metadata, Stream stream)
        {
            return ModelRoot.ReadGLB(stream);
        }
    }
}
