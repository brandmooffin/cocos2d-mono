using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using AppGame.Android.Games;
using AppGame.Shared;
using Microsoft.Xna.Framework;
using System;

namespace AppGame.Android
{
    [Activity(
        Label = "@string/app_name",
        MainLauncher = true,
        Icon = "@drawable/icon",
        Theme = "@style/Theme.Splash",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize
    )]
    public class AppActivity : AndroidGameActivity
    {
        public static SampleGame Game;

        protected override void OnCreate(Bundle bundle)
        {

            base.OnCreate(bundle);

            Game = new SampleGame();
            Game.Run();

            SetContentView(Resource.Layout.activity_main);

            Button simpleGameButton = FindViewById<Button>(Resource.Id.simpleGameButton);
            simpleGameButton.Click += OnSimpleGameClick;

            Button interactiveGameButton = FindViewById<Button>(Resource.Id.interactiveGameButton);
            interactiveGameButton.Click += OnInteractiveGameClick;

            Button spritesheetGameButton = FindViewById<Button>(Resource.Id.spritesheetGameButton);
            spritesheetGameButton.Click += OnSpritesheetGameClick;

            Button texturePackerGameButton = FindViewById<Button>(Resource.Id.texturePackerGameButton);
            texturePackerGameButton.Click += OnTexturePackerGameClick;

            Button nestingGameButton = FindViewById<Button>(Resource.Id.nestingGameButton);
            nestingGameButton.Click += OnNestingGameClick;
        }

        private void OnSimpleGameClick(object sender, EventArgs eventArgs)
        {
            StartActivity(typeof(SimpleGameActivity));
        }

        private void OnInteractiveGameClick(object sender, EventArgs eventArgs)
        {
            StartActivity(typeof(InteractiveGameActivity));
        }

        private void OnSpritesheetGameClick(object sender, EventArgs eventArgs)
        {
            StartActivity(typeof(SpritesheetGameActivity));
        }

        private void OnTexturePackerGameClick(object sender, EventArgs eventArgs)
        {
            StartActivity(typeof(TexturePackerGameActivity));
        }

        private void OnNestingGameClick(object sender, EventArgs eventArgs)
        {
            StartActivity(typeof(NestingGameActivity));
        }
    }
}
