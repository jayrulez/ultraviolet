using Sedulous.TestFramework;

namespace Sedulous.Presentation.Tests
{
    /// <summary>
    /// Contains extension methods for the <see cref="IFrameworkTestApplication"/> interface.
    /// </summary>
    public static class IFrameworkTestApplicationExtensions
    {
        /// <summary>
        /// Specifies that the application should configure the Presentation Foundation.
        /// </summary>
        /// <returns>The Sedulous test application.</returns>
        public static IFrameworkTestApplication WithPresentationFoundationConfigured(this IFrameworkTestApplication @this)
        {
            return @this.WithPlugin(new PresentationFoundationPlugin());
        }
    }
}
