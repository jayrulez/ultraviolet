using Sedulous.Core;

namespace Sedulous.Platform
{
    /// <summary>
    /// Represents a factory method which constructs instances of the <see cref="ScreenRotationService"/> class.
    /// </summary>
    /// <param name="display">The <see cref="IFrameworkDisplay"/> for which to retrieve rotation information.</param>
    /// <returns>The instance of <see cref="ScreenRotationService"/> that was created.</returns>
    public delegate ScreenRotationService ScreenRotationServiceFactory(IFrameworkDisplay display);

    /// <summary>
    /// Represents a service which is responsible for querying the screen's rotation.
    /// </summary>
    public abstract class ScreenRotationService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenRotationService"/> class.
        /// </summary>
        /// <param name="display">The <see cref="IFrameworkDisplay"/> for which to retrieve rotation information.</param>
        protected ScreenRotationService(IFrameworkDisplay display)
        {
            Contract.Require(display, nameof(display));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ScreenRotationService"/> class.
        /// </summary>
        /// <param name="display">The <see cref="IFrameworkDisplay"/> for which to retrieve rotation information.</param>
        /// <returns>The instance of <see cref="ScreenRotationService"/> that was created.</returns>
        public static ScreenRotationService Create(IFrameworkDisplay display)
        {
            var uv = FrameworkContext.DemandCurrent();
            return uv.GetFactoryMethod<ScreenRotationServiceFactory>()(display);
        }

        /// <summary>
        /// Gets the screen's rotation on devices which can be rotated.
        /// </summary>
        public abstract ScreenRotation ScreenRotation
        {
            get;
        }
    }
}
