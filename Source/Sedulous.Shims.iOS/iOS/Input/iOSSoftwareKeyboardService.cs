﻿using System;
using Sedulous;
using Sedulous.Input;
using UIKit;
using Sedulous.SDL2.UIKit;
using System.Linq;

namespace Sedulous.Shims.iOS.Input
{
    /// <summary>
    /// Represents an implementation of the <see cref="SoftwareKeyboardService"/> class for the iOS platform.
    /// </summary>
    public sealed class iOSSoftwareKeyboardService : SoftwareKeyboardService
    {
        /// <inheritdoc/>
        public override Boolean ShowSoftwareKeyboard(KeyboardMode mode)
        {
            var controller = (SDL_uikitviewcontroller)UIApplication.SharedApplication?.KeyWindow?.RootViewController;
            if (controller == null)
                return false;

            var textField = (UITextField)controller.View.Subviews[0];
            switch (mode)
            {
                case KeyboardMode.Number:
                    textField.KeyboardType = UIKeyboardType.DecimalPad;
                    textField.ReloadInputViews();
                    break;

                case KeyboardMode.Phone:
                    textField.KeyboardType = UIKeyboardType.PhonePad;
                    textField.ReloadInputViews();
                    break;

                case KeyboardMode.Datetime:
                    textField.KeyboardType = UIKeyboardType.NumbersAndPunctuation;
                    textField.ReloadInputViews();
                    break;

                default:
                    textField.KeyboardType = UIKeyboardType.Default;
                    textField.ReloadInputViews();
                    break;
            }

            controller.ShowKeyboard();
            OnShowingSoftwareKeyboard();

            return true;
        }

        /// <inheritdoc/>
        public override Boolean HideSoftwareKeyboard()
        {
            var controller = (SDL_uikitviewcontroller)UIApplication.SharedApplication?.KeyWindow?.RootViewController;
            //var controller = (SDL_uikitviewcontroller)UIApplication.SharedApplication?.ConnectedScenes.Windows?.LastOrDefault()?.RootViewController;
            if (controller == null)
                return false;

            controller.HideKeyboard();
            OnHidingSoftwareKeyboard();

            return true;
        }

        /// <inheritdoc/>
        public override Rectangle? TextInputRegion
        {
            get { return textInputRegion; }
            set
            {
                if (textInputRegion != value)
                {
                    textInputRegion = value;
                    FrameworkContext.RequestCurrent()?.Messages.Publish(
                        FrameworkMessages.TextInputRegionChanged, null);
                }
            }
        }

        // Property values.
        private Rectangle? textInputRegion;
    }
}
