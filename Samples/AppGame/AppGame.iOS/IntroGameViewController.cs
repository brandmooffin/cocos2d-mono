using System;
using System.Linq;
using AppGame.Shared;
using AppGame.Shared.Scenes;
using CarPlay;
using Foundation;
using SpriteKit;
using UIKit;

namespace AppGame.iOS;

[Register ("IntroGameViewController")]
public class IntroGameViewController : UIViewController {
    UIWindow AppWindow;

    public IntroGameViewController(IntPtr handle) : base(handle)
    {
    }


    public override void ViewDidLoad ()
    {
        View = new UIView {
            BackgroundColor = UIColor.Red,
        };

        base.ViewDidLoad ();
        Console.WriteLine("View did load...");

        AppWindow = UIApplication.SharedApplication.Delegate.GetWindow();

        var game = new SampleGame(new SimpleScene(), this);
        game.Run();
    }

    public override void ViewWillAppear(bool animated)
    {
        base.ViewWillAppear(animated);
        Console.WriteLine("View will appear...");
    }

    public override void ViewDidAppear(bool animated)
    {
        base.ViewDidAppear(animated);
        Console.WriteLine("View did appear...");

        if (SampleGame.IsExiting)
        {
            SampleGame.IsExiting = false;
            BecomeFirstResponder();

            DismissViewController(true, null);
            RemoveFromParentViewController();
            ResignFirstResponder();

            var windowCount = UIApplication.SharedApplication.Windows.Count();
            if (windowCount > 1) {
                Console.WriteLine("Multiple windows found...");
                AppWindow.BecomeKeyWindow();
                AppWindow.BecomeFirstResponder();
                UIApplication.SharedApplication.Delegate.SetWindow(AppWindow);

                UIApplication.SharedApplication.Windows.ToList().ForEach(window =>
                {
                    if (window != AppWindow)
                    {
                        window.WindowScene = null;
                        window = null;
                    }
                });
            }
        }
    }
}

