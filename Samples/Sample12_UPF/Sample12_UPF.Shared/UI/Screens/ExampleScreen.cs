using System;
using Sedulous.Content;
using Sedulous.UI;

namespace Sample12_UPF.UI.Screens
{
    public class ExampleScreen : UIScreen
    {
        public ExampleScreen(ContentManager globalContent, UIScreenService uiScreenService)
            : base("Content/UI/Screens/ExampleScreen", "ExampleScreen", globalContent)
        {

        }

        protected override Object CreateViewModel(UIView view)
        {
            return new ExampleViewModel(FrameworkContext);
        }
    }
}
