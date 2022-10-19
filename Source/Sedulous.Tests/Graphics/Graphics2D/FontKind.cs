namespace Sedulous.Tests.Graphics.Graphics2D
{
    /// <summary>
    /// Represents the kind of font used in a test.
    /// </summary>
    public enum FontKind
    {
        /// <summary>
        /// The built-in <see cref="Sedulous.Graphics.Graphics2D.SpriteFont"/> class.
        /// </summary>
        SpriteFont,

        /// <summary>
        /// The plugin-provided <see cref="Sedulous.FreeType2.FreeTypeFont"/> class.
        /// </summary>
        FreeType2,
    }
}
