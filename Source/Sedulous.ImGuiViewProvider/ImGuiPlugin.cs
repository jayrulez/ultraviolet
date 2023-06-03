using Sedulous.Core;

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
    }
}
