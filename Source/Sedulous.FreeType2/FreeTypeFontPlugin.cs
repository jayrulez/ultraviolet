using System;
using Sedulous.Graphics.Graphics2D.Text;
using Sedulous.Core;
using Sedulous.Content;

namespace Sedulous.FreeType2
{
    /// <summary>
    /// Contains methods for managing the lifetime of the FreeType2 Font Plugin for Sedulous.
    /// </summary>
    public unsafe class FreeTypeFontPlugin : FrameworkPlugin
    {
        /// <inheritdoc/>
        public override void Initialize(FrameworkContext context, FrameworkFactory factory)
        {
            Contract.Require(context, nameof(context));

            library.InitializeResource();

            factory.SetFactoryMethod<TextShaperFactory>((uvctx, capacity) => new HarfBuzzTextShaper(uvctx, capacity));

            base.Configure(context, factory);
        }

        /// <inheritdoc/>
        public override void RegisterContentImporters(ContentImporterRegistry importers)
        {
            var existing = importers.FindImporter(".ttf");
            if (existing != null)
            {
                if (existing.GetType() == typeof(FreeTypeFontImporter))
                {
                    throw new InvalidOperationException(FreeTypeStrings.PluginAlreadyInitialized);
                }
                else
                {
                    throw new InvalidOperationException(FreeTypeStrings.AlternativePluginAlreadyInitialized);
                }
            }

            importers.RegisterImporter<FreeTypeFontImporter>(".ttf");
            importers.RegisterImporter<FreeTypeFontImporter>(".ttc");
            importers.RegisterImporter<FreeTypeFontImporter>(".otf");
            importers.RegisterImporter<FreeTypeFontImporter>(".otc");
            importers.RegisterImporter<FreeTypeFontImporter>(".fnt");

            base.RegisterContentImporters(importers);
        }

        /// <inheritdoc/>
        public override void RegisterContentProcessors(ContentProcessorRegistry processors)
        {
            processors.RegisterProcessor<FreeTypeFontProcessor>();
            processors.RegisterProcessor<FrameworkFontProcessorFromFreeType>();
            processors.RegisterProcessor<FrameworkFontProcessorFromJObject>();
            processors.RegisterProcessor<FrameworkFontProcessorFromXDocument>();

            base.RegisterContentProcessors(processors);
        }

        /// <summary>
        /// Gets the pointer to the FreeType2 library handle.
        /// </summary>
        internal static IntPtr Library => library.Value.Native;

        // The native FreeType2 library object.
        private static readonly FrameworkSingleton<FreeTypeLibrary> library = 
            new FrameworkSingleton<FreeTypeLibrary>(uv => new FreeTypeLibrary(uv));
    }
}
