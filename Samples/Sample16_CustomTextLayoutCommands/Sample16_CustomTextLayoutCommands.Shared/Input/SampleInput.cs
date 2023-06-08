using Sedulous;
using Sedulous.Core;
using Sedulous.Input;

namespace Sample16_CustomTextLayoutCommands.Input
{
    public static class SampleInput
    {
        public static Actions GetActions(this IInputSubsystem input) =>
            Actions.Instance;

        public class Actions : InputActionCollection
        {
			public Actions(FrameworkContext uv)
                : base(uv)
            { }

            public static Actions Instance { get; } = CreateSingleton<Actions>();

            public InputAction ExitApplication { get; private set; }
            public InputAction ResetScrollingText { get; private set; }

            /// <inheritdoc/>
            protected override void OnCreatingActions()
            {
                this.ExitApplication =
                    CreateAction("EXIT_APPLICATION");

                this.ResetScrollingText =
                    CreateAction("RESET_SCROLLING_TEXT");

                base.OnCreatingActions();
            }

            /// <inheritdoc/>
            protected override void OnResetting()
            {
                switch (FrameworkContext.Platform)
                {
                    case FrameworkPlatform.Android:
                        Reset_Android();
                        break;

                    default:
                        Reset_Desktop();
                        break;
                }
                base.OnResetting();
            }

            private void Reset_Desktop()
            {
                this.ExitApplication
                    .Primary = CreateKeyboardBinding(Key.Escape);

                this.ResetScrollingText
                    .Primary = CreateKeyboardBinding(Key.R);
            }

            private void Reset_Android()
            {
                this.ExitApplication
                    .Primary = CreateKeyboardBinding(Key.AppControlBack);
            }
        }
    }
}
