using System;
using Sedulous.Content;
using Sedulous.UI;

namespace UpfGame.UI.Screens
{
    public class GamePlayScreen : UIScreen
    {
        public GamePlayScreen(ContentManager globalContent)
            : base("Content/UI/Screens/GamePlayScreen", "GamePlayScreen", globalContent)
        { }

        protected override Object CreateViewModel(UIView view) => new GamePlayViewModel();
    }
}
