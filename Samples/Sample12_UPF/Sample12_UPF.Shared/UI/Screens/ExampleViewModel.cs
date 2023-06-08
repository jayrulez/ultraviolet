using System;
using Sedulous;
using Sedulous.Core;
using Sedulous.Presentation;
using Sedulous.Presentation.Controls;

namespace Sample12_UPF.UI.Screens
{
    public class ExampleViewModel
    {
        public ExampleViewModel(FrameworkContext uv)
        {
            this.uv = uv;
        }

        public void Exit(DependencyObject element, RoutedEventData data)
        {
            uv.Host.Exit();
        }

        public void Reset(DependencyObject element, RoutedEventData data)
        {
            this.Message = "Hello, world!";
        }

        public void ButtonClick(DependencyObject element, RoutedEventData data)
        {
            this.Message = "You clicked " + ((Button)element).Content;
        }

        public String Message { get; set; } = "Hello, world!";

        public Boolean IsExitButtonVisible
        {
            get { return uv?.Platform != FrameworkPlatform.iOS; }
        }

        private readonly FrameworkContext uv;
    }
}
