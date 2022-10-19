using System;

namespace Sedulous.Presentation
{
    /// <summary>
    /// Contains common boxed values of the Presentation Foundation's value types.
    /// </summary>
    public static class PresentationBoxedValues
    {
        /// <summary>
        /// Contains boxed <see cref="Sedulous.Presentation.Controls.Orientation"/> values.
        /// </summary>
        public static class Orientation
        {
            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.Controls.Orientation.Horizontal"/>.
            /// </summary>
            public static readonly Object Horizontal =
                Sedulous.Presentation.Controls.Orientation.Horizontal;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.Controls.Orientation.Vertical"/>.
            /// </summary>
            public static readonly Object Vertical =
                Sedulous.Presentation.Controls.Orientation.Vertical;
        }

        /// <summary>
        /// Contains boxed <see cref="Sedulous.Presentation.Thickness"/> values.
        /// </summary>
        public static class Thickness
        {
            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.Thickness.Zero"/>.
            /// </summary>
            public static readonly Object Zero =
                Sedulous.Presentation.Thickness.Zero;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.Thickness.One"/>.
            /// </summary>
            public static readonly Object One =
                Sedulous.Presentation.Thickness.One;
        }

        /// <summary>
        /// Contains boxed <see cref="Sedulous.Presentation.HorizontalAlignment"/> values.
        /// </summary>
        public static class HorizontalAlignment
        {
            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.HorizontalAlignment.Left"/>.
            /// </summary>
            public static readonly Object Left =
                Sedulous.Presentation.HorizontalAlignment.Left;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.HorizontalAlignment.Center"/>.
            /// </summary>
            public static readonly Object Center =
                Sedulous.Presentation.HorizontalAlignment.Center;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.HorizontalAlignment.Right"/>.
            /// </summary>
            public static readonly Object Right =
                Sedulous.Presentation.HorizontalAlignment.Right;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.HorizontalAlignment.Stretch"/>.
            /// </summary>
            public static readonly Object Stretch =
                Sedulous.Presentation.HorizontalAlignment.Stretch;
        }

        /// <summary>
        /// Contains boxed <see cref="Sedulous.Presentation.VerticalAlignment"/> values.
        /// </summary>
        public static class VerticalAlignment
        {
            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.VerticalAlignment.Top"/>.
            /// </summary>
            public static readonly Object Top =
                Sedulous.Presentation.VerticalAlignment.Top;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.VerticalAlignment.Center"/>.
            /// </summary>
            public static readonly Object Center =
                Sedulous.Presentation.VerticalAlignment.Center;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.VerticalAlignment.Bottom"/>.
            /// </summary>
            public static readonly Object Bottom =
                Sedulous.Presentation.VerticalAlignment.Bottom;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.VerticalAlignment.Stretch"/>.
            /// </summary>
            public static readonly Object Stretch =
                Sedulous.Presentation.VerticalAlignment.Stretch;
        }

        /// <summary>
        /// Contains boxed <see cref="Sedulous.Presentation.GridLength"/> values.
        /// </summary>
        public static class GridLength
        {
            /// <summary>
            /// The cached box for a <see cref="Sedulous.Presentation.GridLength"/> of zero pixels.
            /// </summary>
            public static readonly Object Zero =
                new Sedulous.Presentation.GridLength(0);

            /// <summary>
            /// The cached box for a <see cref="Sedulous.Presentation.GridLength"/> of one pixel.
            /// </summary>
            public static readonly Object One =
                new Sedulous.Presentation.GridLength(1);

            /// <summary>
            /// The cached box for an auto-sized <see cref="Sedulous.Presentation.GridLength"/>.
            /// </summary>
            public static readonly Object Auto =
                Sedulous.Presentation.GridLength.Auto;
        }

        /// <summary>
        /// Contains boxed <see cref="Sedulous.Presentation.Controls.Dock"/> values.
        /// </summary>
        public static class Dock
        {
            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.Controls.Dock.Left"/>.
            /// </summary>
            public static readonly Object Left =
                Sedulous.Presentation.Controls.Dock.Left;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.Controls.Dock.Top"/>.
            /// </summary>
            public static readonly Object Top =
                Sedulous.Presentation.Controls.Dock.Top;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.Controls.Dock.Right"/>.
            /// </summary>
            public static readonly Object Right =
                Sedulous.Presentation.Controls.Dock.Right;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.Controls.Dock.Bottom"/>.
            /// </summary>
            public static readonly Object Bottom =
                Sedulous.Presentation.Controls.Dock.Bottom;
        }

        /// <summary>
        /// Contains boxed <see cref="Sedulous.Presentation.Controls.ClickMode"/> values.
        /// </summary>
        public static class ClickMode
        {
            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.Controls.ClickMode.Hover"/>.
            /// </summary>
            public static readonly Object Hover =
                Sedulous.Presentation.Controls.ClickMode.Hover;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.Controls.ClickMode.Press"/>.
            /// </summary>
            public static readonly Object Press =
                Sedulous.Presentation.Controls.ClickMode.Press;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.Controls.ClickMode.Release"/>.
            /// </summary>
            public static readonly Object Release =
                Sedulous.Presentation.Controls.ClickMode.Release;
        }

        /// <summary>
        /// Contains boxed <see cref="Sedulous.Presentation.Controls.ScrollBarVisibility"/> values.
        /// </summary>
        public static class ScrollBarVisibility
        {
            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.Controls.ScrollBarVisibility.Auto"/>.
            /// </summary>
            public static readonly Object Auto =
                Sedulous.Presentation.Controls.ScrollBarVisibility.Auto;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.Controls.ScrollBarVisibility.Disabled"/>.
            /// </summary>
            public static readonly Object Disabled =
                Sedulous.Presentation.Controls.ScrollBarVisibility.Disabled;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.Controls.ScrollBarVisibility.Hidden"/>.
            /// </summary>
            public static readonly Object Hidden =
                Sedulous.Presentation.Controls.ScrollBarVisibility.Hidden;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.Controls.ScrollBarVisibility.Visible"/>.
            /// </summary>
            public static readonly Object Visible =
                Sedulous.Presentation.Controls.ScrollBarVisibility.Visible;
        }

        /// <summary>
        /// Contains boxed <see cref="Sedulous.Presentation.Visibility"/> values.
        /// </summary>
        public static class Visibility
        {
            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.Visibility.Collapsed"/>.
            /// </summary>
            public static readonly Object Collapsed =
                Sedulous.Presentation.Visibility.Collapsed;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.Visibility.Hidden"/>.
            /// </summary>
            public static readonly Object Hidden =
                Sedulous.Presentation.Visibility.Hidden;

            /// <summary>
            /// The cached box for the value <see cref="Sedulous.Presentation.Visibility.Visible"/>.
            /// </summary>
            public static readonly Object Visible =
                Sedulous.Presentation.Visibility.Visible;
        }
    }
}
