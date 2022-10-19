using System;
using Sedulous.Graphics.Graphics2D.Text;
using Sedulous.Core;

namespace Sedulous.FreeType2
{
    /// <summary>
    /// Contains methods for managing the lifetime of the FreeType2 Font Plugin for Sedulous.
    /// </summary>
    public unsafe class FreeTypeFontPlugin : SedulousPlugin
    {
        /// <inheritdoc/>
        public override void Initialize(SedulousContext uv, SedulousFactory factory)
        {
            Contract.Require(uv, nameof(uv));

            library.InitializeResource();

            var content = uv.GetContent();
            var existing = content.Importers.FindImporter(".ttf");
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

            content.RegisterImportersAndProcessors(typeof(FreeTypeFontPlugin).Assembly);

            factory.SetFactoryMethod<TextShaperFactory>((uvctx, capacity) => new HarfBuzzTextShaper(uvctx, capacity));
        }

        /// <summary>
        /// Gets the pointer to the FreeType2 library handle.
        /// </summary>
        internal static IntPtr Library => library.Value.Native;

        // The native FreeType2 library object.
        private static readonly SedulousSingleton<FreeTypeLibrary> library = 
            new SedulousSingleton<FreeTypeLibrary>(uv => new FreeTypeLibrary(uv));
    }
}
