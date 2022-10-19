using Sedulous.Core;

namespace Sedulous.Presentation
{
    /// <summary>
    /// Contains extension methods for the <see cref="ISedulousUI"/> interface.
    /// </summary>
    public static class ISedulousUIExtensions
    {
        /// <summary>
        /// Gets the core management object for the Sedulous Presentation Foundation.
        /// </summary>
        /// <param name="ui">The Sedulous context's UI subsystem.</param>
        /// <returns>The core management object for the Sedulous Presentation Foundation.</returns>
        public static PresentationFoundation GetPresentationFoundation(this ISedulousUI ui)
        {
            Contract.Require(ui, nameof(ui));

            return PresentationFoundation.Instance;
        }
    }
}
