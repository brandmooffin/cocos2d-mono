using Android.App;
using Android.OS;
using Android.Views;
using AppGame.Shared.Scenes;
using AndroidGraphics = Android.Graphics;
using Microsoft.Xna.Framework;
using System;
using Android.Widget;

namespace AppGame.Android.Games
{

    [Activity(Label = "SpritesheetGameActivity")]
    public class SpritesheetGameActivity : AndroidGameActivity
    {
        private View _view;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Console.WriteLine("Creating spritesheet game activity...");
            base.OnCreate(savedInstanceState);

            AppActivity.Game.LoadGameScene(new SpritesheetScene(), this);
            _view = AppActivity.Game.Services.GetService(typeof(View)) as View;

            SetContentView(_view);

            TextView descriptionTextView = new TextView(this)
            {
                Text = "Simple exaple loading sprites from a spritesheet. (This is a native label)",
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