using System;
using System.Linq;
using AppGame.Shared;
using AppGame.Shared.Scenes;
using CoreGraphics;
using Foundation;
using UIKit;

namespace AppGame.iOS;

[Register ("SpritesheetGameViewController")]
public class SpritesheetGameViewController : UIViewController {
    UIWindow AppWindow;
    public SpritesheetGameViewController(IntPtr handle) : base(handle)
    {
    }

    public override void ViewDidLoad ()
    {
        View = new UIView {
            BackgroundColor = UIColor.Red,
        };

        base.ViewDidLoad();
        Console.WriteLine("View did load...");

        AppWindow = UIApplication.SharedApplication.Delegate.GetWindow();

        var game = new SampleGame(new SpritesheetScene(), this);
        game.Run();

        var viewController = game.Services.GetService(typeof(UIViewController)) as UIViewController;

        var viewHeight = viewController.View.Frame.Size.Height;
        var viewWidth = viewController.View.Frame.Size.Width;

        var descriptionLabel = new UILabel(new CGRect(0, viewHeight - 100, viewWidth, 44))
        {
            Text = "Simple exaple loading sprites from a spritesheet. (This is a native label)",
            TextColor = UIColor.White,
            Lines = 2,
            LineBreakMode = UILineBreakMode.WordWrap,
            TextAlignment = UITextAlignment.Center,
            MinimumFontSize = 28.0f
        };
        viewController.View.AddSubview(descriptionLabel);
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
            if (windowCount > 1)
            {
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

