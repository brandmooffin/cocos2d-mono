using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidGraphics = Android.Graphics;
using AppGame.Shared.Scenes;
using Microsoft.Xna.Framework;
using System;

namespace AppGame.Android.Games
{
    [Activity(Label = "InteractiveGameActivity")]
    public class InteractiveGameActivity : AndroidGameActivity
    {
        private View _view;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Console.WriteLine("Creating interactive game activity...");
            base.OnCreate(savedInstanceState);

            AppActivity.Game.LoadGameScene(new InteractiveScene(), this);
            _view = AppActivity.Game.Services.GetService(typeof(View)) as View;

            SetContentView(_view);

            TextView descriptionTextView = new TextView(this)
            {
                Text = "Tap on the screen to add a new sprite. Interactive example with custom font. (This is a native label)",
                TextSize = 28,
                Gravity = GravityFlags.Center | GravityFlags.Bottom,
            };
            descriptionTextView.SetTextColor(AndroidGraphics.Color.White);
            AddContentView(descriptionTextView, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
        }

        public override void OnBackPressed()
        {
            Console.WriteLine("Back pressed...");
            base.OnBackPressed();
        }

        public void PrepareForDestroy()
        {
            ((ViewGroup)_view.Parent).RemoveView(_view);
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("Destroy activity...");
            PrepareForDestroy();
            base.OnDestroy();
        }
    }
}