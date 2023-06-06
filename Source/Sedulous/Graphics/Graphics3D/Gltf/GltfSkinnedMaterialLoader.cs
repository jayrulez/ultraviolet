using System;
using SharpGLTF.Schema2;
using Sedulous.Content;
using Sedulous.Core;

namespace Sedulous.Graphics.Graphics3D
{
    /// <summary>
    /// Represents an implementation of the <see cref="GltfMaterialLoader"/> class which creates instances of <see cref="SkinnedMaterial"/>.
    /// </summary>
    [CLSCompliant(false)]
    public class GltfSkinnedMaterialLoader : GltfBasicMaterialLoader
    {
        /// <inheritdoc/>
        public override Material CreateMaterialForPrimitive(ContentManager contentManager, MeshPrimitive primitive)
        {
            Contract.Require(contentManager, nameof(contentManager));
            Contract.Require(primitive, nameof(primitive));

            var alpha = GetMaterialAlpha(primitive.Material);
            var diffuseColor = GetMaterialDiffuseColor(primitive.Material);
            var specularPower = GetMaterialSpecularPower(primitive.Material);
            var specularColor = GetMaterialSpecularColor(primitive.Material);
            var emissiveColor = GetMaterialEmissiveColor(primitive.Material);
            var texture = GetMaterialTexture(contentManager, primitive.Material);
            if (texture == null)
                texture = GetBlankTexture();

            return new SkinnedMaterial
            {
                Alpha = alpha,
                DiffuseColor = diffuseColor,
                SpecularPower = specularPower,
                SpecularColor = specularColor,
                EmissiveColor = emissiveColor,
                Texture = texture,
            };
        }
    }
}
