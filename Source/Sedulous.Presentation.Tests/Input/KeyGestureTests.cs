using System.Runtime.CompilerServices;
using NUnit.Framework;
using Sedulous.Input;
using Sedulous.Presentation.Input;
using Sedulous.TestFramework;

namespace Sedulous.Presentation.Tests.Input
{
    [TestFixture]
    public class KeyGestureTests : FrameworkTestFramework
    {
        [Test]
        public void KeyGesture_TryParse_SucceedsForValidStrings()
        {
            UsingCulture("en-US", () =>
            {
                RuntimeHelpers.RunClassConstructor(typeof(FrameworkStrings).TypeHandle);

                var gesture = default(KeyGesture);
                var result = KeyGesture.TryParse("X", out gesture);

                TheResultingValue(result).ShouldBe(true);
                TheResultingValue(gesture.Key).ShouldBe(Key.X);
                TheResultingValue(gesture.Modifiers).ShouldBe(ModifierKeys.None);
                TheResultingString(gesture.GetDisplayStringForCulture(null)).ShouldBe("X");
            });
        }

        [Test]
        public void KeyGesture_TryParse_SucceedsForValidStrings_WithModifierKeys()
        {
            UsingCulture("en-US", () =>
            {
                RuntimeHelpers.RunClassConstructor(typeof(FrameworkStrings).TypeHandle);

                var gesture = default(KeyGesture);
                var result = KeyGesture.TryParse("Ctrl+Alt+X", out gesture);

                TheResultingValue(result).ShouldBe(true);
                TheResultingValue(gesture.Key).ShouldBe(Key.X);
                TheResultingValue(gesture.Modifiers).ShouldBe(ModifierKeys.Control | ModifierKeys.Alt);
                TheResultingString(gesture.GetDisplayStringForCulture(null)).ShouldBe("Ctrl+Alt+X");
            });
        }

        [Test]
        public void KeyGesture_TryParse_FailsForInvalidStrings()
        {
            RuntimeHelpers.RunClassConstructor(typeof(FrameworkStrings).TypeHandle);

            var gesture = default(KeyGesture);
            var result = KeyGesture.TryParse("asdfasdfas", out gesture);

            TheResultingValue(result).ShouldBe(false);
            TheResultingObject(gesture).ShouldBeNull();
        }

        [Test]
        public void KeyGesture_TryParse_FailsForInvalidStringsWithRepeatedModifiers()
        {
            RuntimeHelpers.RunClassConstructor(typeof(FrameworkStrings).TypeHandle);

            var gesture = default(KeyGesture);
            var result = KeyGesture.TryParse("Ctrl+Ctrl+X", out gesture);

            TheResultingValue(result).ShouldBe(false);
            TheResultingObject(gesture).ShouldBeNull();
        }
    }
}
