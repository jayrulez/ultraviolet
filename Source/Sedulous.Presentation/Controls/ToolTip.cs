﻿using System;

namespace Sedulous.Presentation.Controls
{
    /// <summary>
    /// Represents a control which displays information about another control when it is hovered over with the mouse.
    /// </summary>
    [UvmlKnownType(null, "Sedulous.Presentation.Controls.Templates.ToolTip.xml")]
    public class ToolTip : ContentControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolTip"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="name">The element's identifying name within its namescope.</param>
        public ToolTip(FrameworkContext context, String name)
            : base(context, name)
        {

        }
    }
}
