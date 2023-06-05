﻿using System;
using Sedulous.Core;
using Sedulous.Presentation;

namespace Sandbox3D.UI.Dialogs
{
    partial class EscMenuDialog
    {
        /// <summary>
        /// Represents the view model for <see cref="DialogScreen"/>.
        /// </summary>
        public class DialogScreenVM
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DialogScreenVM"/> class.
            /// </summary>
            /// <param name="screen">The dialog screen that owns the view model.</param>
            internal DialogScreenVM(DialogScreen screen)
            {
                Contract.Require(screen, nameof(screen));

                this.screen = screen;
            }

            /// <summary>
            /// Handles the "Resume" button being clicked.
            /// </summary>
            public void HandleClickResume(DependencyObject dobj, RoutedEventData data)
            {
                screen.Dialog.Close(false);
            }

            /// <summary>
            /// Handles the "Exit" button being clicked.
            /// </summary>
            public void HandleClickExit(DependencyObject dobj, RoutedEventData data)
            {
                screen.Dialog.Close(true, TimeSpan.Zero);
            }

            /// <summary>
            /// Handles the "Exit to Desktop" button being clicked.
            /// </summary>
            public void HandleClickExitToDesktop(DependencyObject dobj, RoutedEventData data)
            {
                screen.Dialog.Screen.FrameworkContext.Host.Exit();
            }

            // State values.
            private readonly DialogScreen screen;
        }
    }
}