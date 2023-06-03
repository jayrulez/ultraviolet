namespace Sedulous.Graphics.Graphics2D
{
    /// <summary>
    /// Represents a bitmap font used for rendering text.
    /// </summary>
    public class SpriteFont : FrameworkFont<SpriteFontFace>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteFont"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="face">The <see cref="SpriteFontFace"/> that constitutes the font.</param>
        public SpriteFont(FrameworkContext context, SpriteFontFace face)
            : this(context, face, face, face, face)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteFont"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="regular">The <see cref="SpriteFontFace"/> that represents the font's regular style.</param>
        /// <param name="bold">The <see cref="SpriteFontFace"/> that represents the font's bold style.</param>
        /// <param name="italic">The <see cref="SpriteFontFace"/> that represents the font's italic style.</param>
        /// <param name="boldItalic">The <see cref="SpriteFontFace"/> that represents the font's bold/italic style.</param>
        public SpriteFont(FrameworkContext context, SpriteFontFace regular, SpriteFontFace bold, SpriteFontFace italic, SpriteFontFace boldItalic)
            : base(context, regular, bold, italic, boldItalic)
        { }      
    }
}
