using Sedulous.Graphics.Graphics2D;

namespace Sedulous.FreeType2
{
    /// <summary>
    /// Represents an implementation of the <see cref="SedulousFont"/> class using the FreeType2 library.
    /// </summary>
    public sealed class FreeTypeFont : SedulousFont<FreeTypeFontFace>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreeTypeFont"/> class.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        /// <param name="face">The <see cref="FreeTypeFontFace"/> that constitutes the font.</param>
        public FreeTypeFont(SedulousContext uv, FreeTypeFontFace face)
            : this(uv, face, face, face, face)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreeTypeFont"/> class.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        /// <param name="regular">The <see cref="FreeTypeFontFace"/> that represents the font's regular style.</param>
        /// <param name="bold">The <see cref="FreeTypeFontFace"/> that represents the font's bold style.</param>
        /// <param name="italic">The <see cref="FreeTypeFontFace"/> that represents the font's italic style.</param>
        /// <param name="boldItalic">The <see cref="FreeTypeFontFace"/> that represents the font's bold/italic style.</param>
        public FreeTypeFont(SedulousContext uv, FreeTypeFontFace regular, FreeTypeFontFace bold, FreeTypeFontFace italic, FreeTypeFontFace boldItalic)
            : base(uv, regular, bold, italic, boldItalic)
        { }
    }
}
