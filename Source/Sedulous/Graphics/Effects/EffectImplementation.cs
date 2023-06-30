
using System;

namespace Sedulous.Graphics
{
    /// <summary>
    /// Represents a factory method which constructs instances of the <see cref="EffectImplementation"/> class.
    /// </summary>
    /// <param name="context">The Framework context.</param>
    /// <returns>The instance of <see cref="EffectImplementation"/> that was created.</returns>
    public delegate EffectImplementation EffectImplementationFactory(FrameworkContext context);

    /// <summary>
    /// Represents a shader effect's underlying implementation.
    /// </summary>
    public abstract class EffectImplementation : FrameworkResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EffectImplementation"/> class.
        /// </summary>
        /// <param name="context">The Framework context.</param>
        protected EffectImplementation(FrameworkContext context)
            : base(context)
        {

        }

        /// <summary>
        /// Gets the <see cref="Effect"/> that owns this implementation.
        /// </summary>
        public Effect Effect
        {
            get => effect;
            internal set
            {
                if (effect != null)
                    throw new InvalidOperationException(FrameworkStrings.EffectImplementationAlreadyHasOwner);

                effect = value;
            }
        }

        /// <summary>
        /// Gets the effect's collection of parameters.
        /// </summary>
        public abstract EffectParameterCollection Parameters
        {
            get;
        }

        /// <summary>
        /// Gets the effect's collection of techniques.
        /// </summary>
        public abstract EffectTechniqueCollection Techniques
        {
            get;
        }

        /// <summary>
        /// Gets or sets the effect's current technique.
        /// </summary>
        public abstract EffectTechnique CurrentTechnique
        {
            get;
            set;
        }

        /// <summary>
        /// Calls the <see cref="Effect.OnApply()"/> method for the implementation's owning effect.
        /// </summary>
        protected void OnApplyInternal()
        {
            if (effect == null)
                throw new InvalidOperationException(FrameworkStrings.EffectImplementationHasNoOwner);

            effect.OnApply();
        }

        // The implementation's owning effect.
        private Effect effect;
    }
}