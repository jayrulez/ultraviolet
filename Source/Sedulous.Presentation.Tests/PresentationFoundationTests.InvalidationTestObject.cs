using System;
using Sedulous.Presentation;
using Sedulous.Presentation.Media;

namespace Sedulous.Presentation.Tests
{
    partial class PresentationFoundationTests
    {
        public class InvalidationTestObject : DependencyObject
        {
            public Boolean TransformChanged { get; set; }

            public Transform Transform
            {
                get { return GetValue<Transform>(TransformProperty); }
                set { SetValue(TransformProperty, value); }
            }

            public static readonly DependencyProperty TransformProperty = DependencyProperty.Register("Transform", typeof(Transform), typeof(InvalidationTestObject),
                new PropertyMetadata<Transform>(HandleTransformChanged));

            private static void HandleTransformChanged(DependencyObject dobj, Transform oldValue, Transform newValue)
            {
                ((InvalidationTestObject)dobj).TransformChanged = true;
            }
        }
    }
}
