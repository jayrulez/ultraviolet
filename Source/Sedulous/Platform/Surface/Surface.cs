namespace Sedulous.Platform
{
    /// <summary>
    /// Represents the base class for all surfaces.
    /// </summary>
    public abstract class Surface : FrameworkResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Surface"/> class.
        /// </summary>
        /// <param name="context">The Framework context.</param>
        public Surface(FrameworkContext context)
            : base(context)
        {

        }
    }
}
