using Sedulous.Presentation;
using Sedulous.Presentation.Controls;

namespace Sedulous.Presentation.Tests.ViewModels
{
    public class UPF_DirNav_Continue_VM
    {
        public void HandleViewOpening(DependencyObject dobj, RoutedEventData data)
        {
            btnL.Focus();
        }

        private readonly Button btnL = null;
    }
}
