using Sedulous.UI;

namespace Sedulous.Presentation
{
    /// <summary>
    /// Initializes factory methods for the Sedulous Presentation Foundation.
    /// </summary>
    public sealed class PresentationFoundationFactoryInitializer : IFrameworkFactoryInitializer
    {
        /// <inheritdoc/>
        public void Initialize(FrameworkContext owner, FrameworkFactory factory)
        {
            factory.SetFactoryMethod<UIViewProviderInitializerFactory>(() => new PresentationFoundationInitializer());
            factory.SetFactoryMethod<UIViewFactory>((uv, uiPanel, uiPanelDefinition, vmfactory) => PresentationFoundationView.Load(uv, uiPanel, uiPanelDefinition, vmfactory));
            factory.SetFactoryMethod<MessageBoxScreenFactory>((mb, mbowner) => new MessageBoxScreen(mb, mbowner.GlobalContent));
        }
    }
}
