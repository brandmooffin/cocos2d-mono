using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AppGame.Shared;
using AppGame.Shared.Scenes;
using Cocos2D;
using Microsoft.Xna.Framework;
using OpenTK.Windowing.Desktop;
using System;

namespace AppGame.Android;

[Activity(Label = "IntroGameActivity")]
public class IntroGameActivity : AndroidGameActivity
{
    private SampleGame _game;
    private View _view;
    protected override void OnCreate(Bundle savedInstanceState)
    {
        Console.WriteLine("Creating game activity...");
        base.OnCreate(savedInstanceState);

        _game = new SampleGame(new IntroScene());
        _view = _game.Services.GetService(typeof(View)) as View;

        SetContentView(_view);
        _game.Run();
    }

    public override void OnBackPressed()
    {
        Console.WriteLine("Back pressed...");
        base.OnBackPressed();
       // Finish();
    }

    protected override void OnDestroy()
    {
        Console.WriteLine("Destroy activity...");
        base.OnDestroy();

        //_game.Exit();
        //_game.Dispose();

        //CCDirector.SharedDirector.End();
        // CCDirector.SharedDirector.PurgeCachedData();

        //CCApplication.SharedApplication.Dispose();
        //CCApplication.SharedApplication.GraphicsDevice.Dispose();
    }
}