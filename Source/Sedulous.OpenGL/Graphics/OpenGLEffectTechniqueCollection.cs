using System.Collections.Generic;
using Sedulous.Graphics;

namespace Sedulous.OpenGL.Graphics
{
    /// <summary>
    /// Represents the OpenGL implementation of the EffectTechniqueCollection class.
    /// </summary>
    public sealed class OpenGLEffectTechniqueCollection : EffectTechniqueCollection
    {
        /// <summary>
        /// Initializes a new instance of the OpenGLEffectTechniqueCollection class.
        /// </summary>
        /// <param name="techniques">A collection of effect techniques to add to the collection.</param>
        public OpenGLEffectTechniqueCollection(IEnumerable<OpenGLEffectTechnique> techniques)
        {
            if (techniques != null)
            {
                foreach (var technique in techniques)
                {
                    AddInternal(technique);
                }
            }
        }
    }
}
