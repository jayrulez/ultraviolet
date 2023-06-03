using System;
using Sedulous.Core;
using Sedulous.Presentation.Controls.Primitives;

namespace Sedulous.Presentation.Controls
{
    /// <summary>
    /// Represents a UI element which displays progress towards some goal.
    /// </summary>
    [UvmlKnownType(null, "Sedulous.Presentation.Controls.Templates.ProgressBar.xml")]
    [UvmlDefaultProperty("Value")]
    public class ProgressBar : RangeBase
    {
        /// <summary>
        /// Initializes the <see cref="ProgressBar"/> type.
        /// </summary>
        static ProgressBar()
        {
            FocusableProperty.OverrideMetadata(typeof(ProgressBar), new PropertyMetadata<Boolean>(CommonBoxedValues.Boolean.False));
            MaximumProperty.OverrideMetadata(typeof(ProgressBar), new PropertyMetadata<Double>(100.0));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBar"/> class.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        /// <param name="name">The element's identifying name within its namescope.</param>
        public ProgressBar(FrameworkContext uv, String name)
            : base(uv, name)
        {

        }
    }
}
