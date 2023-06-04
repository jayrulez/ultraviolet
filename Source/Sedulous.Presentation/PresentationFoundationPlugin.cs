﻿using Sedulous.Content;
using Sedulous.Presentation.Styles;
using Sedulous.UI;

namespace Sedulous.Presentation
{
    /// <summary>
    /// Represents a plugin for the Sedulous Framework which provides user interface views using the Sedulous Presentation Foundation.
    /// </summary>
    public sealed class PresentationFoundationPlugin : FrameworkPlugin
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
        public override void Register(FrameworkConfiguration configuration) =>
            PresentationFoundation.Configure(configuration, presentationConfig);


        /// <inheritdoc/>
        public override void Configure(FrameworkContext context, FrameworkFactory factory)
        {
            factory.SetFactoryMethod<UIViewProviderInitializerFactory>(() => new PresentationFoundationInitializer());
            factory.SetFactoryMethod<UIViewFactory>((uv, uiPanel, uiPanelDefinition, vmfactory) => PresentationFoundationView.Load(uv, uiPanel, uiPanelDefinition, vmfactory));
            factory.SetFactoryMethod<MessageBoxScreenFactory>((mb, mbowner) => new MessageBoxScreen(mb, mbowner.GlobalContent));

            base.Configure(context, factory);
        }



        /// <inheritdoc/>
        public override void RegisterContentImporters(ContentImporterRegistry importers)
        {
            importers.RegisterImporter<UvssDocumentImporter>(".uvss");

            base.RegisterContentImporters(importers);
        }

        /// <inheritdoc/>
        public override void RegisterContentProcessors(ContentProcessorRegistry processors)
        {
            processors.RegisterProcessor<UvssDocumentProcessor>();

            base.RegisterContentProcessors(processors);
        }

        // UPF configuration settings.
        private readonly PresentationFoundationConfiguration presentationConfig;
    }
}
