using Sedulous.TestFramework;

namespace Sedulous.Presentation.Tests
{
    /// <summary>
    /// Contains extension methods for the <see cref="ISedulousTestApplication"/> interface.
    /// </summary>
    public static class ISedulousTestApplicationExtensions
    {
        /// <summary>
        /// Specifies that the application should configure the Presentation Foundation.
        /// </summary>
        /// <returns>The Sedulous test application.</returns>
        public static ISedulousTestApplication WithPresentationFoundationConfigured(this ISedulousTestApplication @this)
        {
            return @this.WithPlugin(new PresentationFoundationPlugin());
        }
    }
}
