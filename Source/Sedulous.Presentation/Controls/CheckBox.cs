using System;
using Sedulous.Presentation.Controls.Primitives;

namespace Sedulous.Presentation.Controls
{
    /// <summary>
    /// Represents a check box.
    /// </summary>
    [UvmlKnownType(null, "Sedulous.Presentation.Controls.Templates.CheckBox.xml")]
    public class CheckBox : ToggleButton
    {
        /// <summary>
        /// Initializes the <see cref="CheckBox"/> type.
        /// </summary>
        static CheckBox()
        {
            VerticalContentAlignmentProperty.OverrideMetadata(typeof(CheckBox), new PropertyMetadata<VerticalAlignment>(VerticalAlignment.Top));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckBox"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="name">The element's identifying name within its namescope.</param>
        public CheckBox(FrameworkContext context, String name)
            : base(context, name)
        {

        }
    }
}
