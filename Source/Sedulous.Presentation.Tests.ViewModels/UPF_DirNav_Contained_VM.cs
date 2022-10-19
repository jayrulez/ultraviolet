using Sedulous.Presentation;
using Sedulous.Presentation.Controls;

namespace Sedulous.Presentation.Tests.ViewModels
{
    public class UPF_DirNav_Contained_VM
    {
        public void HandleViewOpening(DependencyObject dobj, RoutedEventData data)
        {
            btn1.Focus();
        }

        private readonly Button btn1 = null;
    }
}
