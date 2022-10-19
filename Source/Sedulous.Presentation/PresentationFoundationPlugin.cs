namespace Sedulous.Presentation
{
    /// <summary>
    /// Represents a plugin for the Sedulous Framework which provides user interface views using the Sedulous Presentation Foundation.
    /// </summary>
    public sealed class PresentationFoundationPlugin : SedulousPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PresentationFoundationPlugin"/> class.
        /// </summary>
        /// <param name="presentationConfig">Configuration settings for the Sedulous Presentation Foundation.</param>
        public PresentationFoundationPlugin(PresentationFoundationConfiguration presentationConfig = null)
        {
            this.presentationConfig = presentationConfig;
        }

        /// <inheritdoc/>
        public override void Register(SedulousConfiguration configuration) =>
            PresentationFoundation.Configure(configuration, presentationConfig);

        // UPF configuration settings.
        private readonly PresentationFoundationConfiguration presentationConfig;
    }
}
