using System;
using AppGame.Shared;
using AppGame.Shared.Scenes;
using Foundation;
using UIKit;

namespace AppGame.iOS;

[Register ("InteractionGameViewController")]
public class InteractionGameViewController : UIViewController {

    public InteractionGameViewController(IntPtr handle) : base(handle)
    {
    }


    public override void ViewDidLoad ()
    {
        View = new UIView {
            BackgroundColor = UIColor.Red,
        };

        base.ViewDidLoad ();

        var game = new SampleGame(new InteractionScene(), this);
        game.Run();
    }
}

