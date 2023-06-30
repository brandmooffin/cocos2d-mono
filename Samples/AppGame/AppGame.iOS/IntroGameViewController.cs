using System;
using AppGame.Shared;
using AppGame.Shared.Scenes;
using Foundation;
using UIKit;

namespace AppGame.iOS;

[Register ("IntroGameViewController")]
public class IntroGameViewController : UIViewController {

    public IntroGameViewController(IntPtr handle) : base(handle)
    {
    }


    public override void ViewDidLoad ()
    {
        View = new UIView {
            BackgroundColor = UIColor.Red,
        };

        base.ViewDidLoad ();

        var game = new SampleGame(new IntroScene(), this);
        game.Run();
    }
}

