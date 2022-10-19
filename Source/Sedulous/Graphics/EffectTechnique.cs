using System;

namespace Sedulous.Graphics
{
    /// <summary>
    /// Represents one of an effect's techniques, which contains all of the state necessary
    /// to render a particular material.
    /// </summary>
    public abstract class EffectTechnique : SedulousResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EffectTechnique"/> class.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        public EffectTechnique(SedulousContext uv)
            : base(uv)
        {

        }

        /// <summary>
        /// Gets the effect technique's name.
        /// </summary>
        public abstract String Name
        {
            get;
        }
        
        /// <summary>
        /// Gets the effect technique's collection of passes.
        /// </summary>
        public abstract EffectPassCollection Passes
        {
            get;
        }
    }
}
