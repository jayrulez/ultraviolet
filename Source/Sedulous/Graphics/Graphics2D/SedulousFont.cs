using System;

namespace Sedulous.Graphics.Graphics2D
{
    /// <summary>
    /// Represents the base class for all font classes.
    /// </summary>
    public abstract class SedulousFont : SedulousResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SedulousFont{TFontFace}"/> class.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        /// <param name="regular">The font's regular font face.</param>
        /// <param name="bold">The font's bold font face.</param>
        /// <param name="italic">The font's italic font face.</param>
        /// <param name="boldItalic">The font's bold italic font face.</param>
        protected SedulousFont(SedulousContext uv, SedulousFontFace regular, SedulousFontFace bold, SedulousFontFace italic, SedulousFontFace boldItalic)
            : base(uv)
        {
            uv.ValidateResource(regular);
            uv.ValidateResource(bold);
            uv.ValidateResource(italic);
            uv.ValidateResource(boldItalic);

            if (regular == null && bold == null && italic == null && boldItalic == null)
                throw new ArgumentException(SedulousStrings.InvalidFontFaces);

            this.Regular = regular;
            this.Bold = bold;
            this.Italic = italic;
            this.BoldItalic = boldItalic;
        }

        /// <summary>
        /// Implicitly converts an <see cref="SedulousFont"/> into a font face by returning the font's regular face.
        /// </summary>
        /// <param name="font">The <see cref="SedulousFont"/> to convert.</param>
        /// <returns>The converted font face.</returns>
        public static implicit operator SedulousFontFace(SedulousFont font) => font?.GetFace(SedulousFontStyle.Regular);

        /// <summary>
        /// Gets the font face that corresponds to the specified style.
        /// </summary>
        /// <remarks>If the requested font face does not exist, the closest matching font face will be returned instead.</remarks>
        /// <param name="style">The style for which to retrieve a font face.</param>
        /// <returns>The <see cref="SedulousFontFace"/> that corresponds to the specified style.</returns>
        public SedulousFontFace GetFace(SedulousFontStyle style)
        {
            switch (style)
            {
                case SedulousFontStyle.Regular:
                    return Regular ?? Bold ?? Italic ?? BoldItalic;
                case SedulousFontStyle.Bold:
                    return Bold ?? BoldItalic ?? Regular ?? Italic;
                case SedulousFontStyle.Italic:
                    return Italic ?? BoldItalic ?? Regular ?? Bold;
                case SedulousFontStyle.BoldItalic:
                    return BoldItalic ?? Italic ?? Bold ?? Regular;
            }
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the font face that corresponds to the specified style.
        /// </summary>
        /// <remarks>If the requested font face does not exist, the closest matching font face will be returned instead.</remarks>
        /// <param name="bold">A value indicating whether to retrieve a bold font face.</param>
        /// <param name="italic">A value indicating whether to retrieve an italic font face.</param>
        /// <returns>The <see cref="SedulousFontFace"/> that corresponds to the specified style.</returns>
        public SedulousFontFace GetFace(Boolean bold, Boolean italic)
        {
            if (bold && italic)
                return GetFace(SedulousFontStyle.BoldItalic);

            if (bold)
                return GetFace(SedulousFontStyle.Bold);

            if (italic)
                return GetFace(SedulousFontStyle.Italic);

            return GetFace(SedulousFontStyle.Regular);
        }

        /// <summary>
        /// Gets the font's regular face.
        /// </summary>
        public SedulousFontFace Regular { get; }

        /// <summary>
        /// Gets the font's bold face.
        /// </summary>
        public SedulousFontFace Bold { get; }

        /// <summary>
        /// Gets the font's italic face.
        /// </summary>
        public SedulousFontFace Italic { get; }

        /// <summary>
        /// Gets the font's bold/italic face.
        /// </summary>
        public SedulousFontFace BoldItalic { get; }
        
    }
}
