namespace Sedulous.Graphics
{
    /// <summary>
    /// Represents the base class for all surfaces.
    /// </summary>
    public abstract class Surface : FrameworkResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Surface"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        public Surface(FrameworkContext context)
            : base(context)
        {

        }
    }
}
