using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
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
    public class AppActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_main);

            Button introGameButton = FindViewById<Button>(Resource.Id.introGameButton);
            introGameButton.Click += OnIntroGameClick;

            Button startGameButton = FindViewById<Button>(Resource.Id.startGameButton);
            startGameButton.Click += OnStartGameClick;
        }

        private void OnIntroGameClick(object sender, EventArgs eventArgs)
        {
            StartActivity(typeof(IntroGameActivity));
        }

        private void OnStartGameClick(object sender, EventArgs eventArgs)
        {
            StartActivity(typeof(GameActivity));
        }
    }
}
