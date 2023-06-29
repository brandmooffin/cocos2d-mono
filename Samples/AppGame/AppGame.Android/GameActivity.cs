using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Microsoft.Xna.Framework;
using System;

namespace AppGame.Android;

[Activity(Label = "GameActivity")]
public class GameActivity : AndroidGameActivity
{
    private Game1 _game;
    private View _view;
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        _game = new Game1();
        _view = _game.Services.GetService(typeof(View)) as View;

        SetContentView(_view);
        _game.Run();
    }
}