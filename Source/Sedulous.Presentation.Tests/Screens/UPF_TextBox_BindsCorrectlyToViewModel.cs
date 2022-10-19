using Sedulous.Content;
using Sedulous.Presentation.Tests.ViewModels;

namespace Sedulous.Presentation.Tests.Screens
{
    public class UPF_TextBox_BindsCorrectlyToViewModel : TestScreenBase<UPF_TextBox_BindsCorrectlyToViewModel_VM>
    {
        public UPF_TextBox_BindsCorrectlyToViewModel(ContentManager globalContent)
            : base("Resources/Content/UI/Screens/UPF_TextBox_BindsCorrectlyToViewModel", "View", globalContent)
        {

        }
    }
}
