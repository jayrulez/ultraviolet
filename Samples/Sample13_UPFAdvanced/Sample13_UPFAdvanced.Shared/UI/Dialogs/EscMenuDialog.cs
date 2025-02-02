﻿using System;
using Sample13_UPFAdvanced.UI.Screens;
using Sedulous.Core;
using Sedulous.Presentation;
using Sedulous.UI;

namespace Sample13_UPFAdvanced.UI.Dialogs
{
    /// <summary>
    /// Represents the dialog box that is shown when the user presses the escape key.
    /// </summary>
    public partial class EscMenuDialog : Modal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EscMenuDialog"/> class.
        /// </summary>
        /// <param name="owner">The screen that owns the dialog box.</param>
        public EscMenuDialog(GameScreenBase owner)
        {
            Contract.Require(owner, "owner");

            this.screen = new DialogScreen(this, owner.GlobalContent);
        }

        /// <inheritdoc/>
        public override UIScreen Screen
        {
            get
            {
                Contract.EnsureNotDisposed(this, Disposed);

                return screen;
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                SafeDispose.Dispose(screen);
            }
            base.Dispose(disposing);
        }

        // Property values.
        private readonly DialogScreen screen;
    }
}
