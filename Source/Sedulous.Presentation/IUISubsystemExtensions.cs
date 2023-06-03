using Sedulous.Core;

namespace Sedulous.Presentation
{
    /// <summary>
    /// Contains extension methods for the <see cref="IUISubsystem"/> interface.
    /// </summary>
    public static class IUISubsystemExtensions
    {
        /// <summary>
        /// Gets the core management object for the Sedulous Presentation Foundation.
        /// </summary>
        /// <param name="ui">The Sedulous context's UI subsystem.</param>
        /// <returns>The core management object for the Sedulous Presentation Foundation.</returns>
        public static PresentationFoundation GetPresentationFoundation(this IUISubsystem ui)
        {
            Contract.Require(ui, nameof(ui));

            return PresentationFoundation.Instance;
        }
    }
}
