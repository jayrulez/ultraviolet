using System;

namespace Sedulous
{
    /// <summary>
    /// Contains commonly-used boxed values of the Sedulous Framework's value types.
    /// </summary>
    public static class FrameworkBoxedValues
    {
        /// <summary>
        /// Contains boxed <see cref="Sedulous.Color"/> values.
        /// </summary>
        public static class Color
        {
            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Color.White"/>.
            /// </summary>
            public static readonly Object White = Sedulous.Color.White;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Color.Black"/>.
            /// </summary>
            public static readonly Object Black = Sedulous.Color.Black;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Color.Transparent"/>.
            /// </summary>
            public static readonly Object Transparent = Sedulous.Color.Transparent;
        }

        /// <summary>
        /// Contains boxed <see cref="Sedulous.Graphics.Graphics2D.FrameworkFontStyle"/> values.
        /// </summary>
        public static class SpriteFontStyle
        {
            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Graphics.Graphics2D.FrameworkFontStyle.Regular"/>.
            /// </summary>
            public static readonly Object Regular =
                Sedulous.Graphics.Graphics2D.FrameworkFontStyle.Regular;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Graphics.Graphics2D.FrameworkFontStyle.Bold"/>.
            /// </summary>
            public static readonly Object Bold =
                Sedulous.Graphics.Graphics2D.FrameworkFontStyle.Bold;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Graphics.Graphics2D.FrameworkFontStyle.Italic"/>.
            /// </summary>
            public static readonly Object Italic =
                Sedulous.Graphics.Graphics2D.FrameworkFontStyle.Italic;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Graphics.Graphics2D.FrameworkFontStyle.BoldItalic"/>.
            /// </summary>
            public static readonly Object BoldItalic =
                Sedulous.Graphics.Graphics2D.FrameworkFontStyle.BoldItalic;
        }
    }
}
