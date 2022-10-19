using System;

namespace Sedulous.Presentation.Controls
{
    /// <summary>
    /// Represents a horizontal slider.
    /// </summary>
    [UvmlKnownType(null, "Sedulous.Presentation.Controls.Templates.HSlider.xml")]
    public class HSlider : OrientedSlider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HSlider"/> class.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        /// <param name="name">The element's identifying name within its namescope.</param>
        public HSlider(SedulousContext uv, String name)
            : base(uv, name)
        {

        }
    }
}
