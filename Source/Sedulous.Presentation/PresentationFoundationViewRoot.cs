﻿using System;
using Sedulous.Core;
using Sedulous.Presentation.Controls;
using Sedulous.Presentation.Controls.Primitives;
using Sedulous.Presentation.Input;
using Sedulous.Presentation.Media;

namespace Sedulous.Presentation
{
    /// <summary>
    /// Represents the root layout element for a <see cref="PresentationFoundationView"/>.
    /// </summary>
    public sealed class PresentationFoundationViewRoot : FrameworkElement
    {
        /// <summary>
        /// Initializes the <see cref="PresentationFoundationViewRoot"/> type.
        /// </summary>
        static PresentationFoundationViewRoot()
        {
            FocusManager.IsFocusScopeProperty.OverrideMetadata(
                typeof(PresentationFoundationView), new PropertyMetadata<Boolean>(CommonBoxedValues.Boolean.True, PropertyMetadataOptions.None));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PresentationFoundationViewRoot"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="name">The element's identifying name within its namescope.</param>
        public PresentationFoundationViewRoot(FrameworkContext context, String name) 
            : base(context, name)
        {
            this.children = new VisualCollection(this);

            this.toolTipPopup = new Popup(context, null);
            this.toolTipPopup.IsHitTestVisible = false;
            this.children.Add(this.toolTipPopup);

            this.toolTip = new ToolTip(context, null);
            this.toolTipPopup.Child = this.toolTip;
        }

        /// <summary>
        /// Gets the view root's child element.
        /// </summary>
        public UIElement Child
        {
            get { return child; }
            internal set
            {
                if (child == value)
                    return;

                var oldChild = this.child;
                var newChild = value;

                if (oldChild != null)
                    oldChild.ChangeLogicalAndVisualParents(null, null);

                this.child = newChild;

                if (newChild != null)
                    newChild.ChangeLogicalAndVisualParents(this, this);

                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets the popup that contains the view's tooltips.
        /// </summary>
        internal Popup ToolTipPopup
        {
            get { return toolTipPopup; }
        }

        /// <summary>
        /// Gets the view's tooltip control.
        /// </summary>
        internal ToolTip ToolTip
        {
            get { return toolTip; }
        }

        /// <inheritdoc/>
        protected internal override UIElement GetLogicalChild(Int32 childIndex)
        {
            switch (childIndex)
            {
                case 0:
                    return child ?? toolTipPopup;

                case 1:
                    if (child != null)
                    {
                        return toolTipPopup;
                    }
                    break;
            }
            throw new ArgumentOutOfRangeException("childIndex");
        }

        /// <inheritdoc/>
        protected internal override UIElement GetVisualChild(Int32 childIndex)
        {
            switch (childIndex)
            {
                case 0:
                    return child ?? toolTipPopup;

                case 1:
                    if (child != null)
                    {
                        return toolTipPopup;
                    }
                    break;
            }
            throw new ArgumentOutOfRangeException("childIndex");
        }

        /// <inheritdoc/>
        protected internal override Int32 LogicalChildrenCount
        {
            get { return (child == null) ? 1 : 2; }
        }

        /// <inheritdoc/>
        protected internal override Int32 VisualChildrenCount
        {
            get { return (child == null) ? 1 : 2; }
        }

        /// <inheritdoc/>
        protected override Size2D MeasureOverride(Size2D availableSize)
        {
            if (child != null)
            {
                child.Measure(availableSize);
                return child.DesiredSize;
            }
            toolTipPopup.Measure(availableSize);
            return Size2D.Zero;
        }

        /// <inheritdoc/>
        protected override Size2D ArrangeOverride(Size2D finalSize, ArrangeOptions options)
        {
            if (child != null)
            {
                child.Arrange(new RectangleD(Point2D.Zero, finalSize), options);
            }
            toolTipPopup.Arrange(new RectangleD(Point2D.Zero, finalSize), options);
            return finalSize;
        }

        // State values.
        private readonly VisualCollection children;
        private UIElement child;

        // Tooltips.
        private readonly Popup toolTipPopup;
        private readonly ToolTip toolTip;
    }
}
