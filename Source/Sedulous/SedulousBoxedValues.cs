﻿using System;

namespace Sedulous
{
    /// <summary>
    /// Contains commonly-used boxed values of the Sedulous Framework's value types.
    /// </summary>
    public static class SedulousBoxedValues
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
        /// Contains boxed <see cref="Sedulous.Graphics.Graphics2D.SedulousFontStyle"/> values.
        /// </summary>
        public static class SpriteFontStyle
        {
            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Graphics.Graphics2D.SedulousFontStyle.Regular"/>.
            /// </summary>
            public static readonly Object Regular =
                Sedulous.Graphics.Graphics2D.SedulousFontStyle.Regular;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Graphics.Graphics2D.SedulousFontStyle.Bold"/>.
            /// </summary>
            public static readonly Object Bold =
                Sedulous.Graphics.Graphics2D.SedulousFontStyle.Bold;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Graphics.Graphics2D.SedulousFontStyle.Italic"/>.
            /// </summary>
            public static readonly Object Italic =
                Sedulous.Graphics.Graphics2D.SedulousFontStyle.Italic;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Graphics.Graphics2D.SedulousFontStyle.BoldItalic"/>.
            /// </summary>
            public static readonly Object BoldItalic =
                Sedulous.Graphics.Graphics2D.SedulousFontStyle.BoldItalic;
        }
    }
}
