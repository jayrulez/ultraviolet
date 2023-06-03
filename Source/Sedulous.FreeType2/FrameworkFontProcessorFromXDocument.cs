using System;
using System.IO;
using System.Xml.Linq;
using Sedulous.Content;
using Sedulous.Graphics.Graphics2D;

namespace Sedulous.FreeType2
{
    /// <summary>
    /// Loads font assets.
    /// </summary>
    [ContentProcessor]
    public sealed class FrameworkFontProcessorFromXDocument : ContentProcessor<XDocument, FrameworkFont>
    {
        /// <inheritdoc/>
        public override void ExportPreprocessed(ContentManager manager, IContentProcessorMetadata metadata, BinaryWriter writer, XDocument input, Boolean delete) =>
            GetSpriteFontProcessor().ExportPreprocessed(manager, metadata, writer, input, delete);

        /// <inheritdoc/>
        public override FrameworkFont ImportPreprocessed(ContentManager manager, IContentProcessorMetadata metadata, BinaryReader reader) =>
            GetSpriteFontProcessor().ImportPreprocessed(manager, metadata, reader);

        /// <inheritdoc/>
        public override FrameworkFont Process(ContentManager manager, IContentProcessorMetadata metadata, XDocument input) =>
            GetSpriteFontProcessor().Process(manager, metadata, input);

        /// <inheritdoc/>
        public override Boolean SupportsPreprocessing => 
            GetSpriteFontProcessor().SupportsPreprocessing;

        /// <summary>
        /// Gets the registered content processor for loading <see cref="SpriteFont"/> instances from XML documents.
        /// </summary>
        private static ContentProcessor<XDocument, SpriteFont> GetSpriteFontProcessor()
        {
            var impl = FrameworkContext.DemandCurrent().GetContent().Processors.FindProcessor(typeof(XDocument), typeof(SpriteFont));
            if (impl == null)
                throw new InvalidOperationException(FreeTypeStrings.ContentRedirectionError.Format(typeof(XDocument).Name, typeof(SpriteFont).Name));

            return (ContentProcessor<XDocument, SpriteFont>)impl;
        }
    }
}
