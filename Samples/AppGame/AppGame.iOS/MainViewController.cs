using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Threading.Tasks;
using AppGame.Shared;
using AppGame.Shared.Scenes;
using CoreGraphics;
using Foundation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using UIKit;

namespace AppGame.iOS;

[Register ("MainViewController")]
public class MainViewController : UIViewController
{
    public MainViewController(IntPtr handle) : base(handle)
    {
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        View.BackgroundColor = UIColor.White;

        var StartGameButton = UIButton.FromType(UIButtonType.System);
        StartGameButton.Frame = new CGRect(20, 200, 280, 44);
        StartGameButton.SetTitle("Start Game", UIControlState.Normal);

        var IntroGameButton = UIButton.FromType(UIButtonType.System);
        IntroGameButton.Frame = new CGRect(20, 400, 280, 44);
        IntroGameButton.SetTitle("Intro Game", UIControlState.Normal);


        IntroGameButton.TouchUpInside += (sender, e) => {
            var introGameStoryboard = UIStoryboard.FromName("IntroGameStoryboard", null);
            var introGameViewController = introGameStoryboard.InstantiateViewController("IntroGame") as IntroGameViewController;
            PresentViewController(introGameViewController, true, null);
        };

        StartGameButton.TouchUpInside += (sender, e) => {
            var interactionGameStoryboard = UIStoryboard.FromName("InteractionGameStoryboard", null);
            var interactionGameViewController = interactionGameStoryboard.InstantiateViewController("InteractionGame") as InteractionGameViewController;
            PresentViewController(interactionGameViewController, true, null);
        };

        View.AddSubview(IntroGameButton);
        View.AddSubview(StartGameButton);
    }

    public override void DidReceiveMemoryWarning()
    {
        // Releases the view if it doesn't have a superview.
        base.DidReceiveMemoryWarning();

        // Release any cached data, images, etc that aren't in use.
    }

    #region View lifecycle

    public override void ViewWillAppear(bool animated)
    {
        base.ViewWillAppear(animated);
    }

    public override void ViewDidAppear(bool animated)
    {
        base.ViewDidAppear(animated);
    }

    public override void ViewWillDisappear(bool animated)
    {
        base.ViewWillDisappear(animated);
    }

    public override void ViewDidDisappear(bool animated)
    {
        base.ViewDidDisappear(animated);
    }

    #endregion
}