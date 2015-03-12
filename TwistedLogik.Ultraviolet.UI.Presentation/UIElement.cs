﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TwistedLogik.Nucleus;
using TwistedLogik.Ultraviolet.Content;
using TwistedLogik.Ultraviolet.Graphics.Graphics2D;
using TwistedLogik.Ultraviolet.Input;
using TwistedLogik.Ultraviolet.Platform;
using TwistedLogik.Ultraviolet.UI.Presentation.Animations;
using TwistedLogik.Ultraviolet.UI.Presentation.Elements;
using TwistedLogik.Ultraviolet.UI.Presentation.Input;
using TwistedLogik.Ultraviolet.UI.Presentation.Styles;

namespace TwistedLogik.Ultraviolet.UI.Presentation
{
    /// <summary>
    /// Represents the method that is called when a class is added to or removed from a UI element.
    /// </summary>
    /// <param name="element">The UI element that raised the event.</param>
    /// <param name="classname">The name of the class that was added or removed.</param>
    public delegate void UIElementClassEventHandler(UIElement element, String classname);

    /// <summary>
    /// Represents the method that is called when a UI element is drawn.
    /// </summary>
    /// <param name="element">The element being drawn.</param>
    /// <param name="time">Time elapsed since the last call to <see cref="UltravioletContext.Draw(UltravioletTime)"/>.</param>
    /// <param name="dc">The drawing context that describes the render state of the layout.</param>
    public delegate void UIElementDrawingEventHandler(UIElement element, UltravioletTime time, DrawingContext dc);

    /// <summary>
    /// Represents the method that is called when a UI element is updated.
    /// </summary>
    /// <param name="element">The element being updated.</param>
    /// <param name="time">Time elapsed since the last call to <see cref="UltravioletContext.Update(UltravioletTime)"/>.</param>
    public delegate void UIElementUpdatingEventHandler(UIElement element, UltravioletTime time);

    /// <summary>
    /// Represents the method that is called when a UI element raises an event.
    /// </summary>
    /// <param name="element">The element that raised the event.</param>
    public delegate void UIElementEventHandler(UIElement element);

    /// <summary>
    /// Represents the method that is called when a UI element raises a routed event.
    /// </summary>
    /// <param name="element">The element that raised the event.</param>
    /// <param name="handled">A value indicating whether the event has been handled.</param>
    public delegate void UIElementRoutedEventHandler(UIElement element, ref Boolean handled);

    /// <summary>
    /// Represents the base class for all elements within the Ultraviolet Presentation Foundation.
    /// </summary>
    public abstract class UIElement : Visual
    {
        /// <summary>
        /// Initialies the <see cref="UIElement"/> type.
        /// </summary>
        static UIElement()
        {
            RoutedEvent.RegisterClassHandler(typeof(UIElement), Keyboard.GotKeyboardFocusEvent, new UIElementRoutedEventHandler(OnGotKeyboardFocusProxy));
            RoutedEvent.RegisterClassHandler(typeof(UIElement), Keyboard.LostKeyboardFocusEvent, new UIElementRoutedEventHandler(OnLostKeyboardFocusProxy));
            RoutedEvent.RegisterClassHandler(typeof(UIElement), Keyboard.KeyDownEvent, new UIElementKeyDownEventHandler(OnKeyDownProxy));
            RoutedEvent.RegisterClassHandler(typeof(UIElement), Keyboard.KeyUpEvent, new UIElementKeyEventHandler(OnKeyUpProxy));
            RoutedEvent.RegisterClassHandler(typeof(UIElement), Keyboard.TextInputEvent, new UIElementKeyboardEventHandler(OnTextInputProxy));

            RoutedEvent.RegisterClassHandler(typeof(UIElement), Mouse.GotMouseCaptureEvent, new UIElementRoutedEventHandler(OnGotMouseCaptureProxy));
            RoutedEvent.RegisterClassHandler(typeof(UIElement), Mouse.LostMouseCaptureEvent, new UIElementRoutedEventHandler(OnLostMouseCaptureProxy));
            RoutedEvent.RegisterClassHandler(typeof(UIElement), Mouse.MouseMoveEvent, new UIElementMouseMoveEventHandler(OnMouseMoveProxy));
            RoutedEvent.RegisterClassHandler(typeof(UIElement), Mouse.MouseEnterEvent, new UIElementMouseEventHandler(OnMouseEnterProxy));
            RoutedEvent.RegisterClassHandler(typeof(UIElement), Mouse.MouseLeaveEvent, new UIElementMouseEventHandler(OnMouseLeaveProxy));
            RoutedEvent.RegisterClassHandler(typeof(UIElement), Mouse.MouseDownEvent, new UIElementMouseButtonEventHandler(OnMouseDownProxy));
            RoutedEvent.RegisterClassHandler(typeof(UIElement), Mouse.MouseUpEvent, new UIElementMouseButtonEventHandler(OnMouseUpProxy));
            RoutedEvent.RegisterClassHandler(typeof(UIElement), Mouse.MouseClickEvent, new UIElementMouseButtonEventHandler(OnMouseClickProxy));
            RoutedEvent.RegisterClassHandler(typeof(UIElement), Mouse.MouseDoubleClickEvent, new UIElementMouseButtonEventHandler(OnMouseDoubleClickProxy));
            RoutedEvent.RegisterClassHandler(typeof(UIElement), Mouse.MouseWheelEvent, new UIElementMouseWheelEventHandler(OnMouseWheelProxy));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIElement"/> class.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        /// <param name="id">The unique identifier of this element within its layout.</param>
        public UIElement(UltravioletContext uv, String id)
        {
            Contract.Require(uv, "uv");

            this.uv      = uv;
            this.id      = id;
            this.classes = new UIElementClassCollection(this);

            var attr = (UvmlKnownTypeAttribute)GetType().GetCustomAttributes(typeof(UvmlKnownTypeAttribute), false).SingleOrDefault();
            if (attr != null)
            {
                this.name = attr.Name ?? GetType().Name;
            }
        }

        /// <summary>
        /// Initializes the element's dependency properties and the dependency properties
        /// of any children of this element.
        /// </summary>
        /// <param name="recursive">A value indicating whether to clear the dependency
        /// properties of this element's child elements.</param>
        public void InitializeDependencyProperties(Boolean recursive = true)
        {
            InitializeDependencyPropertiesCore(recursive);
        }

        /// <summary>
        /// Reloads this element's content and the content of any children of this element.
        /// </summary>
        /// <param name="recursive">A value indicating whether to reload the content
        /// of this element's child elements.</param>
        public void ReloadContent(Boolean recursive = true)
        {
            ReloadContentCore(recursive);
        }

        /// <summary>
        /// Clears the animations which are attached to this element, and optionally
        /// any animations attached to children of this element.
        /// </summary>
        /// <param name="recursive">A value indicating whether to clear the animations
        /// of this element's child elements.</param>
        public void ClearAnimations(Boolean recursive = true)
        {
            ClearAnimationsCore(recursive);
        }

        /// <summary>
        /// Clears the local values of this element's dependency properties,
        /// and optionally the local values of any dependency properties belonging
        /// to children of this element.
        /// </summary>
        /// <param name="recursive">A value indicating whether to clear the local dependency
        /// property values of this element's child elements.</param>
        public void ClearLocalValues(Boolean recursive = true)
        {
            ClearLocalValuesCore(recursive);
        }

        /// <summary>
        /// Clears the styled values of this element's dependency properties,
        /// and optionally the styled values of any dependency properties belonging
        /// to children of this element.
        /// </summary>
        /// <param name="recursive">A value indicating whether to clear the styled dependency
        /// property values of this element's child elements.</param>
        public void ClearStyledValues(Boolean recursive = true)
        {
            ClearStyledValuesCore(recursive);
        }

        /// <summary>
        /// Draws the element using the specified <see cref="SpriteBatch"/>.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="UltravioletContext.Draw(UltravioletTime)"/>.</param>
        /// <param name="dc">The drawing context that describes the render state of the layout.</param>
        public void Draw(UltravioletTime time, DrawingContext dc)
        {
            var clip = ClipRectangle;
            if (clip != null)
                dc.PushClipRectangle(clip.Value);

            var shouldDraw = true;

            if (clip.HasValue && clip.Value.IsEmpty)
                shouldDraw = false;

            var scissor = Ultraviolet.GetGraphics().GetScissorRectangle();
            if (scissor.HasValue)
            {
                var absBounds = Display.DipsToPixels(AbsoluteBounds);
                if (!absBounds.Intersects(scissor.Value))
                    shouldDraw = false;
            }

            if (shouldDraw)
            {
                dc.PushOpacity(Opacity);

                DrawCore(time, dc);
                OnDrawing(time, dc);

                dc.PopOpacity();
            }

            if (clip != null)
                dc.PopClipRectangle();
        }

        /// <summary>
        /// Updates the element's state.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="UltravioletContext.Update(UltravioletTime)"/>.</param>
        public void Update(UltravioletTime time)
        {
            foreach (var clock in storyboardClocks)
                clock.Value.Update(time);

            Digest(time);

            UpdateCore(time);
            OnUpdating(time);
        }

        /// <summary>
        /// Immediately updates the element's layout.
        /// </summary>
        public void UpdateLayout()
        {
            if (!IsMeasureValid && !IsArrangeValid)
                return;

            Measure(MostRecentAvailableSize);
            Arrange(MostRecentFinalRect);
        }

        /// <summary>
        /// Performs cleanup operations and releases any internal framework resources.
        /// The object remains usable after this method is called, but certain aspects
        /// of its state, such as animations, may be reset.
        /// </summary>
        public void Cleanup()
        {
            ClearAnimations(false);
            CleanupStoryboards();
            CleanupCore();
        }

        /// <summary>
        /// Caches layout parameters related to the element's position within the element hierarchy.
        /// </summary>
        public void CacheLayoutParameters()
        {
            var view = View;

            CacheLayoutDepth();
            CacheView();
            CacheControl();

            if (View != view)
            {
                InvalidateStyle();
            }

            CacheLayoutParametersCore();
        }

        /// <summary>
        /// Animates this element using the specified storyboard.
        /// </summary>
        /// <param name="storyboard">The storyboard being applied to the element.</param>
        /// <param name="clock">The storyboard clock that tracks playback.</param>
        /// <param name="root">The root element to which the storyboard is being applied.</param>
        public void Animate(Storyboard storyboard, StoryboardClock clock, UIElement root)
        {
            Contract.Require(storyboard, "storyboard");
            Contract.Require(clock, "clock");
            Contract.Require(root, "root");

            foreach (var target in storyboard.Targets)
            {
                var targetAppliesToElement = false;
                if (target.Selector == null)
                {
                    if (this == root)
                    {
                        targetAppliesToElement = true;
                    }
                }
                else
                {
                    targetAppliesToElement = target.Selector.MatchesElement(this, root);
                }

                if (targetAppliesToElement)
                {
                    foreach (var animation in target.Animations)
                    {
                        var dp = FindDependencyPropertyByName(animation.Key);
                        if (dp != null)
                        {
                            Animate(dp, animation.Value, clock);
                        }
                    }
                }
            }

            AnimateCore(storyboard, clock, root);
        }

        /// <summary>
        /// Applies the specified stylesheet to this element.
        /// </summary>
        /// <param name="stylesheet">The stylesheet to apply to this element.</param>
        public void Style(UvssDocument stylesheet)
        {
            if (View == null)
            {
                this.isStyleValid = true;
                return;
            }

            if (isStyleValid && mostRecentStylesheet == stylesheet)
                return;

            var upf = Ultraviolet.GetUI().GetPresentationFoundation();
            upf.PerformanceStats.StyleCountLastFrame++;

            this.mostRecentStylesheet = stylesheet;

            isStyling = true;
            try
            {
                ApplyStyles(stylesheet);
                StyleCore(stylesheet);
                ReloadContent(false);
            }
            finally
            {
                isStyling = false;
            }

            this.isStyleValid = true;

            InvalidateMeasure();

            upf.StyleQueue.Remove(this);
        }

        /// <summary>
        /// Calculates the element's desired size.
        /// </summary>
        /// <param name="availableSize">The size of the area which the element's parent has 
        /// specified is available for the element's layout.</param>
        public void Measure(Size2D availableSize)
        {
            Contract.EnsureRange(availableSize.Width >= 0 && availableSize.Height >= 0, "availableSize");

            if (View == null)
            {
                this.isMeasureValid = true;
                return;
            }

            if (isMeasureValid && mostRecentAvailableSize.Equals(availableSize))
                return;

            var upf = Ultraviolet.GetUI().GetPresentationFoundation();
            upf.PerformanceStats.MeasureCountLastFrame++;

            this.mostRecentAvailableSize = availableSize;

            var desiredSizeChanged = false;
            var desiredSize        = Size2D.Zero;

            isMeasuring = true;
            try
            {
                desiredSize = MeasureCore(availableSize);
            }
            finally
            {
                isMeasuring = false;
            }

            if (Double.IsPositiveInfinity(desiredSize.Width) || Double.IsPositiveInfinity(desiredSize.Height))
                throw new InvalidOperationException(PresentationStrings.MeasureMustProduceFiniteDesiredSize);

            if (!this.desiredSize.Equals(desiredSize))
                desiredSizeChanged = true;

            this.desiredSize    = desiredSize;
            this.isMeasureValid = true;

            InvalidateArrange();

            upf.MeasureQueue.Remove(this);

            if (desiredSizeChanged)
            {
                var parent = VisualTreeHelper.GetParent(this) as UIElement;
                if (parent != null)
                {
                    parent.OnChildDesiredSizeChanged(this);
                }
            }
        }

        /// <summary>
        /// Sets the element's final area relative to its parent and arranges
        /// the element's children within its layout area.
        /// </summary>
        /// <param name="finalRect">The element's final position and size relative to its parent element.</param>
        /// <param name="options">A set of <see cref="ArrangeOptions"/> values specifying the options for this arrangement.</param>
        public void Arrange(RectangleD finalRect, ArrangeOptions options = ArrangeOptions.None)
        {
            Contract.EnsureRange(finalRect.Width >= 0 && finalRect.Height >= 0, "finalRect");

            if (View == null)
            {
                this.isArrangeValid = true;
                return;
            }

            if (isArrangeValid && mostRecentFinalRect.Equals(finalRect) && ((Int32)mostRecentArrangeOptions).Equals((Int32)options))
                return;

            var upf = Ultraviolet.GetUI().GetPresentationFoundation();
            upf.PerformanceStats.ArrangeCountLastFrame++;

            this.mostRecentArrangeOptions = options;
            this.mostRecentFinalRect = finalRect;

            if (Visibility == Visibility.Collapsed)
            {
                this.renderSize = Size2.Zero;
            }
            else
            {
                isArranging = true;
                try
                {
                    this.renderSize = ArrangeCore(finalRect, options);
                }
                finally
                {
                    isArranging = false;
                }
            }
            this.isArrangeValid = true;

            InvalidatePosition();

            upf.ArrangeQueue.Remove(this);
        }

        /// <summary>
        /// Positions the element in absolute screen space.
        /// </summary>
        /// <param name="position">The position of the element's parent element in absolute screen space.</param>
        public void Position(Point2D position)
        {
            if (View == null)
            {
                this.isPositionValid = true;
                return;
            }

            if (isPositionValid && mostRecentPosition.Equals(position))
                return;

            var upf = Ultraviolet.GetUI().GetPresentationFoundation();
            upf.PerformanceStats.PositionCountLastFrame++;

            this.mostRecentPosition = position;

            var parent = VisualTreeHelper.GetParent(this) as UIElement;
            var contentRegionOffset = (parent == null || parent == Control) ? Point2D.Zero : parent.ContentOffset;

            var offsetX = mostRecentFinalRect.X + RenderOffset.X + contentRegionOffset.X;
            var offsetY = mostRecentFinalRect.Y + RenderOffset.Y + contentRegionOffset.Y;
            var offset  = new Point2D(offsetX, offsetY);

            this.relativeBounds = new RectangleD(offset.X, offset.Y, RenderSize.Width, RenderSize.Height);
            this.absoluteBounds = new RectangleD(position.X + offset.X, position.Y + offset.Y, RenderSize.Width, RenderSize.Height);

            PositionCore(position);
            this.isPositionValid = true;

            Clip();

            upf.PositionQueue.Remove(this);
        }

        /// <summary>
        /// Calculates the clipping rectangle for the element.
        /// </summary>
        public void Clip()
        {
            var clip = ClipCore();
            if (clip.HasValue)
            {
                var clipValue = clip.Value;
                if (clipValue.Width < 0 || clipValue.Height < 0)
                {
                    throw new InvalidOperationException(PresentationStrings.ClipRectangleMustHaveValidDimensions);
                }
            }
            clipRectangle = clip;
        }

        /// <summary>
        /// Invalidates the element's styling state.
        /// </summary>
        public void InvalidateStyle()
        {
            if (View == null || !IsStyleValid || IsStyling)
                return;

            this.isStyleValid = false;
            uv.GetUI().GetPresentationFoundation().StyleQueue.Enqueue(this);
        }

        /// <summary>
        /// Invalidates the element's measurement state.
        /// </summary>
        public void InvalidateMeasure()
        {
            if (View == null || !IsMeasureValid || IsMeasuring)
                return;

            this.isMeasureValid = false;
            uv.GetUI().GetPresentationFoundation().MeasureQueue.Enqueue(this);
        }

        /// <summary>
        /// Invalidates the element's arrangement state.
        /// </summary>
        public void InvalidateArrange()
        {
            if (View == null || !IsArrangeValid || IsArranging)
                return;

            this.isArrangeValid = false;
            uv.GetUI().GetPresentationFoundation().ArrangeQueue.Enqueue(this);
        }

        /// <summary>
        /// Invalidates the element's position state.
        /// </summary>
        public void InvalidatePosition()
        {
            if (View == null || !IsPositionValid)
                return;

            this.isPositionValid = false;
            uv.GetUI().GetPresentationFoundation().PositionQueue.Enqueue(this);
        }

        /// <summary>
        /// Gets the element at the specified pixel coordinates relative to this element's bounds.
        /// </summary>
        /// <param name="x">The element-relative x-coordinate of the pixel to evaluate.</param>
        /// <param name="y">The element-relative y-coordinate of the pixel to evaluate.</param>
        /// <param name="isHitTest">A value indicating whether this test should respect the value of the <see cref="IsHitTestVisible"/> property.</param>
        /// <returns>The element at the specified pixel coordinates, or <c>null</c> if no such element exists.</returns>
        public UIElement GetElementAtPixel(Int32 x, Int32 y, Boolean isHitTest)
        {
            var dipsX = Display.PixelsToDips(x);
            var dipsY = Display.PixelsToDips(y);

            return GetElementAtPoint(dipsX, dipsY, isHitTest);
        }

        /// <summary>
        /// Gets the element at the specified pixel coordinates relative to this element's bounds.
        /// </summary>
        /// <param name="pt">The relative coordinates of the pixel to evaluate.</param>
        /// <param name="isHitTest">A value indicating whether this test should respect the value of the <see cref="IsHitTestVisible"/> property.</param>
        /// <returns>The element at the specified pixel coordinates, or <c>null</c> if no such element exists.</returns>
        public UIElement GetElementAtPixel(Point2 pt, Boolean isHitTest)
        {
            return GetElementAtPixel(pt.X, pt.Y, isHitTest);
        }

        /// <summary>
        /// Gets the element at the specified device-independent coordinates relative to this element's bounds.
        /// </summary>
        /// <param name="x">The element-relative x-coordinate of the point to evaluate.</param>
        /// <param name="y">The element-relative y-coordinate of the point to evaluate.</param>
        /// <param name="isHitTest">A value indicating whether this test should respect the value of the <see cref="IsHitTestVisible"/> property.</param>
        /// <returns>The element at the specified coordinates, or <c>null</c> if no such element exists.</returns>
        public UIElement GetElementAtPoint(Double x, Double y, Boolean isHitTest)
        {
            if (!Bounds.Contains(x, y))
                return null;

            if (!IsEnabled)
                return null;

            if (isHitTest && !IsHitTestVisible)
                return null;

            if (Visibility != Visibility.Visible)
                return null;

            return GetElementAtPointCore(x, y, isHitTest);
        }

        /// <summary>
        /// Gets the element at the specified device-independent coordinates relative to this element's bounds.
        /// </summary>
        /// <param name="pt">The relative coordinates of the point to evaluate.</param>
        /// <param name="isHitTest">A value indicating whether this test should respect the
        /// value of the <see cref="IsHitTestVisible"/> property.</param>
        /// <returns>The element at the specified coordinates, or <c>null</c> if no such element exists.</returns>
        public UIElement GetElementAtPoint(Point2D pt, Boolean isHitTest)
        {
            return GetElementAtPoint(pt.X, pt.Y, isHitTest);
        }

        /// <summary>
        /// Gets the element which is navigated to when focus is moved "up," usually by pressing 
        /// up on the directional pad of a game controller.
        /// </summary>
        /// <returns>The specified element, or <c>null</c> if no such element is defined.</returns>
        public UIElement GetNavUpElement()
        {
            var target = FindNavElement(NavUp);
            if (target == null)
            {
                if (Parent != null)
                {
                    return (Parent.AutoNav ? Parent.GetNextNavUp(this) : null) ?? Parent.GetNavUpElement();
                }
            }
            return target;
        }

        /// <summary>
        /// Gets the element which is navigated to when focus is moved "down," usually by pressing 
        /// down on the directional pad of a game controller.
        /// </summary>
        /// <returns>The specified element, or <c>null</c> if no such element is defined.</returns>
        public UIElement GetNavDownElement()
        {
            var target = FindNavElement(NavDown);
            if (target == null)
            {
                if (Parent != null)
                {
                    return (Parent.AutoNav ? Parent.GetNextNavDown(this) : null) ?? Parent.GetNavDownElement();
                }
            }
            return target;
        }

        /// <summary>
        /// Gets the element which is navigated to when focus is moved "left," usually by pressing 
        /// left on the directional pad of a game controller.
        /// </summary>
        /// <returns>The specified element, or <c>null</c> if no such element is defined.</returns>
        public UIElement GetNavLeftElement()
        {
            var target = FindNavElement(NavLeft);
            if (target == null)
            {
                if (Parent != null)
                {
                    return (Parent.AutoNav ? Parent.GetNextNavLeft(this) : null) ?? Parent.GetNavLeftElement();
                }
            }
            return target;
        }

        /// <summary>
        /// Gets the element which is navigated to when focus is moved "right," usually by pressing 
        /// right on the directional pad of a game controller.
        /// </summary>
        /// <returns>The specified element, or <c>null</c> if no such element is defined.</returns>
        public UIElement GetNavRightElement()
        {
            var target = FindNavElement(NavRight);
            if (target == null)
            {
                if (Parent != null)
                {
                    return (Parent.AutoNav ? Parent.GetNextNavRight(this) : null) ?? Parent.GetNavRightElement();
                }
            }
            return target;
        }

        /// <summary>
        /// Gets the element which is navigated to when focus is moved to the next tab stop.
        /// </summary>
        /// <returns>The specified element, or <c>null</c> if no such element is defined.</returns>
        public UIElement GetNextTabStop()
        {
            return GetNextTabStopInternal();
        }

        /// <summary>
        /// Gets the element which is navigated to when focus is moved to the previous tab stop.
        /// </summary>
        /// <returns>The specified element, or <c>null</c> if no such element is defined.</returns>
        public UIElement GetPreviousTabStop()
        {
            return GetPreviousTabStopInternal();
        }

        /// <summary>
        /// Gets the first focusable element in the part of the logical tree which is rooted in this element.
        /// </summary>
        /// <param name="tabStop">A value indicating whether the matching element must also be a tab stop.</param>
        /// <returns>The first focusable element, or <c>null</c> if no such element exists.</returns>
        public UIElement GetFirstFocusableDescendant(Boolean tabStop)
        {
            var match = GetFirstFocusableDescendantInternal(this, tabStop);
            if (match == null && Focusable && (!tabStop || IsTabStop))
            {
                return this;
            }
            return match;
        }

        /// <summary>
        /// Gets the last focusable element in the part of the logical tree which is rooted in this element.
        /// </summary>
        /// <param name="tabStop">A value indicating whether the element must also be a tab stop.</param>
        /// <returns>The last focusable element, or <c>null</c> if no such element exists.</returns>
        public UIElement GetLastFocusableDescendant(Boolean tabStop)
        {
            var match = GetLastFocusableDescendantInternal(this, tabStop);
            if (match == null && Focusable && (!tabStop || IsTabStop))
            {
                return this;
            }
            return match;
        }

        /// <summary>
        /// Adds a handler for a routed event to the element.
        /// </summary>
        /// <param name="evt">A <see cref="RoutedEvent"/> that identifies the routed event for which to add a handler.</param>
        /// <param name="handler">A delegate that represents the handler to add to the element for the specified routed event.</param>
        public void AddHandler(RoutedEvent evt, Delegate handler)
        {
            Contract.Require(evt, "evt");
            Contract.Require(handler, "handler");

            AddHandler(evt, handler, false);
        }

        /// <summary>
        /// Adds a handler for a routed event to the element.
        /// </summary>
        /// <param name="evt">A <see cref="RoutedEvent"/> that identifies the routed event for which to add a handler.</param>
        /// <param name="handler">A delegate that represents the handler to add to the element for the specified routed event.</param>
        /// <param name="handledEventsToo">A value indicating whether the handler should receive events which have already been handled by other handlers.</param>
        public void AddHandler(RoutedEvent evt, Delegate handler, Boolean handledEventsToo)
        {
            Contract.Require(evt, "evt");
            Contract.Require(handler, "handler");

            routedEventManager.Add(evt, handler, handledEventsToo);
        }

        /// <summary>
        /// Removes a handler for a routed event from the element.
        /// </summary>
        /// <param name="evt">A <see cref="RoutedEvent"/> that identifies the routed event for which to remove a handler.</param>
        /// <param name="handler">A delegate that represents the handler to remove from the element for the specified routed event.</param>
        public void RemoveHandler(RoutedEvent evt, Delegate handler)
        {
            Contract.Require(evt, "evt");
            Contract.Require(handler, "handler");

            routedEventManager.Remove(evt, handler);
        }

        /// <summary>
        /// Gets the Ultraviolet context that created this element.
        /// </summary>
        public UltravioletContext Ultraviolet
        {
            get { return uv; }
        }

        /// <summary>
        /// Gets the collection of styling classes associated with this element.
        /// </summary>
        public UIElementClassCollection Classes
        {
            get { return classes; }
        }

        /// <summary>
        /// Gets the element's unique identifier within its layout.
        /// </summary>
        public String ID
        {
            get { return id; }
        }

        /// <summary>
        /// Gets the name that identifies this element type within the Presentation Foundation.
        /// </summary>
        public String Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the element's associated view.
        /// </summary>
        public PresentationFoundationView View
        {
            get { return view; }
            internal set { view = value; }
        }

        /// <summary>
        /// Gets the element's associated view model.
        /// </summary>
        public Object ViewModel
        {
            get { return view == null ? null : View.ViewModel; }
        }

        /// <summary>
        /// Gets the element's parent element.
        /// </summary>
        public UIElement Parent
        {
            get { return parent; }
            internal set
            {
                if (parent != value)
                {
                    parent = value;
                    OnLogicalParentChangedInternal();
                }
            }
        }

        /// <summary>
        /// Gets the control that owns this element, if this element is a control component.
        /// </summary>
        public Control Control
        {
            get { return control; }
        }

        /// <summary>
        /// Gets a value indicating whether this element is a control component.
        /// </summary>
        public Boolean IsComponent
        {
            get { return control != null; }
        }

        /// <summary>
        /// Gets a value indicating whether the element's styling state is valid.
        /// </summary>
        public Boolean IsStyleValid
        {
            get { return isStyleValid; }
        }

        /// <summary>
        /// Gets a value indicating whether the element's arrangement state is valid.
        /// </summary>
        public Boolean IsArrangeValid
        {
            get { return isArrangeValid; }
        }

        /// <summary>
        /// Gets a value indicating whether the element's measurement state is valid.
        /// </summary>
        public Boolean IsMeasureValid
        {
            get { return isMeasureValid; }
        }

        /// <summary>
        /// Gets a value indicating whether the element's position state is valid.
        /// </summary>
        public Boolean IsPositionValid
        {
            get { return isPositionValid; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether auto-nav is enabled for this control.
        /// If enabled, auto-nav will attempt to automatically determine the next nav control in the
        /// up, down, left, and right directions, even if the values of the <see cref="NavUp"/>, <see cref="NavDown"/>,
        /// <see cref="NavLeft"/>, and <see cref="NavRight"/> properties have not been set.
        /// </summary>
        public Boolean AutoNav
        {
            get { return GetValue<Boolean>(AutoNavProperty); }
            set { SetValue<Boolean>(AutoNavProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the element is enabled.
        /// </summary>
        public Boolean IsEnabled
        {
            get { return GetValue<Boolean>(IsEnabledProperty); }
            set { SetValue<Boolean>(IsEnabledProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether this element has input focus.
        /// </summary>
        public Boolean IsFocused
        {
            get { return (View == null) ? false : View.ElementWithFocus == this; }
        }

        /// <summary>
        /// Gets a value indicating whether the mouse cursor is currently hovering over this element.
        /// </summary>
        public Boolean IsHovering
        {
            get { return isHovering; }
            private set
            {
                if (isHovering != value)
                {
                    isHovering = value;
                    OnIsHoveringChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the element is visible to hit tests.
        /// </summary>
        public Boolean IsHitTestVisible
        {
            get { return GetValue<Boolean>(IsHitTestVisibleProperty); }
            set { SetValue<Boolean>(IsHitTestVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this element participates in tab navigation.
        /// </summary>
        public Boolean IsTabStop
        {
            get { return GetValue<Boolean>(IsTabStopProperty); }
            set { SetValue<Boolean>(IsTabStopProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the element can receive input focus.
        /// </summary>
        public Boolean Focusable
        {
            get { return GetValue<Boolean>(FocusableProperty); }
            set { SetValue<Boolean>(FocusableProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value specifying the element's visibility state.
        /// </summary>
        public Visibility Visibility
        {
            get { return GetValue<Visibility>(VisibilityProperty); }
            set { SetValue<Visibility>(VisibilityProperty, value); }
        }

        /// <summary>
        /// Gets the position of the element in absolute screen coordinates as of the
        /// last call to the <see cref="Position(Point2D)"/> method.
        /// </summary>
        public Point2D AbsolutePosition
        {
            get { return absoluteBounds.Location; }
        }

        /// <summary>
        /// Gets or sets the offset from the top-left corner of the element's layout
        /// area to the top-left corner of the element itself.
        /// </summary>
        public Point2D RenderOffset
        {
            get { return renderOffset; }
            protected set { renderOffset = value; }
        }

        /// <summary>
        /// Gets the final render size of the element.
        /// </summary>
        public Size2D RenderSize
        {
            get
            {
                if (Visibility == Visibility.Collapsed)
                {
                    return Size2D.Zero;
                }
                return renderSize;
            }
        }

        /// <summary>
        /// Gets the element's desired size as calculated during the most recent measure pass.
        /// </summary>
        public Size2D DesiredSize
        {
            get
            {
                if (Visibility == Visibility.Collapsed)
                {
                    return Size2D.Zero;
                }
                return desiredSize;
            }
        }

        /// <summary>
        /// Gets the absolute bounding box of the layout area in which the element was most recently arranged.
        /// </summary>
        public RectangleD AbsoluteLayoutBounds
        {
            get { return mostRecentFinalRect; }
        }

        /// <summary>
        /// Gets the element's bounds in element-local space.
        /// </summary>
        public RectangleD Bounds
        {
            get { return new RectangleD(0, 0, RenderSize.Width, RenderSize.Height); }
        }

        /// <summary>
        /// Gets the element's bounds relative to its parent element.
        /// </summary>
        public RectangleD RelativeBounds
        {
            get { return relativeBounds; }
        }

        /// <summary>
        /// Gets the element's bounds in absolute screen space.
        /// </summary>
        public RectangleD AbsoluteBounds
        {
            get { return absoluteBounds; }
        }

        /// <summary>
        /// Gets the element's clipping rectangle. A value of <c>null</c> indicates that
        /// clipping is disabled for this element.
        /// </summary>
        public RectangleD? ClipRectangle
        {
            get { return clipRectangle; }
        }

        /// <summary>
        /// Gets the offset applied to the region's content. This is usually used to scroll the 
        /// element's content within its content region.
        /// </summary>
        public virtual Point2D ContentOffset
        {
            get { return Point2D.Zero; }
        }

        /// <summary>
        /// Gets or sets the opacity of the element and its children.
        /// </summary>
        public Single Opacity
        {
            get { return GetValue<Single>(OpacityProperty); }
            set { SetValue<Single>(OpacityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the identifier of the element which is navigated to when focus is
        /// moved "up," usually by pressing up on the directional pad of a game controller.
        /// </summary>
        public String NavUp
        {
            get { return GetValue<String>(NavUpProperty); }
            set { SetValue<String>(NavUpProperty, value); }
        }

        /// <summary>
        /// Gets or sets the identifier of the element which is navigated to when focus is
        /// moved "down," usually by pressing down on the directional pad of a game controller.
        /// </summary>
        public String NavDown
        {
            get { return GetValue<String>(NavDownProperty); }
            set { SetValue<String>(NavDownProperty, value); }
        }

        /// <summary>
        /// Gets or sets the identifier of the element which is navigated to when focus is
        /// moved "left," usually by pressing down on the directional pad of a game controller.
        /// </summary>
        public String NavLeft
        {
            get { return GetValue<String>(NavLeftProperty); }
            set { SetValue<String>(NavLeftProperty, value); }
        }

        /// <summary>
        /// Gets or sets the identifier of the element which is navigated to when focus is
        /// moved "right," usually by pressing down on the directional pad of a game controller.
        /// </summary>
        public String NavRight
        {
            get { return GetValue<String>(NavRightProperty); }
            set { SetValue<String>(NavRightProperty, value); }
        }

        /// <summary>
        /// Gets a value which indicates the element's relative position within the tab order
        /// of its parent element.
        /// </summary>
        public Int32 TabIndex
        {
            get { return GetValue<Int32>(TabIndexProperty); }
            set { SetValue<Int32>(TabIndexProperty, value); }
        }

        /// <summary>
        /// Occurs when a class is added to the element.
        /// </summary>
        public event UIElementClassEventHandler ClassAdded;

        /// <summary>
        /// Occurs when a class is removed from the element.
        /// </summary>
        public event UIElementClassEventHandler ClassRemoved;

        /// <summary>
        /// Occurs when the element is being drawn.
        /// </summary>
        public event UIElementDrawingEventHandler Drawing;

        /// <summary>
        /// Occurs when the element is being updated.
        /// </summary>
        public event UIElementUpdatingEventHandler Updating;

        /// <summary>
        /// Occurs when the value of the <see cref="AutoNav"/> property changes.
        /// </summary>
        public event UIElementEventHandler AutoNavChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="IsEnabled"/> property changes.
        /// </summary>
        public event UIElementEventHandler IsEnabledChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="IsHovering"/> property changes.
        /// </summary>
        public event UIElementEventHandler IsHoveringChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="IsHitTestVisible"/> property changes.
        /// </summary>
        public event UIElementEventHandler IsHitTestVisibleChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="IsTabStop"/> property changes.
        /// </summary>
        public event UIElementEventHandler IsTabStopChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="Focusable"/> dependency property changes.
        /// </summary>
        public event UIElementEventHandler FocusableChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="Visibility"/> property changes.
        /// </summary>
        public event UIElementEventHandler VisibilityChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="Opacity"/> property changes.
        /// </summary>
        public event UIElementEventHandler OpacityChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="NavUp"/> property changes.
        /// </summary>
        public event UIElementEventHandler NavUpChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="NavDown"/> property changes.
        /// </summary>
        public event UIElementEventHandler NavDownChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="NavLeft"/> property changes.
        /// </summary>
        public event UIElementEventHandler NavLeftChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="NavRight"/> property changes.
        /// </summary>
        public event UIElementEventHandler NavRightChanged;

        /// <summary>
        /// Occurs when the value of the <see cref="TabIndex"/> property changes.
        /// </summary>
        public event UIElementEventHandler TabIndexChanged;

        /// <summary>
        /// Identifies the <see cref="AutoNav"/> dependency property.
        /// </summary>
        [Styled("autonav")]
        public static readonly DependencyProperty AutoNavProperty = DependencyProperty.Register("AutoNav", typeof(Boolean), typeof(UIElement),
            new DependencyPropertyMetadata(HandleAutoNavChanged, () => true, DependencyPropertyOptions.None));

        /// <summary>
        /// Identifies the <see cref="IsEnabled"/> dependency property.
        /// </summary>
        [Styled("enabled")]
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(Boolean), typeof(UIElement),
            new DependencyPropertyMetadata(HandleIsEnabledChanged, () => true, DependencyPropertyOptions.None));

        /// <summary>
        /// Identifies the <see cref="IsHitTestVisible"/> dependency property.
        /// </summary>
        [Styled("hit-test-visible")]
        public static readonly DependencyProperty IsHitTestVisibleProperty = DependencyProperty.Register("IsHitTestVisible", typeof(Boolean), typeof(UIElement),
            new DependencyPropertyMetadata(HandleIsHitTestVisibleChanged, () => true, DependencyPropertyOptions.None));

        /// <summary>
        /// Identifies the <see cref="IsTabStop"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTabStopProperty = DependencyProperty.Register("IsTabStop", typeof(Boolean), typeof(UIElement),
            new DependencyPropertyMetadata(HandleIsTabStopChanged, () => true, DependencyPropertyOptions.None));

        /// <summary>
        /// Identifies the <see cref="Focusable"/> dependency property.
        /// </summary>
        [Styled("focusable")]
        public static readonly DependencyProperty FocusableProperty = DependencyProperty.Register("Focusable", typeof(Boolean), typeof(UIElement),
            new DependencyPropertyMetadata(HandleFocusableChanged, () => false, DependencyPropertyOptions.None));

        /// <summary>
        /// Identifies the <see cref="Visibility"/> dependency property.
        /// </summary>
        [Styled("visibility")]
        public static readonly DependencyProperty VisibilityProperty = DependencyProperty.Register("Visibility", typeof(Visibility), typeof(UIElement),
            new DependencyPropertyMetadata(HandleVisibilityChanged, () => Visibility.Visible, DependencyPropertyOptions.AffectsMeasure));

        /// <summary>
        /// Identifies the <see cref="Opacity"/> dependency property.
        /// </summary>
        [Styled("opacity")]
        public static readonly DependencyProperty OpacityProperty = DependencyProperty.Register("Opacity", typeof(Single), typeof(UIElement),
            new DependencyPropertyMetadata(HandleOpacityChanged, () => 1.0f, DependencyPropertyOptions.None));

        /// <summary>
        /// Identifies the <see cref="NavUp"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NavUpProperty = DependencyProperty.Register("NavUp", typeof(String), typeof(UIElement),
            new DependencyPropertyMetadata(HandleNavUpChanged, () => null, DependencyPropertyOptions.None));

        /// <summary>
        /// Identifies the <see cref="NavDown"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NavDownProperty = DependencyProperty.Register("NavDown", typeof(String), typeof(UIElement),
            new DependencyPropertyMetadata(HandleNavDownChanged, () => null, DependencyPropertyOptions.None));

        /// <summary>
        /// Identifies the <see cref="NavLeft"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NavLeftProperty = DependencyProperty.Register("NavLeft", typeof(String), typeof(UIElement),
            new DependencyPropertyMetadata(HandleNavLeftChanged, () => null, DependencyPropertyOptions.None));

        /// <summary>
        /// Identifies the <see cref="NavRight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NavRightProperty = DependencyProperty.Register("NavRight", typeof(String), typeof(UIElement),
            new DependencyPropertyMetadata(HandleNavRightChanged, () => null, DependencyPropertyOptions.None));

        /// <summary>
        /// Identifies the <see cref="TabIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TabIndexProperty = DependencyProperty.Register("TabIndex", typeof(Int32), typeof(UIElement),
            new DependencyPropertyMetadata(HandleTabIndexChanged, () => 0, DependencyPropertyOptions.None));

        /// <summary>
        /// Applies a visual state transition to the element.
        /// </summary>
        /// <param name="style">The style which defines the state transition.</param>
        internal virtual void ApplyStyledVisualStateTransition(UvssStyle style)
        {

        }

        /// <summary>
        /// Called when something occurs which requires the element to invalidate its cached
        /// layout information, such as the visual or logical parent changing.
        /// </summary>
        internal virtual void OnLayoutCacheInvalidatedInternal()
        {
            CacheLayoutParameters();
            InvalidateStyle();

            OnLayoutCacheInvalidated();
        }

        /// <summary>
        /// Invokes the <see cref="OnLogicalParentChanged()"/> method.
        /// </summary>
        internal virtual void OnLogicalParentChangedInternal()
        {
            OnLayoutCacheInvalidatedInternal();
            OnLogicalParentChanged();
        }

        /// <inheritdoc/>
        internal override void OnVisualParentChangedInternal()
        {
            OnLayoutCacheInvalidatedInternal();
            base.OnVisualParentChangedInternal();
        }

        /// <summary>
        /// Changes the element's logical and visual parents.
        /// </summary>
        /// <param name="logicalParent">The element's new logical parent.</param>
        /// <param name="visualParent">The element's new visual parent.</param>
        internal void ChangeLogicalAndVisualParents(UIElement logicalParent, Visual visualParent)
        {
            if (this.Parent != null)
                this.Parent = null;

            this.Parent = logicalParent;

            if (this.VisualParent != null)
                this.VisualParent.RemoveVisualChild(this);

            if (visualParent != null)
                visualParent.AddVisualChild(this);
        }

        /// <summary>
        /// Changes the element's logical parent.
        /// </summary>
        /// <param name="logicalParent">The element's new logical parent.</param>
        internal void ChangeLogicalParent(UIElement logicalParent)
        {
            if (this.Parent != null)
                this.Parent = null;

            this.Parent = logicalParent;
        }

        /// <summary>
        /// Changes the element's visual parent.
        /// </summary>
        /// <param name="visualParent">The element's new visual parent.</param>
        internal void ChangeVisualParent(Visual visualParent)
        {
            if (this.VisualParent != null)
                this.VisualParent.RemoveVisualChild(this);

            if (visualParent != null)
                visualParent.AddVisualChild(this);
        }

        /// <summary>
        /// Invalidates the element's style.
        /// </summary>
        internal void InvalidateStyleInternal()
        {
            isStyleValid = false;
        }

        /// <summary>
        /// Invalidates the element's measure.
        /// </summary>
        internal void InvalidateMeasureInternal()
        {
            isMeasureValid = false;
        }

        /// <summary>
        /// Invalidates the element's arrangement.
        /// </summary>
        internal void InvalidateArrangeInternal()
        {
            isArrangeValid = false;
        }

        /// <summary>
        /// Invalidates the element's position.
        /// </summary>
        internal void InvalidatePositionInternal()
        {
            isPositionValid = false;
        }

        /// <summary>
        /// Searches the object for a dependency property which matches the specified name.
        /// </summary>
        /// <param name="name">The name of the dependency property for which to search.</param>
        /// <returns>The <see cref="DependencyProperty"/> instance which matches the specified name, or <c>null</c> if no
        /// such property exists on this object.</returns>
        internal DependencyProperty FindDependencyPropertyByName(DependencyPropertyName name)
        {
            if (name.IsAttachedProperty)
            {
                if (Parent != null && String.Equals(Parent.Name, name.Container, StringComparison.OrdinalIgnoreCase))
                {
                    return DependencyProperty.FindByName(name.Name, Parent.GetType());
                }
                return null;
            }
            return DependencyProperty.FindByName(name.Name, GetType());
        }

        /// <summary>
        /// Finds a styled dependency property according to its styling name.
        /// </summary>
        /// <param name="name">The styling name of the dependency property to retrieve.</param>
        /// <returns>The <see cref="DependencyProperty"/> instance which matches the specified styling name, or <c>null</c> if no
        /// such dependency property exists on this object.</returns>
        internal DependencyProperty FindStyledDependencyProperty(String name)
        {
            Contract.RequireNotEmpty(name, "name");

            return FindStyledDependencyProperty(name, GetType());
        }

        /// <summary>
        /// Begins the specified storyboard on this element.
        /// </summary>
        /// <param name="storyboard">The storyboard to begin on this element.</param>
        internal void BeginStoryboard(Storyboard storyboard)
        {
            StoryboardClock existingClock;
            storyboardClocks.TryGetValue(storyboard, out existingClock);

            var clock = StoryboardClockPool.Instance.Retrieve(storyboard);
            storyboardClocks[storyboard] = clock;

            Animate(storyboard, clock, this);

            clock.Start();

            if (existingClock != null)
            {
                existingClock.Stop();
                StoryboardClockPool.Instance.Release(existingClock);
            }
        }

        /// <summary>
        /// Stops the specified storyboard on this element.
        /// </summary>
        /// <param name="storyboard">The storyboard to stop on this element.</param>
        internal void StopStoryboard(Storyboard storyboard)
        {
            StoryboardClock clock;
            if (storyboardClocks.TryGetValue(storyboard, out clock))
            {
                clock.Stop();
                storyboardClocks.Remove(storyboard);
                StoryboardClockPool.Instance.Release(clock);
            }
        }

        /// <summary>
        /// Gets the element's list of event handlers for the specified routed event.
        /// </summary>
        /// <param name="evt">A <see cref="RoutedEvent"/> that identifies the routed event for which to retrieve handlers.</param>
        /// <returns>The element's internal list of event handlers for the specified routed event.</returns>
        internal List<RoutedEventHandlerMetadata> GetHandlers(RoutedEvent evt)
        {
            return routedEventManager.GetHandlers(evt);
        }

        /// <summary>
        /// Gets the stylesheet that was most recently passed to the <see cref="Style(UvssDocument)"/> method.
        /// </summary>
        internal UvssDocument MostRecentStylesheet
        {
            get { return mostRecentStylesheet; }
        }

        /// <summary>
        /// Gets the arrangement options that were most recently passed to the <see cref="Arrange(RectangleD, ArrangeOptions)"/> method.
        /// </summary>
        internal ArrangeOptions MostRecentArrangeOptions
        {
            get { return mostRecentArrangeOptions; }
        }

        /// <summary>
        /// Gets the final rectangle that was most recently passed to the <see cref="Arrange(RectangleD, ArrangeOptions)"/> method.
        /// </summary>
        internal RectangleD MostRecentFinalRect
        {
            get { return mostRecentFinalRect; }
        }

        /// <summary>
        /// Gets the available size that was most recently passed to the <see cref="Measure(Size2D)"/> method.
        /// </summary>
        internal Size2D MostRecentAvailableSize
        {
            get { return mostRecentAvailableSize; }
        }

        /// <summary>
        /// Gets the position that was most recently passed to the <see cref="Position(Point2D)"/> method.
        /// </summary>
        internal Point2D MostRecentPosition
        {
            get { return mostRecentPosition; }
        }

        /// <summary>
        /// Gets the element's depth within the layout tree.
        /// </summary>
        internal Int32 LayoutDepth
        {
            get { return layoutDepth; }
        }

        /// <summary>
        /// Gets a value indicating whether the element is in the process of being styled.
        /// </summary>
        internal Boolean IsStyling
        {
            get { return isStyling; }
        }

        /// <summary>
        /// Gets a value indicating whether the element is in the process of being measured.
        /// </summary>
        internal Boolean IsMeasuring
        {
            get { return isMeasuring; }
        }

        /// <summary>
        /// Gets a value indicating whether the element is in the process of being arranged.
        /// </summary>
        internal Boolean IsArranging
        {
            get { return isArranging; }
        }

        /// <inheritdoc/>
        protected internal sealed override void OnMeasureAffectingPropertyChanged()
        {
            InvalidateMeasure();
            base.OnMeasureAffectingPropertyChanged();
        }

        /// <inheritdoc/>
        protected internal sealed override void OnArrangeAffectingPropertyChanged()
        {
            InvalidateArrange();
            base.OnMeasureAffectingPropertyChanged();
        }

        /// <inheritdoc/>
        protected internal sealed override void OnPositionAffectingPropertyChanged()
        {
            InvalidatePosition();
            base.OnPositionAffectingPropertyChanged();
        }

        /// <summary>
        /// Applies the specified stylesheet's styles to this element and its children.
        /// </summary>
        /// <param name="stylesheet">The stylesheet to apply to the element.</param>
        protected internal sealed override void ApplyStyles(UvssDocument stylesheet)
        {
            stylesheet.ApplyStyles(this);
        }

        /// <summary>
        /// Applies a style to the element.
        /// </summary>
        /// <param name="style">The style which is being applied.</param>
        /// <param name="selector">The selector which caused the style to be applied.</param>
        /// <param name="attached">A value indicating whether thie style represents an attached property.</param>
        protected internal sealed override void ApplyStyle(UvssStyle style, UvssSelector selector, Boolean attached)
        {
            Contract.Require(style, "style");
            Contract.Require(selector, "selector");

            var name = style.Name;
            if (name == "transition")
            {
                ApplyStyledVisualStateTransition(style);
            }
            else
            {
                var setter = attached ? Parent.GetStyleSetter(name, selector.PseudoClass) : GetStyleSetter(name, selector.PseudoClass);
                if (setter == null)
                    return;

                setter(this, style, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Raises the <see cref="ClassAdded"/> event.
        /// </summary>
        /// <param name="classname">The name of the class that was added to the element.</param>
        protected internal virtual void OnClassAdded(String classname)
        {
            var temp = ClassAdded;
            if (temp != null)
            {
                temp(this, classname);
            }
        }

        /// <summary>
        /// Raises the <see cref="ClassRemoved"/> event.
        /// </summary>
        /// <param name="classname">The name of the class that was removed from the element.</param>
        protected internal virtual void OnClassRemoved(String classname)
        {
            var temp = ClassRemoved;
            if (temp != null)
            {
                temp(this, classname);
            }
        }

        /// <summary>
        /// Called when the desired size of one of the element's children is changed.
        /// </summary>
        /// <param name="child">The child element that was resized.</param>
        protected virtual void OnChildDesiredSizeChanged(UIElement child)
        {
            if (IsMeasureValid)
            {
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Invoked when a <see cref="Keyboard.GotKeyboardFocusEvent"/> attached routed event occurs.
        /// </summary>
        /// <param name="handled">A value indicating whether the event has been handled.</param>
        protected virtual void OnGotKeyboardFocus(ref Boolean handled)
        {

        }

        /// <summary>
        /// Invoked when a <see cref="Keyboard.LostKeyboardFocusEvent"/> attached routed event occurs.
        /// </summary>
        /// <param name="handled">A value indicating whether the event has been handled.</param>
        protected virtual void OnLostKeyboardFocus(ref Boolean handled)
        {

        }

        /// <summary>
        /// Invoked when a <see cref="Keyboard.KeyDownEvent"/> attached routed event occurs.
        /// </summary>
        /// <param name="device">The <see cref="KeyboardDevice"/> that raised the event.</param>
        /// <param name="key">The <see cref="Key"/> value that represents the key that was pressed.</param>
        /// <param name="modifiers">A <see cref="KeyModifiers"/> value indicating which of the key modifiers are currently active.</param>
        /// <param name="handled">A value indicating whether the event has been handled.</param>
        protected virtual void OnKeyDown(KeyboardDevice device, Key key, KeyModifiers modifiers, ref Boolean handled)
        {

        }

        /// <summary>
        /// Invoked when a <see cref="Keyboard.KeyUpEvent"/> attached routed event occurs.
        /// </summary>
        /// <param name="device">The <see cref="KeyboardDevice"/> that raised the event.</param>
        /// <param name="key">The <see cref="Key"/> value that represents the key that was pressed.</param>
        /// <param name="handled">A value indicating whether the event has been handled.</param>
        protected virtual void OnKeyUp(KeyboardDevice device, Key key, ref Boolean handled)
        {

        }

        /// <summary>
        /// Invoked when a <see cref="Keyboard.TextInputEvent"/> attached routed event occurs.
        /// </summary>
        /// <param name="device">The <see cref="KeyboardDevice"/> that raised the event.</param>
        /// <param name="handled">A value indicating whether the event has been handled.</param>
        protected virtual void OnTextInput(KeyboardDevice device, ref Boolean handled)
        {

        }

        /// <summary>
        /// Invoked by the <see cref="Mouse.GotMouseCaptureEvent"/> attached routed event.
        /// </summary>
        /// <param name="handled">A value indicating whether the event has been handled.</param>
        protected virtual void OnGotMouseCapture(ref Boolean handled)
        {

        }

        /// <summary>
        /// Invoked by the <see cref="Mouse.LostMouseCaptureEvent"/> attached routed event.
        /// </summary>
        /// <param name="handled">A value indicating whether the event has been handled.</param>
        protected virtual void OnLostMouseCapture(ref Boolean handled)
        {

        }

        /// <summary>
        /// Invoked by the <see cref="Mouse.MouseMoveEvent"/> attached routed event.
        /// </summary>
        /// <param name="device">The mouse device.</param>
        /// <param name="x">The x-coordinate of the cursor in device-independent screen coordinates.</param>
        /// <param name="y">The y-coordinate of the cursor in device-independent screen coordinates.</param>
        /// <param name="dx">The difference between the x-coordinate of the mouse's 
        /// current position and the x-coordinate of the mouse's previous position.</param>
        /// <param name="dy">The difference between the y-coordinate of the mouse's 
        /// current position and the y-coordinate of the mouse's previous position.</param>
        /// <param name="handled">A value indicating whether the event has been handled.</param>
        protected virtual void OnMouseMove(MouseDevice device, Double x, Double y, Double dx, Double dy, ref Boolean handled)
        {

        }

        /// <summary>
        /// Invoked by the <see cref="Mouse.MouseEnterEvent"/> attached routed event.
        /// </summary>
        /// <param name="device">The mouse device.</param>
        /// <param name="handled">A value indicating whether the event has been handled.</param>
        protected virtual void OnMouseEnter(MouseDevice device, ref Boolean handled)
        {

        }

        /// <summary>
        /// Invoked by the <see cref="Mouse.MouseLeaveEvent"/> attached routed event.
        /// </summary>
        /// <param name="device">The mouse device.</param>
        /// <param name="handled">A value indicating whether the event has been handled.</param>
        protected virtual void OnMouseLeave(MouseDevice device, ref Boolean handled)
        {

        }

        /// <summary>
        /// Invoked by the <see cref="Mouse.MouseUpEvent"/> attached routed event.
        /// </summary>
        /// <param name="device">The mouse device.</param>
        /// <param name="button">The mouse button that was pressed or released.</param>
        /// <param name="handled">A value indicating whether the event has been handled.</param>
        protected virtual void OnMouseUp(MouseDevice device, MouseButton button, ref Boolean handled)
        {

        }

        /// <summary>
        /// Invoked by the <see cref="Mouse.MouseDownEvent"/> attached routed event.
        /// </summary>
        /// <param name="device">The mouse device.</param>
        /// <param name="button">The mouse button that was pressed or released.</param>
        /// <param name="handled">A value indicating whether the event has been handled.</param>
        protected virtual void OnMouseDown(MouseDevice device, MouseButton button, ref Boolean handled)
        {

        }

        /// <summary>
        /// Invoked by the <see cref="Mouse.MouseClickEvent"/> attached routed event.
        /// </summary>
        /// <param name="device">The mouse device.</param>
        /// <param name="button">The mouse button that was pressed or released.</param>
        /// <param name="handled">A value indicating whether the event has been handled.</param>
        protected virtual void OnMouseClick(MouseDevice device, MouseButton button, ref Boolean handled)
        {

        }

        /// <summary>
        /// Invoked by the <see cref="Mouse.MouseDoubleClickEvent"/> attached routed event.
        /// </summary>
        /// <param name="device">The mouse device.</param>
        /// <param name="button">The mouse button that was pressed or released.</param>
        /// <param name="handled">A value indicating whether the event has been handled.</param>
        protected virtual void OnMouseDoubleClick(MouseDevice device, MouseButton button, ref Boolean handled)
        {

        }

        /// <summary>
        /// Invoked by the <see cref="Mouse.MouseWheelEvent"/> attached routed event.
        /// </summary>
        /// <param name="device">The mouse device.</param>
        /// <param name="x">The amount that the wheel was scrolled along the x-axis.</param>
        /// <param name="y">The amount that the wheel was scrolled along the y-axis.</param>
        /// <param name="handled">A value indicating whether the event has been handled.</param>
        protected virtual void OnMouseWheel(MouseDevice device, Double x, Double y, ref Boolean handled)
        {

        }

        /// <summary>
        /// Removes the specified child element from this element.
        /// </summary>
        /// <param name="child">The child element to remove from this element.</param>
        protected internal virtual void RemoveLogicalChild(UIElement child)
        {

        }

        /// <inheritdoc/>
        protected internal sealed override Object DependencyDataSource
        {
            get { return IsComponent ? Control : ViewModel; }
        }

        /// <inheritdoc/>
        protected internal sealed override DependencyObject DependencyContainer
        {
            get { return VisualTreeHelper.GetParent(this); }
        }

        /// <summary>
        /// Raises the <see cref="Drawing"/> event.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="UltravioletContext.Draw(UltravioletTime)"/>.</param>
        /// <param name="dc">The drawing context that describes the render state of the layout.</param>
        protected virtual void OnDrawing(UltravioletTime time, DrawingContext dc)
        {
            var temp = Drawing;
            if (temp != null)
            {
                temp(this, time, dc);
            }
        }

        /// <summary>
        /// Raises the <see cref="Updating"/> event.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="UltravioletContext.Update(UltravioletTime)"/>.</param>
        protected virtual void OnUpdating(UltravioletTime time)
        {
            var temp = Updating;
            if (temp != null)
            {
                temp(this, time);
            }
        }

        /// <summary>
        /// Raises the <see cref="AutoNavChanged"/> event.
        /// </summary>
        protected virtual void OnAutoNavChanged()
        {
            var temp = AutoNavChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Occurs when something happens which requires the element to invalidate any
        /// cached layout information, such as changing the element's visual or logical parent.
        /// </summary>
        protected virtual void OnLayoutCacheInvalidated()
        {

        }

        /// <summary>
        /// Occurs when the element's logical parent is changed.
        /// </summary>
        protected virtual void OnLogicalParentChanged()
        {
            CacheLayoutParameters();
            InvalidateStyle();
        }

        /// <summary>
        /// Raises the <see cref="IsEnabledChanged"/> event.
        /// </summary>
        protected virtual void OnIsEnabledChanged()
        {
            var temp = IsEnabledChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Raises the <see cref="IsHoveringChanged"/> event.
        /// </summary>
        protected virtual void OnIsHoveringChanged()
        {
            var temp = IsHoveringChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Raises the <see cref="IsHitTestVisibleChanged"/> event.
        /// </summary>
        protected virtual void OnIsHitTestVisibleChanged()
        {
            var temp = IsHitTestVisibleChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Raises the <see cref="IsTabStopChanged"/> event.
        /// </summary>
        protected virtual void OnIsTabStopChanged()
        {
            var temp = IsTabStopChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Raises the <see cref="FocusableChanged"/> event.
        /// </summary>
        protected virtual void OnFocusableChanged()
        {
            var temp = FocusableChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Raises the <see cref="VisibilityChanged"/> event.
        /// </summary>
        protected virtual void OnVisibilityChanged()
        {
            var temp = VisibilityChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Raises the <see cref="OpacityChanged"/> event.
        /// </summary>
        protected virtual void OnOpacityChanged()
        {
            var temp = OpacityChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Raises the <see cref="NavUpChanged"/> event.
        /// </summary>
        protected virtual void OnNavUpChanged()
        {
            var temp = NavUpChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Raises the <see cref="NavDownChanged"/> event.
        /// </summary>
        protected virtual void OnNavDownChanged()
        {
            var temp = NavDownChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Raises the <see cref="NavLeftChanged"/> event.
        /// </summary>
        protected virtual void OnNavLeftChanged()
        {
            var temp = NavLeftChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Raises the <see cref="NavRightChanged"/> event.
        /// </summary>
        protected virtual void OnNavRightChanged()
        {
            var temp = NavRightChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// Raises the <see cref="TabIndexChanged"/> event.
        /// </summary>
        protected virtual void OnTabIndexChanged()
        {
            var temp = TabIndexChanged;
            if (temp != null)
            {
                temp(this);
            }
        }

        /// <summary>
        /// When overridden in a derived class, draws the element using the specified <see cref="SpriteBatch"/>.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="UltravioletContext.Draw(UltravioletTime)"/>.</param>
        /// <param name="dc">The drawing context that describes the render state of the layout.</param>
        protected virtual void DrawCore(UltravioletTime time, DrawingContext dc)
        {

        }

        /// <summary>
        /// When overridden in a derived class, updates the element's state.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="UltravioletContext.Update(UltravioletTime)"/>.</param>
        protected virtual void UpdateCore(UltravioletTime time)
        {

        }

        /// <summary>
        /// When overridden in a derived class, initializes the element's dependency
        /// properties and the dependency properties of any children of this element.
        /// </summary>
        /// <param name="recursive">A value indicating whether to clear the dependency
        /// properties of this element's child elements.</param>
        protected virtual void InitializeDependencyPropertiesCore(Boolean recursive)
        {
            ((DependencyObject)this).InitializeDependencyProperties();

            VisualTreeHelper.ForEachChild<UIElement>(this, CommonBoxedValues.Boolean.FromValue(recursive), (child, state) =>
            {
                child.InitializeDependencyProperties((Boolean)state);
            });
        }

        /// <summary>
        /// When overridden in a derived class, reloads this element's content 
        /// and the content of any children of this element.
        /// </summary>
        protected virtual void ReloadContentCore(Boolean recursive)
        {
            VisualTreeHelper.ForEachChild<UIElement>(this, CommonBoxedValues.Boolean.FromValue(recursive), (child, state) =>
            {
                child.ReloadContent((Boolean)state);
            });
        }

        /// <summary>
        /// When overridden in a derived class, clears the animations which are attached to 
        /// this element, and optionally any animations attached to children of this element.
        /// </summary>
        /// <param name="recursive">A value indicating whether to clear the animations
        /// of this element's child elements.</param>
        protected virtual void ClearAnimationsCore(Boolean recursive)
        {
            ((DependencyObject)this).ClearAnimations();

            VisualTreeHelper.ForEachChild<UIElement>(this, CommonBoxedValues.Boolean.FromValue(recursive), (child, state) =>
            {
                child.ClearAnimations((Boolean)state);
            });
        }

        /// <summary>
        /// When overridden in a derived class, clears the local values of this element's 
        /// dependency properties, and optionally the local values of any dependency properties belonging
        /// to children of this element.
        /// </summary>
        /// <param name="recursive">A value indicating whether to clear the local dependency
        /// property values of this element's child elements.</param>
        protected virtual void ClearLocalValuesCore(Boolean recursive)
        {
            ((DependencyObject)this).ClearLocalValues();

            VisualTreeHelper.ForEachChild<UIElement>(this, CommonBoxedValues.Boolean.FromValue(recursive), (child, state) =>
            {
                child.ClearLocalValues((Boolean)state);
            });
        }

        /// <summary>
        /// When overridden in a derived class, clears the styled values of this element's 
        /// dependency properties, and optionally the styled values of any dependency properties belonging
        /// to children of this element.
        /// </summary>
        /// <param name="recursive">A value indicating whether to clear the styled dependency
        /// property values of this element's child elements.</param>
        protected virtual void ClearStyledValuesCore(Boolean recursive)
        {
            ((DependencyObject)this).ClearStyledValues();

            VisualTreeHelper.ForEachChild<UIElement>(this, CommonBoxedValues.Boolean.FromValue(recursive), (child, state) =>
            {
                child.ClearStyledValues((Boolean)state);
            });
        }

        /// <summary>
        /// When overridden in a derived class, performs cleanup operations and releases any 
        /// internal framework resources for this element and any child elements.
        /// </summary>
        protected virtual void CleanupCore()
        {
            VisualTreeHelper.ForEachChild<UIElement>(this, null, (child, state) =>
            {
                child.Cleanup();
            });
        }

        /// <summary>
        /// When overridden in a derived class, caches layout parameters related to the
        /// element's position within the element hierarchy for this element and for
        /// any child elements.
        /// </summary>
        protected virtual void CacheLayoutParametersCore()
        {
            VisualTreeHelper.ForEachChild<UIElement>(this, null, (child, state) =>
            {
                child.CacheLayoutParameters();
            });
        }

        /// <summary>
        /// When overridden in a derived class, animates this element using the specified storyboard.
        /// </summary>
        /// <param name="storyboard">The storyboard being applied to the element.</param>
        /// <param name="clock">The storyboard clock that tracks playback.</param>
        /// <param name="root">The root element to which the storyboard is being applied.</param>
        protected virtual void AnimateCore(Storyboard storyboard, StoryboardClock clock, UIElement root)
        {
            var children = VisualTreeHelper.GetChildrenCount(this);
            for (int i = 0; i < children; i++)
            {
                var child = VisualTreeHelper.GetChild(this, i) as UIElement;
                if (child != null)
                {
                    child.Animate(storyboard, clock, root);
                }
            }
        }

        /// <summary>
        /// When overridden in a derived class, applies the specified stylesheet
        /// to this element and to any child elements.
        /// </summary>
        /// <param name="stylesheet">The stylesheet to apply to this element and its children.</param>
        protected virtual void StyleCore(UvssDocument stylesheet)
        {
            VisualTreeHelper.ForEachChild<UIElement>(this, stylesheet, (child, state) =>
            {
                child.Style((UvssDocument)state);
            });
        }

        /// <summary>
        /// When overridden in a derived class, calculates the element's desired size and 
        /// the desired sizes of any child elements.
        /// </summary>
        /// <param name="availableSize">The size of the area which the element's parent has 
        /// specified is available for the element's layout.</param>
        /// <returns>The element's desired size, considering the size of any content elements.</returns>
        protected virtual Size2D MeasureCore(Size2D availableSize)
        {
            return Size2D.Zero;
        }

        /// <summary>
        /// When overridden in a derived class, sets the element's final area relative to its 
        /// parent and arranges the element's children within its layout area.
        /// </summary>
        /// <param name="finalRect">The element's final position and size relative to its parent element.</param>
        /// <param name="options">A set of <see cref="ArrangeOptions"/> values specifying the options for this arrangement.</param>
        protected virtual Size2D ArrangeCore(RectangleD finalRect, ArrangeOptions options)
        {
            return new Size2D(finalRect.Width, finalRect.Height);
        }

        /// <summary>
        /// When overridden in a derived class, positions the element in absolute screen space.
        /// </summary>
        /// <param name="position">The position of the element's parent element in absolute screen space.</param>
        protected virtual void PositionCore(Point2D position)
        {

        }

        /// <summary>
        /// When overridden in a derived class, calculates the clipping rectangle for this element.
        /// </summary>
        /// <returns>The clipping rectangle for this element in absolute screen coordinates, or <c>null</c> to disable clipping.</returns>
        protected virtual RectangleD? ClipCore()
        {
            return null;
        }

        /// <summary>
        /// When overridden in a derived class, gets the element at the specified device-independent 
        /// coordinates relative to this element's bounds.
        /// </summary>
        /// <param name="x">The element-relative x-coordinate of the point to evaluate.</param>
        /// <param name="y">The element-relative y-coordinate of the point to evaluate.</param>
        /// <param name="isHitTest">A value indicating whether this test should respect the value of the <see cref="IsHitTestVisible"/> property.</param>
        /// <returns>The element at the specified coordinates, or <c>null</c> if no such element exists.</returns>
        protected virtual UIElement GetElementAtPointCore(Double x, Double y, Boolean isHitTest)
        {
            if (!Bounds.Contains(x, y))
                return null;

            var children = VisualTreeHelper.GetChildrenCount(this);
            for (int i = children - 1; i >= 0; i--)
            {
                var child = VisualTreeHelper.GetChild(this, i) as UIElement;
                if (child == null)
                    continue;

                var childRelativeX = x - child.RelativeBounds.X;
                var childRelativeY = y - child.RelativeBounds.Y;

                var childMatch = child.GetElementAtPoint(childRelativeX, childRelativeY, isHitTest);
                if (childMatch != null)
                {
                    return childMatch;
                }
            }

            return this;
        }

        /// <summary>
        /// Gets the next element to navigate to when focus is moved "up," assuming
        /// that focus is currently in the specified child element.
        /// </summary>
        /// <param name="current">The child element of this element which currently has focus.</param>
        /// <returns>The next element to navigate to, or <c>null</c> if this element has no navigation preferences.</returns>
        protected virtual UIElement GetNextNavUp(UIElement current)
        {
            return null;
        }

        /// <summary>
        /// Gets the next element to navigate to when focus is moved "down," assuming
        /// that focus is currently in the specified child element.
        /// </summary>
        /// <param name="current">The child element of this element which currently has focus.</param>
        /// <returns>The next element to navigate to, or <c>null</c> if this element has no navigation preferences.</returns>
        protected virtual UIElement GetNextNavDown(UIElement current)
        {
            return null;
        }

        /// <summary>
        /// Gets the next element to navigate to when focus is moved "left," assuming
        /// that focus is currently in the specified child element.
        /// </summary>
        /// <param name="current">The child element of this element which currently has focus.</param>
        /// <returns>The next element to navigate to, or <c>null</c> if this element has no navigation preferences.</returns>
        protected virtual UIElement GetNextNavLeft(UIElement current)
        {
            return null;
        }

        /// <summary>
        /// Gets the next element to navigate to when focus is moved "right," assuming
        /// that focus is currently in the specified child element.
        /// </summary>
        /// <param name="current">The child element of this element which currently has focus.</param>
        /// <returns>The next element to navigate to, or <c>null</c> if this element has no navigation preferences.</returns>
        protected virtual UIElement GetNextNavRight(UIElement current)
        {
            return null;
        }

        /// <summary>
        /// Loads the specified asset from the global content manager.
        /// </summary>
        /// <typeparam name="TOutput">The type of object being loaded.</typeparam>
        /// <param name="asset">The identifier of the asset to load.</param>
        /// <returns>The asset that was loaded.</returns>
        protected TOutput LoadGlobalContent<TOutput>(AssetID asset)
        {
            if (View == null)
                return default(TOutput);

            return View.LoadLocalContent<TOutput>(asset);
        }

        /// <summary>
        /// Loads the specified asset from the local content manager.
        /// </summary>
        /// <typeparam name="TOutput">The type of object being loaded.</typeparam>
        /// <param name="asset">The identifier of the asset to load.</param>
        /// <returns>The asset that was loaded.</returns>
        protected TOutput LoadLocalContent<TOutput>(AssetID asset)
        {
            if (View == null)
                return default(TOutput);

            return View.LoadLocalContent<TOutput>(asset);
        }

        /// <summary>
        /// Loads the specified image from the global content manager.
        /// </summary>
        /// <param name="image">The image to load.</param>
        protected void LoadGlobalImage<T>(T image) where T : TextureImage
        {
            if (View == null)
                return;

            View.LoadGlobalImage(image);
        }

        /// <summary>
        /// Loads the specified image from the local content manager.
        /// </summary>
        /// <param name="image">The image to load.</param>
        protected void LoadLocalImage<T>(T image) where T : TextureImage
        {
            if (View == null)
                return;

            View.LoadLocalImage(image);
        }

        /// <summary>
        /// Loads the specified resource from the global content manager.
        /// </summary>
        /// <param name="resource">The resource to load.</param>
        /// <param name="asset">The asset identifier that specifies which resource to load.</param>
        protected void LoadGlobalResource<T>(FrameworkResource<T> resource, AssetID asset) where T : class
        {
            if (View == null)
                return;

            View.LoadGlobalResource(resource, asset);
        }

        /// <summary>
        /// Loads the specified resource from the local content manager.
        /// </summary>
        /// <param name="resource">The resource to load.</param>
        /// <param name="asset">The asset identifier that specifies which resource to load.</param>
        protected void LoadLocalResource<T>(FrameworkResource<T> resource, AssetID asset) where T : class
        {
            if (View == null)
                return;

            View.LoadLocalResource(resource, asset);
        }

        /// <summary>
        /// Loads the specified sourced image.
        /// </summary>
        /// <param name="image">The sourced image to load.</param>
        protected void LoadImage(SourcedImage image)
        {
            if (View == null)
                return;

            View.LoadImage(image);
        }

        /// <summary>
        /// Loads the specified sourced resource.
        /// </summary>
        /// <typeparam name="T">The type of resource being loaded.</typeparam>
        /// <param name="resource">The sourced resource to load.</param>
        protected void LoadResource<T>(SourcedResource<T> resource) where T : class
        {
            if (View == null)
                return;

            View.LoadResource(resource);
        }

        /// <summary>
        /// Draws an image that fills the entire element.
        /// </summary>
        /// <param name="dc">The drawing context that describes the render state of the layout.</param>
        /// <param name="image">The image to draw.</param>
        /// <param name="color">The color with which to draw the image.</param>
        /// <param name="drawBlankImage">A value indicating whether a blank placeholder should be drawn if 
        /// the specified image does not exist or is not loaded.</param>
        protected void DrawImage(DrawingContext dc, SourcedImage image, Color color, Boolean drawBlankImage = false)
        {
            DrawImage(dc, image, null, color, drawBlankImage);
        }

        /// <summary>
        /// Draws the specified image.
        /// </summary>
        /// <param name="dc">The drawing context that describes the render state of the layout.</param>
        /// <param name="image">The image to draw.</param>
        /// <param name="area">The area, relative to the element, in which to draw the image. A value of
        /// <c>null</c> specifies that the image should fill the element's entire area on the screen.</param>
        /// <param name="color">The color with which to draw the image.</param>
        /// <param name="drawBlank">A value indicating whether a blank placeholder should be drawn if 
        /// the specified image does not exist or is not loaded.</param>
        protected void DrawImage(DrawingContext dc, SourcedImage image, RectangleD? area, Color color, Boolean drawBlank = false)
        {
            Contract.Require(dc, "dc");

            var colorPlusOpacity = color * dc.Opacity;
            if (colorPlusOpacity.Equals(Color.Transparent))
                return;

            var imageResource = image.Resource;
            if (imageResource == null || !imageResource.IsLoaded)
            {
                if (drawBlank)
                {
                    DrawBlank(dc, area, colorPlusOpacity);
                }
            }
            else
            {
                var imageAreaRel = area ?? new RectangleD(0, 0, RenderSize.Width, RenderSize.Height);
                var imageAreaAbs = imageAreaRel + AbsolutePosition;
                var imageAreaPix = (RectangleF)Display.DipsToPixels(imageAreaAbs);

                var origin = new Vector2(
                    (Int32)(imageAreaPix.Width / 2f),
                    (Int32)(imageAreaPix.Height / 2f));

                var position = new Vector2(
                    (Int32)(imageAreaPix.X + (imageAreaPix.Width / 2f)),
                    (Int32)(imageAreaPix.Y + (imageAreaPix.Height / 2f)));

                dc.SpriteBatch.DrawImage(imageResource, position, (Int32)imageAreaPix.Width, (Int32)imageAreaPix.Height,
                    colorPlusOpacity, 0f, origin, SpriteEffects.None, 0f);
            }
        }

        /// <summary>
        /// Draws a blank rectangle.
        /// </summary>
        /// <param name="dc">The drawing context that describes the render state of the layout.</param>
        /// <param name="area">The area, relative to the element, in which to draw the image. A value of
        /// <c>null</c> specifies that the image should fill the element's entire area on the screen.</param>
        /// <param name="color">The color with which to draw the image.</param>
        protected void DrawBlank(DrawingContext dc, RectangleD? area, Color color)
        {
            Contract.Require(dc, "dc");

            var colorPlusOpacity = color * dc.Opacity;
            if (colorPlusOpacity.Equals(Color.Transparent))
                return;

            var imageResource = View.Resources.BlankImage.Resource;
            if (imageResource == null || !imageResource.IsLoaded)
                return;

            var imageAreaRel = area ?? new RectangleD(0, 0, RenderSize.Width, RenderSize.Height);
            var imageAreaAbs = imageAreaRel + AbsolutePosition;
            var imageAreaPix = (RectangleF)Display.DipsToPixels(imageAreaAbs);

            var origin = new Vector2(
                (Int32)(imageAreaPix.Width / 2f),
                (Int32)(imageAreaPix.Height / 2f));

            var position = new Vector2(
                (Int32)(imageAreaPix.X + (imageAreaPix.Width / 2f)),
                (Int32)(imageAreaPix.Y + (imageAreaPix.Height / 2f)));

            dc.SpriteBatch.DrawImage(imageResource, position, (Int32)imageAreaPix.Width, (Int32)imageAreaPix.Height,
                colorPlusOpacity, 0f, origin, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Gets the window in which the element is being displayed.
        /// </summary>
        protected IUltravioletWindow Window
        {
            get { return (View == null) ? null : View.Window; }
        }

        /// <summary>
        /// Gets the display on which the element is being displayed.
        /// </summary>
        protected IUltravioletDisplay Display
        {
            get { return (View == null) ? null : View.Display; }
        }

        /// <summary>
        /// Occurs when the value of the <see cref="AutoNav"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The dependency object that raised the event.</param>
        private static void HandleAutoNavChanged(DependencyObject dobj)
        {
            var element = (UIElement)dobj;
            element.OnAutoNavChanged();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="IsEnabled"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The dependency object that raised the event.</param>
        private static void HandleIsEnabledChanged(DependencyObject dobj)
        {
            var element = (UIElement)dobj;
            element.OnIsEnabledChanged();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="IsHitTestVisible"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The dependency object that raised the event.</param>
        private static void HandleIsHitTestVisibleChanged(DependencyObject dobj)
        {
            var element = (UIElement)dobj;
            element.OnIsHitTestVisibleChanged();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="IsTabStop"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The dependency object that raised the event.</param>
        private static void HandleIsTabStopChanged(DependencyObject dobj)
        {
            var element = (UIElement)dobj;
            element.OnIsTabStopChanged();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="Focusable"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The dependency object that raised the event.</param>
        private static void HandleFocusableChanged(DependencyObject dobj)
        {
            var element = (UIElement)dobj;
            element.OnFocusableChanged();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="Visibility"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The dependency object that raised the event.</param>
        private static void HandleVisibilityChanged(DependencyObject dobj)
        {
            var element = (UIElement)dobj;
            element.OnVisibilityChanged();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="Opacity"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The dependency object that raised the event.</param>
        private static void HandleOpacityChanged(DependencyObject dobj)
        {
            var element = (UIElement)dobj;
            element.OnOpacityChanged();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="NavUp"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The dependency object that raised the event.</param>
        private static void HandleNavUpChanged(DependencyObject dobj)
        {
            var element = (UIElement)dobj;
            element.OnNavUpChanged();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="NavDown"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The dependency object that raised the event.</param>
        private static void HandleNavDownChanged(DependencyObject dobj)
        {
            var element = (UIElement)dobj;
            element.OnNavDownChanged();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="NavLeft"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The dependency object that raised the event.</param>
        private static void HandleNavLeftChanged(DependencyObject dobj)
        {
            var element = (UIElement)dobj;
            element.OnNavLeftChanged();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="NavRight"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The dependency object that raised the event.</param>
        private static void HandleNavRightChanged(DependencyObject dobj)
        {
            var element = (UIElement)dobj;
            element.OnNavRightChanged();
        }

        /// <summary>
        /// Occurs when the value of the <see cref="TabIndex"/> dependency property changes.
        /// </summary>
        /// <param name="dobj">The dependency object that raised the event.</param>
        private static void HandleTabIndexChanged(DependencyObject dobj)
        {
            var element = (UIElement)dobj;
            element.OnTabIndexChanged();
        }

        /// <summary>
        /// Invokes the <see cref="OnGotKeyboardFocus"/> method.
        /// </summary>
        private static void OnGotKeyboardFocusProxy(UIElement element, ref Boolean handled)
        {
            element.OnGotKeyboardFocus(ref handled);
        }

        /// <summary>
        /// Invokes the <see cref="OnLostKeyboardFocus"/> method.
        /// </summary>
        private static void OnLostKeyboardFocusProxy(UIElement element, ref Boolean handled)
        {
            element.OnLostKeyboardFocus(ref handled);
        }

        /// <summary>
        /// Invokes the <see cref="OnKeyDown"/> method.
        /// </summary>
        private static void OnKeyDownProxy(UIElement element, KeyboardDevice device, Key key, KeyModifiers modifiers, ref Boolean handled)
        {
            element.OnKeyDown(device, key, modifiers, ref handled);
        }

        /// <summary>
        /// Invokes the <see cref="OnKeyUp"/> method.
        /// </summary>
        private static void OnKeyUpProxy(UIElement element, KeyboardDevice device, Key key, ref Boolean handled)
        {
            element.OnKeyUp(device, key, ref handled);
        }

        /// <summary>
        /// Invokes the <see cref="OnTextInput"/> method.
        /// </summary>
        private static void OnTextInputProxy(UIElement element, KeyboardDevice device, ref Boolean handled)
        {
            element.OnTextInput(device, ref handled);
        }

        /// <summary>
        /// Invokes the <see cref="OnGotMouseCapture"/> method.
        /// </summary>
        /// <param name="handled">A value indicating whether the event has been handled.</param>
        private static void OnGotMouseCaptureProxy(UIElement element, ref Boolean handled)
        {
            element.OnGotMouseCapture(ref handled);
        }

        /// <summary>
        /// Invokes the <see cref="OnLostMouseCapture"/> method.
        /// </summary>
        /// <param name="handled">A value indicating whether the event has been handled.</param>
        private static void OnLostMouseCaptureProxy(UIElement element, ref Boolean handled)
        {
            element.OnLostMouseCapture(ref handled);
        }

        /// <summary>
        /// Invokes the <see cref="OnMouseMove"/> method.
        /// </summary>
        /// <param name="device">The mouse device.</param>
        /// <param name="x">The x-coordinate of the cursor in device-independent screen coordinates.</param>
        /// <param name="y">The y-coordinate of the cursor in device-independent screen coordinates.</param>
        /// <param name="dx">The difference between the x-coordinate of the mouse's 
        /// current position and the x-coordinate of the mouse's previous position.</param>
        /// <param name="dy">The difference between the y-coordinate of the mouse's 
        /// current position and the y-coordinate of the mouse's previous position.</param>
        /// <param name="handled">A value indicating whether the event has been handled.</param>
        private static void OnMouseMoveProxy(UIElement element, MouseDevice device, Double x, Double y, Double dx, Double dy, ref Boolean handled)
        {
            element.OnMouseMove(device, x, y, dx, dy, ref handled);
        }

        /// <summary>
        /// Invokes the <see cref="OnMouseEnter"/> method.
        /// </summary>
        /// <param name="device">The mouse device.</param>
        /// <param name="handled">A value indicating whether the event has been handled.</param>
        private static void OnMouseEnterProxy(UIElement element, MouseDevice device, ref Boolean handled)
        {
            element.IsHovering = true;
            element.OnMouseEnter(device, ref handled);
        }

        /// <summary>
        /// Invokes the <see cref="OnMouseLeave"/> method.
        /// </summary>
        /// <param name="device">The mouse device.</param>
        /// <param name="handled">A value indicating whether the event has been handled.</param>
        private static void OnMouseLeaveProxy(UIElement element, MouseDevice device, ref Boolean handled)
        {
            element.IsHovering = false;
            element.OnMouseLeave(device, ref handled);
        }

        /// <summary>
        /// Invokes the <see cref="OnMouseDown"/> method.
        /// </summary>
        /// <param name="device">The mouse device.</param>
        /// <param name="button">The mouse button that was pressed.</param>
        /// <param name="handled">A value indicating whether the event has been handled.</param>
        private static void OnMouseDownProxy(UIElement element, MouseDevice device, MouseButton button, ref Boolean handled)
        {
            element.OnMouseDown(device, button, ref handled);
        }

        /// <summary>
        /// Invokes the <see cref="OnMouseUp"/> method.
        /// </summary>
        /// <param name="device">The mouse device.</param>
        /// <param name="button">The mouse button that was released.</param>
        /// <param name="handled">A value indicating whether the event has been handled.</param>
        private static void OnMouseUpProxy(UIElement element, MouseDevice device, MouseButton button, ref Boolean handled)
        {
            element.OnMouseUp(device, button, ref handled);
        }

        /// <summary>
        /// Invokes the <see cref="OnMouseClick"/> method.
        /// </summary>
        /// <param name="device">The mouse device.</param>
        /// <param name="button">The mouse button that was clicked.</param>
        /// <param name="handled">A value indicating whether the event has been handled.</param>
        private static void OnMouseClickProxy(UIElement element, MouseDevice device, MouseButton button, ref Boolean handled)
        {
            element.OnMouseClick(device, button, ref handled);
        }

        /// <summary>
        /// Invokes the <see cref="OnMouseDoubleClick"/> method.
        /// </summary>
        /// <param name="device">The mouse device.</param>
        /// <param name="button">The mouse button that was clicked.</param>
        /// <param name="handled">A value indicating whether the event has been handled.</param>
        private static void OnMouseDoubleClickProxy(UIElement element, MouseDevice device, MouseButton button, ref Boolean handled)
        {
            element.OnMouseDoubleClick(device, button, ref handled);
        }

        /// <summary>
        /// Invokes the <see cref="OnMouseWheel"/> method.
        /// </summary>
        /// <param name="device">The mouse device.</param>
        /// <param name="x">The amount that the wheel was scrolled along the x-axis.</param>
        /// <param name="y">The amount that the wheel was scrolled along the y-axis.</param>
        /// <param name="handled">A value indicating whether the event has been handled.</param>
        private static void OnMouseWheelProxy(UIElement element, MouseDevice device, Double x, Double y, ref Boolean handled)
        {
            element.OnMouseWheel(device, x, y, ref handled);
        }

        /// <summary>
        /// Updates the value of the <see cref="LayoutDepth"/> property.
        /// </summary>
        private void CacheLayoutDepth()
        {
            this.layoutDepth = (Parent == null) ? 0 : Parent.LayoutDepth + 1;
            this.logicalOrder = 0;

            var parent = LogicalTreeHelper.GetParent(this);
            if (parent != null)
            {
                var children = LogicalTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < children; i++)
                {
                    if (LogicalTreeHelper.GetChild(parent, i) == this)
                    {
                        this.logicalOrder = i;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the value of the <see cref="View"/> property.
        /// </summary>
        private void CacheView()
        {
            var logicalParent = LogicalTreeHelper.GetParent(this) as UIElement;
            if (logicalParent != null)
            {
                this.view = null;

                for (var current = logicalParent; current != null; current = LogicalTreeHelper.GetParent(current) as UIElement)
                {
                    if (current.View != null)
                    {
                        this.view = current.View;
                        break;
                    }
                }
            }
            else
            {
                var visualParent = VisualTreeHelper.GetParent(this) as UIElement;
                if (visualParent == null)
                    return;

                this.view = null;

                for (var current = visualParent; current != null; current = VisualTreeHelper.GetParent(current) as UIElement)
                {
                    if (current.View != null)
                    {
                        this.view = current.View;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the value of the <see cref="Control"/> property.
        /// </summary>
        private void CacheControl()
        {
            UnregisterElement();

            this.control = FindControl();

            RegisterElement();
        }

        /// <summary>
        /// Adds the element to the current view's element registry.
        /// </summary>
        private void RegisterElement()
        {
            if (elementRegistrationContext == null)
                elementRegistrationContext = FindElementRegistry();

            if (String.IsNullOrEmpty(id))
                return;

            if (elementRegistrationContext != null)
                elementRegistrationContext.RegisterElement(this);
        }

        /// <summary>
        /// Removes the element from the current view's element registry.
        /// </summary>
        private void UnregisterElement()
        {
            if (String.IsNullOrEmpty(id))
                return;

            if (elementRegistrationContext != null)
            {
                elementRegistrationContext.UnregisterElement(this);
                elementRegistrationContext = null;
            }
        }

        /// <summary>
        /// Finds the element registration context for this element.
        /// </summary>
        /// <returns>The element registration context for this element.</returns>
        private UIElementRegistry FindElementRegistry()
        {
            if (Control != null)
            {
                return Control.ComponentRegistry;
            }
            return (view == null) ? null : view.ElementRegistry;
        }

        /// <summary>
        /// Gets the specified element within the current navigation context.
        /// </summary>
        /// <param name="id">The identifier of the element to retrieve.</param>
        /// <returns>The element with the specified identifier, or <c>null</c> if no such element exists.</returns>
        private UIElement FindNavElement(String id)
        {
            if (elementRegistrationContext == null)
                return null;

            if (String.IsNullOrEmpty(id))
                return null;

            var element = elementRegistrationContext.GetElementByID(id);
            if (element == null || element.Focusable)
                return element;

            return element.GetFirstFocusableDescendant(false);
        }

        /// <summary>
        /// Gets the element which is navigated to when focus is moved to the next tab stop.
        /// </summary>
        /// <param name="current">The currently focused element, if any.</param>
        /// <returns>The specified element, or <c>null</c> if no such element is defined.</returns>
        private UIElement GetNextTabStopInternal(UIElement current = null)
        {
            // Find the first matching child element.
            var childMatch = this.GetNextTabStopWithinTree(current);
            if (childMatch != null)
                return childMatch;

            // Find the next matching sibling.
            if (Parent != null)
            {
                var siblingMatch = Parent.GetNextTabStopWithinTree(this);
                if (siblingMatch != null)
                    return siblingMatch;

                // Find our parent's next sibling.
                return Parent.GetNextTabStopInternal(this);
            }

            return null;
        }

        /// <summary>
        /// Gets the element which is navigated to when focus is moved to the previous tab stop.
        /// </summary>
        /// <returns>The specified element, or <c>null</c> if no such element is defined.</returns>
        private UIElement GetPreviousTabStopInternal()
        {
            if (Parent != null)
            {
                // Find our previous sibling in the tab order.
                var siblingMatch = Parent.GetPreviousTabStopWithinTree(this);
                if (siblingMatch != null)
                    return siblingMatch.GetLastFocusableDescendant(true);

                // If we have no qualifying siblings, return our parent.
                if (Parent.Focusable && Parent.IsTabStop)
                    return Parent;

                // If our parent doesn't qualify, let it figure it out!
                return Parent.GetPreviousTabStopInternal();
            }

            return null;
        }

        /// <summary>
        /// Gets the next tab stop within the part of the visual tree that is rooted in the current element.
        /// </summary>
        /// <param name="currentStop">The current tab stop.</param>
        /// <returns>The specified tab stop, or <c>null</c> if no such element exists.</returns>
        private UIElement GetNextTabStopWithinTree(UIElement currentStop)
        {
            var currentFound    = false;
            var currentTabIndex = (currentStop == null ? Int32.MinValue : currentStop.TabIndex);

            var match = default(UIElement);
            for (int i = 0; i < VisualChildrenCount; i++)
            {
                var child = GetVisualChild(i);
                if (child == currentStop)
                {
                    currentFound = true;
                    continue;
                }

                var matchingDescendant = child.GetFirstFocusableDescendant(true);
                if (matchingDescendant != null)
                {
                    if (currentFound && matchingDescendant.TabIndex == currentTabIndex)
                        return matchingDescendant;

                    if (matchingDescendant.TabIndex > currentTabIndex && (match == null || match.TabIndex > matchingDescendant.TabIndex))
                        match = matchingDescendant;
                }
            }

            return match;
        }

        /// <summary>
        /// Gets the previous tab stop within the part of the visual tree that is rooted in the current element.
        /// </summary>
        /// <param name="currentStop">The current tab stop.</param>
        /// <returns>The specified tab stop, or <c>null</c> if no such element exists.</returns>
        private UIElement GetPreviousTabStopWithinTree(UIElement currentStop)
        {
            var currentFound    = false;
            var currentTabIndex = (currentStop == null ? Int32.MaxValue : currentStop.TabIndex);

            var match = default(UIElement);
            for (int i = VisualChildrenCount - 1; i >= 0; i--)
            {
                var child = GetVisualChild(i);
                if (child == currentStop)
                {
                    currentFound = true;
                    continue;
                }

                if (currentFound && child.TabIndex == currentTabIndex)
                {
                    var matchingDescendant = child.GetLastFocusableDescendant(true);
                    if (matchingDescendant != null)
                        return matchingDescendant;
                }

                if (child.TabIndex < currentTabIndex && (match == null || match.TabIndex < child.TabIndex))
                    match = child;
            }

            return (match == null) ? null : match.GetLastFocusableDescendant(true); ;
        }

        /// <summary>
        /// Recurses through the logical tree to find the first descendant of the specified element
        /// which is focusable (and potentially, a tab stop).
        /// </summary>
        /// <param name="parent">The parent element which is being examined.</param>
        /// <param name="tabStop">A value indicating whether a matching element must be a tab stop.</param>
        /// <returns>The first element within this branch of the logical tree which meets the specified criteria.</returns>
        private UIElement GetFirstFocusableDescendantInternal(UIElement parent, Boolean tabStop)
        {
            var children = EnumerateVisualChildrenInTabOrder();
            foreach (var child in children)
            {
                var candidate = child.GetFirstFocusableDescendant(tabStop);
                if (candidate != null)
                {
                    children.Clear();
                    return candidate;
                }
            }
            children.Clear();
            return null;
        }

        /// <summary>
        /// Recurses through the logical tree to find the last descendant of the specified element
        /// which is focusable (and potentially, a tab stop).
        /// </summary>
        /// <param name="parent">The parent element which is being examined.</param>
        /// <param name="tabStop">A value indicating whether a matching element must be a tab stop.</param>
        /// <returns>The last element within this branch of the logical tree which meets the specified criteria.</returns>
        private UIElement GetLastFocusableDescendantInternal(UIElement parent, Boolean tabStop)
        {
            var children = EnumerateVisualChildrenInReverseTabOrder();
            foreach (var child in children)
            {
                var candidate = child.GetLastFocusableDescendant(tabStop);
                if (candidate != null)
                {
                    children.Clear();
                    return candidate;
                }
            }
            children.Clear();
            return null;
        }

        /// <summary>
        /// Searches the element hierarchy for the control that owns
        /// this element, if this element is a component.
        /// </summary>
        /// <returns>The control that owns this element, or <c>null</c> if this element is not a component.</returns>
        private Control FindControl()
        {
            if (Parent is Control && ((Control)Parent).ComponentRoot == this)
                return (Control)Parent;

            var current = Parent;
            while (current != null)
            {
                if (current is ContentControl)
                    return null;

                if (current.Control != null)
                    return current.Control;

                current = current.Parent;
            }
            return null;
        }

        /// <summary>
        /// Cleans up the element's storyboards by returning any storyboard clocks to the global pool.
        /// </summary>
        private void CleanupStoryboards()
        {
            var pool = StoryboardClockPool.Instance;
            foreach (var kvp in storyboardClocks)
            {
                kvp.Value.Stop();
                pool.Release(kvp.Value);
            }
            storyboardClocks.Clear();
        }

        /// <summary>
        /// Returns a list containing the element's visual children.
        /// </summary>
        /// <returns>A list containing the element's visual children.</returns>
        private List<UIElement> EnumerateVisualChildren()
        {
            visualChildEnumerationBuffer.Clear();

            for (int i = 0; i < VisualChildrenCount; i++)
                visualChildEnumerationBuffer.Add(GetVisualChild(i));

            return visualChildEnumerationBuffer;
        }

        /// <summary>
        /// Returns a list containing the element's visual children in tab order.
        /// </summary>
        /// <returns>A list containing the element's visual children in tab order.</returns>
        private List<UIElement> EnumerateVisualChildrenInTabOrder()
        {
            var buffer = EnumerateVisualChildren();

            buffer.Sort((element1, element2) =>
            {
                if (element1.TabIndex == element2.TabIndex)
                {
                    return element1.logicalOrder.CompareTo(element2.logicalOrder);
                }
                return element1.TabIndex.CompareTo(element2.TabIndex);
            });

            return buffer;
        }

        /// <summary>
        /// Returns a list containing the element's visual children in reverse tab order.
        /// </summary>
        /// <returns>A list containing the element's visual children in reverse tab order.</returns>
        private List<UIElement> EnumerateVisualChildrenInReverseTabOrder()
        {
            var buffer = EnumerateVisualChildren();

            buffer.Sort((element1, element2) =>
            {
                if (element1.TabIndex == element2.TabIndex)
                {
                    return -element1.logicalOrder.CompareTo(element2.logicalOrder);
                }
                return -element1.TabIndex.CompareTo(element2.TabIndex);
            });

            return buffer;
        }

        // Buffers which are used to enumerate an element's logical children.
        [ThreadStatic]
        private readonly List<UIElement> visualChildEnumerationBuffer = new List<UIElement>(32);

        // Property values.
        private readonly UltravioletContext uv;
        private readonly UIElementClassCollection classes;
        private readonly String id;
        private readonly String name;
        private PresentationFoundationView view;
        private UIElement parent;
        private Control control = null;
        private Boolean isStyleValid;
        private Boolean isMeasureValid;
        private Boolean isArrangeValid;
        private Boolean isPositionValid;
        private Boolean isHovering;
        private Point2D renderOffset;
        private Size2D renderSize;
        private Size2D desiredSize;
        private RectangleD relativeBounds;
        private RectangleD absoluteBounds;
        private RectangleD? clipRectangle;

        // Layout parameters.
        private UvssDocument mostRecentStylesheet;
        private ArrangeOptions mostRecentArrangeOptions;
        private RectangleD mostRecentFinalRect;
        private Size2D mostRecentAvailableSize;
        private Point2D mostRecentPosition;
        private Int32 layoutDepth;
        private Int32 logicalOrder;

        // State values.
        private UIElementRegistry elementRegistrationContext;
        private Boolean isStyling;
        private Boolean isMeasuring;
        private Boolean isArranging;

        // The collection of active storyboard clocks on this element.
        private readonly Dictionary<Storyboard, StoryboardClock> storyboardClocks = 
            new Dictionary<Storyboard, StoryboardClock>();

        // The element's routed event manager.
        private readonly RoutedEventManager routedEventManager = new RoutedEventManager();
    }
}
