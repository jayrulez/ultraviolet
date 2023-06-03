using System;

namespace Sedulous.Presentation.Controls
{
    /// <summary>
    /// Represents a framework element which renders a rectangle.
    /// </summary>
    [UvmlKnownType]
    public class Rectangle : Shape
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="name">The element's identifying name within its namescope.</param>
        public Rectangle(FrameworkContext context, String name)
            : base(context, name)
        {

        }

        /// <inheritdoc/>
        protected override void DrawOverride(FrameworkTime time, DrawingContext dc)
        {
            DrawBlank(dc, null, FillColor);

            base.DrawOverride(time, dc);
        }
    }
}
