using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using AppGame.Shared.Scenes;
using AndroidGraphics = Android.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace AppGame.Android.Games
{

    [Activity(Label = "NestingGameActivity")]
    public class NestingGameActivity : AndroidGameActivity
    {
        private View _view;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Console.WriteLine("Creating game activity...");
            base.OnCreate(savedInstanceState);

            _view = AppActivity.Game.Services.GetService(typeof(View)) as View;

            SetContentView(_view);

            AppActivity.Game.LoadGameScene(new NestingScene(), this);
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