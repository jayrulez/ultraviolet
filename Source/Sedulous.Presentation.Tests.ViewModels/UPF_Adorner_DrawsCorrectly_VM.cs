﻿using Sedulous.Presentation.Controls;
using Sedulous.Presentation.Documents;

namespace Sedulous.Presentation.Tests.ViewModels
{
    public class UPF_Adorner_DrawsCorrectly_VM
    {
        public void HandleViewLoaded(DependencyObject dobj, RoutedEventData data)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(button);
            adornerLayer.Add(new ExampleBoxesAdorner(button));
        }

        private readonly Button button = null;
    }
}
