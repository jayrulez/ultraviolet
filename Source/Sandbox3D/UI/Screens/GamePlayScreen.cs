﻿using System;
using Sedulous;
using Sedulous.Content;
using Sedulous.Core;
using Sedulous.UI;
using Sandbox3D.UI.Dialogs;

namespace Sandbox3D.UI.Screens
{
    /// <summary>
    /// Represents the primary gameplay screen.
    /// </summary>
    public class GamePlayScreen : GameScreenBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GamePlayScreen"/> class.
        /// </summary>
        /// <param name="globalContent">The content manager with which to load globally-available assets.</param>
        /// <param name="uiScreenService">The screen service which created this screen.</param>
        public GamePlayScreen(ContentManager globalContent, UIScreenService uiScreenService)
            : base("Content/UI/Screens/GamePlayScreen", "GamePlayScreen", globalContent, uiScreenService)
        {
            this.escMenuDialog = new EscMenuDialog(this);
        }

        /// <inheritdoc/>
        public override void Update(FrameworkTime time)
        {
            var vm = View.GetViewModel<GamePlayViewModel>();
            if (vm != null && vm.IsTriangleSpinning)
            {
                var rotMax = (Single)(Math.PI * 2.0);
                var rotDelta = (Single)(rotMax * time.ElapsedTime.TotalSeconds);
                vm.TriangleRotation = (vm.TriangleRotation + rotDelta) % rotMax;
            }
            base.Update(time);
        }
        
        /// <inheritdoc/>
        protected override Object CreateViewModel(UIView view)
        {
            return new GamePlayViewModel(this, escMenuDialog);
        }

        /// <inheritdoc/>
        protected override void OnOpening()
        {
            ResetViewModel();
            base.OnOpening();
        }

        /// <inheritdoc/>
        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                SafeDispose.Dispose(escMenuDialog);
            }
            base.Dispose(disposing);
        }
        
        // The screen's message box modal instance.
        private readonly EscMenuDialog escMenuDialog;
    }
}
