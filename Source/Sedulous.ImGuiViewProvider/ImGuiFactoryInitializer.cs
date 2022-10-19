﻿using Sedulous.UI;

namespace Sedulous.ImGuiViewProvider
{
    /// <summary>
    /// Initializes factory methods for the Sedulous ImGui View Provider Plugin.
    /// </summary>
    public class ImGuiFactoryInitializer : ISedulousFactoryInitializer
    {
        /// <inheritdoc/>
        public void Initialize(SedulousContext owner, SedulousFactory factory)
        {
            factory.SetFactoryMethod<UIViewFactory>((uv, uiPanel, uiPanelDefinition, vmfactory) => ImGuiView.Create(uv, uiPanel, uiPanelDefinition, vmfactory));
        }
    }
}
