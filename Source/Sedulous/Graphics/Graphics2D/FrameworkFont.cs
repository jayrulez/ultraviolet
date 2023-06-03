using System;

namespace Sedulous.Graphics.Graphics2D
{
    /// <summary>
    /// Represents the base class for all font classes.
    /// </summary>
    public abstract class FrameworkFont : FrameworkResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkFont{TFontFace}"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="regular">The font's regular font face.</param>
        /// <param name="bold">The font's bold font face.</param>
        /// <param name="italic">The font's italic font face.</param>
        /// <param name="boldItalic">The font's bold italic font face.</param>
        protected FrameworkFont(FrameworkContext context, FrameworkFontFace regular, FrameworkFontFace bold, FrameworkFontFace italic, FrameworkFontFace boldItalic)
            : base(context)
        {
            context.ValidateResource(regular);
            context.ValidateResource(bold);
            context.ValidateResource(italic);
            context.ValidateResource(boldItalic);

            if (regular == null && bold == null && italic == null && boldItalic == null)
                throw new ArgumentException(FrameworkStrings.InvalidFontFaces);

            this.Regular = regular;
            this.Bold = bold;
            this.Italic = italic;
            this.BoldItalic = boldItalic;
        }

        /// <summary>
        /// Implicitly converts an <see cref="FrameworkFont"/> into a font face by returning the font's regular face.
        /// </summary>
        /// <param name="font">The <see cref="FrameworkFont"/> to convert.</param>
        /// <returns>The converted font face.</returns>
        public static implicit operator FrameworkFontFace(FrameworkFont font) => font?.GetFace(FrameworkFontStyle.Regular);

        /// <summary>
        /// Gets the font face that corresponds to the specified style.
        /// </summary>
        /// <remarks>If the requested font face does not exist, the closest matching font face will be returned instead.</remarks>
        /// <param name="style">The style for which to retrieve a font face.</param>
        /// <returns>The <see cref="FrameworkFontFace"/> that corresponds to the specified style.</returns>
        public FrameworkFontFace GetFace(FrameworkFontStyle style)
        {
            switch (style)
            {
                case FrameworkFontStyle.Regular:
                    return Regular ?? Bold ?? Italic ?? BoldItalic;
                case FrameworkFontStyle.Bold:
                    return Bold ?? BoldItalic ?? Regular ?? Italic;
                case FrameworkFontStyle.Italic:
                    return Italic ?? BoldItalic ?? Regular ?? Bold;
                case FrameworkFontStyle.BoldItalic:
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
        /// <returns>The <see cref="FrameworkFontFace"/> that corresponds to the specified style.</returns>
        public FrameworkFontFace GetFace(Boolean bold, Boolean italic)
        {
            if (bold && italic)
                return GetFace(FrameworkFontStyle.BoldItalic);

            if (bold)
                return GetFace(FrameworkFontStyle.Bold);

            if (italic)
                return GetFace(FrameworkFontStyle.Italic);

            return GetFace(FrameworkFontStyle.Regular);
        }

        /// <summary>
        /// Gets the font's regular face.
        /// </summary>
        public FrameworkFontFace Regular { get; }

        /// <summary>
        /// Gets the font's bold face.
        /// </summary>
        public FrameworkFontFace Bold { get; }

        /// <summary>
        /// Gets the font's italic face.
        /// </summary>
        public FrameworkFontFace Italic { get; }

        /// <summary>
        /// Gets the font's bold/italic face.
        /// </summary>
        public FrameworkFontFace BoldItalic { get; }
        
    }
}
