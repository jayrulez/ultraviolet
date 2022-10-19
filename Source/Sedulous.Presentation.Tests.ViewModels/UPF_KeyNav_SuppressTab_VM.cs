using Sedulous.Input;
using Sedulous.Presentation;
using Sedulous.Presentation.Input;

namespace Sedulous.Presentation.Tests.ViewModels
{
    public class UPF_KeyNav_SuppressTab_VM
    {
        public void SuppressTab(DependencyObject dobj, KeyboardDevice device, Key key, ModifierKeys modifiers, RoutedEventData data)
        {
            data.Handled = true;
        }
    }
}
