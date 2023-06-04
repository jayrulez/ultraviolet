using Sedulous.Core;
using Sedulous.UI;

namespace Sedulous.ImGuiViewProvider
{
    /// <summary>
    /// Represents a plugin for the Sedulous Framework which provides user interface views using Dear ImGui.
    /// </summary>
    public sealed class ImGuiPlugin : FrameworkPlugin
    {
        /// <inheritdoc/>
        public override void Register(FrameworkConfiguration sedulousConfig)
        {
            Contract.Require(sedulousConfig, nameof(sedulousConfig));

            sedulousConfig.ViewProviderAssembly = typeof(ImGuiPlugin).Assembly.FullName;
            sedulousConfig.ViewProviderConfiguration = null;
        }

        /// <inheritdoc/>
        public override void Configure(FrameworkContext context, FrameworkFactory factory)
        {
            factory.SetFactoryMethod<UIViewFactory>((uv, uiPanel, uiPanelDefinition, vmfactory) => ImGuiView.Create(uv, uiPanel, uiPanelDefinition, vmfactory));

            base.Configure(context, factory);
        }
    }
}
