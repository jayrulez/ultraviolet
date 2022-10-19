using System.Runtime.CompilerServices;
using NUnit.Framework;
using Sedulous.Input;
using Sedulous.Presentation.Input;
using Sedulous.TestFramework;

namespace Sedulous.Presentation.Tests.Input
{
    [TestFixture]
    public class GamePadGestureTests : SedulousTestFramework
    {
        [Test]
        public void GamePadGesture_TryParse_SucceedsForValidStrings()
        {
            RuntimeHelpers.RunClassConstructor(typeof(SedulousStrings).TypeHandle);

            var gesture = default(GamePadGesture);
            var result = GamePadGesture.TryParse("LeftStick", out gesture);

            TheResultingValue(result).ShouldBe(true);
            TheResultingValue(gesture.Button).ShouldBe(GamePadButton.LeftStick);
            TheResultingValue(gesture.PlayerIndex).ShouldBe(GamePadGesture.AnyPlayerIndex);
        }

        [Test]
        public void GamePadGesture_TryParse_SucceedsForValidStrings_WithExplicitAnyPlayerIndex()
        {
            RuntimeHelpers.RunClassConstructor(typeof(SedulousStrings).TypeHandle);

            var gesture = default(GamePadGesture);
            var result = GamePadGesture.TryParse("ANY:LeftStick", out gesture);

            TheResultingValue(result).ShouldBe(true);
            TheResultingValue(gesture.Button).ShouldBe(GamePadButton.LeftStick);
            TheResultingValue(gesture.PlayerIndex).ShouldBe(GamePadGesture.AnyPlayerIndex);
        }

        [Test]
        public void GamePadGesture_TryParse_SucceedsForValidStrings_WithNumericPlayerIndex()
        {
            RuntimeHelpers.RunClassConstructor(typeof(SedulousStrings).TypeHandle);

            var gesture = default(GamePadGesture);
            var result = GamePadGesture.TryParse("P1:LeftStick", out gesture);

            TheResultingValue(result).ShouldBe(true);
            TheResultingValue(gesture.Button).ShouldBe(GamePadButton.LeftStick);
            TheResultingValue(gesture.PlayerIndex).ShouldBe(1);
        }

        [Test]
        public void GamePadGesture_TryParse_FailsForInvalidStrings()
        {
            RuntimeHelpers.RunClassConstructor(typeof(SedulousStrings).TypeHandle);

            var gesture = default(GamePadGesture);
            var result = GamePadGesture.TryParse("asdfasdfas", out gesture);

            TheResultingValue(result).ShouldBe(false);
            TheResultingObject(gesture).ShouldBeNull();
        }

        [Test]
        public void GamePadGesture_TryParse_FailsForInvalidStringsWithNegativePlayerIndices()
        {
            RuntimeHelpers.RunClassConstructor(typeof(SedulousStrings).TypeHandle);

            var gesture = default(GamePadGesture);
            var result = GamePadGesture.TryParse("P-1:LeftStick", out gesture);

            TheResultingValue(result).ShouldBe(false);
            TheResultingObject(gesture).ShouldBeNull();
        }
    }
}
