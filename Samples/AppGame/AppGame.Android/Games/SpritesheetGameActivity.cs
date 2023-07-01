using Android.App;
using Android.OS;
using Android.Views;
using AppGame.Shared;
using AppGame.Shared.Scenes;
using AndroidGraphics = Android.Graphics;
using Cocos2D;
using Microsoft.Xna.Framework;
using System;
using Android.Widget;

namespace AppGame.Android.Games
{

    [Activity(Label = "SpritesheetGameActivity")]
    public class SpritesheetGameActivity : AndroidGameActivity
    {
        private SampleGame _game;
        private View _view;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Console.WriteLine("Creating spritesheet game activity...");
            base.OnCreate(savedInstanceState);

            _game = new SampleGame(new SpritesheetScene());
            _view = _game.Services.GetService(typeof(View)) as View;

            SetContentView(_view);
            _game.Run();

            TextView descriptionTextView = new TextView(this)
            {
                Text = "Simple exaple loading sprites from a spritesheet. (This is a native label).",
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

        protected override void OnDestroy()
        {
            Console.WriteLine("Destroy activity...");
            base.OnDestroy();

            CCDirector.SharedDirector.End();
        }
    }
}