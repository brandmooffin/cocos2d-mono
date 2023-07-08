using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace AppGame.iOS;

[Register ("MainViewController")]
public class MainViewController : UIViewController
{
    public MainViewController(IntPtr handle) : base(handle)
    {
    }

    public MainViewController(ObjCRuntime.NativeHandle handle) : base(handle)
    {

    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        View.BackgroundColor = UIColor.White;

        var simpleGameButton = UIButton.FromType(UIButtonType.System);
        simpleGameButton.Frame = new CGRect(20, 200, 280, 44);
        simpleGameButton.SetTitle("Simple Game", UIControlState.Normal);

        var interactiveGameButton = UIButton.FromType(UIButtonType.System);
        interactiveGameButton.Frame = new CGRect(20, 400, 280, 44);
        interactiveGameButton.SetTitle("Interactive Game", UIControlState.Normal);

        var spritesheetGameButton = UIButton.FromType(UIButtonType.System);
        spritesheetGameButton.Frame = new CGRect(20, 500, 280, 44);
        spritesheetGameButton.SetTitle("Game using spritesheet", UIControlState.Normal);

        var texturePackerGameButton = UIButton.FromType(UIButtonType.System);
        texturePackerGameButton.Frame = new CGRect(20, 600, 280, 44);
        texturePackerGameButton.SetTitle("Game using TexturePacker", UIControlState.Normal);


        simpleGameButton.TouchUpInside += (sender, e) => {
            var simpleGameStoryboard = UIStoryboard.FromName("SimpleGameStoryboard", null);
            var simpleGameViewController = simpleGameStoryboard.InstantiateViewController("SimpleGame") as SimpleGameViewController;
            PresentViewController(simpleGameViewController, true, null);
        };

        interactiveGameButton.TouchUpInside += (sender, e) => {
            var interactiveGameStoryboard = UIStoryboard.FromName("InteractiveGameStoryboard", null);
            var interactiveGameViewController = interactiveGameStoryboard.InstantiateViewController("InteractiveGame") as InteractiveGameViewController;
            PresentViewController(interactiveGameViewController, true, null);
        };

        spritesheetGameButton.TouchUpInside += (sender, e) => {
            var spritesheetGameStoryboard = UIStoryboard.FromName("SpritesheetGameStoryboard", null);
            var spritesheetGameViewController = spritesheetGameStoryboard.InstantiateViewController("SpritesheetGame") as SpritesheetGameViewController;
            PresentViewController(spritesheetGameViewController, true, null);
        };

        texturePackerGameButton.TouchUpInside += (sender, e) => {
            var texturePackerGameStoryboard = UIStoryboard.FromName("TexturePackerGameStoryboard", null);
            var texturePackerGameViewController = texturePackerGameStoryboard.InstantiateViewController("TexturePackerGame") as TexturePackerGameViewController;
            PresentViewController(texturePackerGameViewController, true, null);
        };

        View.AddSubview(simpleGameButton);
        View.AddSubview(interactiveGameButton);
        View.AddSubview(spritesheetGameButton);
        View.AddSubview(texturePackerGameButton);
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