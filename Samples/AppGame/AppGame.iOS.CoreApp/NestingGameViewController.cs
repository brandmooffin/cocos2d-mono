using AppGame.Shared;
using AppGame.Shared.Scenes;

namespace AppGame.iOS.CoreApp
{
    [Register("NestingGameViewController")]
    public class NestingGameViewController : UIViewController
    {
        UILabel descriptionLabel;

        public override void ViewDidLoad()
        {
            View = new UIView
            {
                BackgroundColor = UIColor.Red,
            };

            base.ViewDidLoad();
            Console.WriteLine("View did load...");

            var windowCount = UIApplication.SharedApplication.Windows.Count();
            if (windowCount > 1)
            {
                UIApplication.SharedApplication.Windows[0].MakeKeyAndVisible();
            }

            AppDelegate.Game.LoadGameScene(new NestingScene(), this);

            var viewController = AppDelegate.Game.Services.GetService(typeof(UIViewController)) as UIViewController;

            var viewHeight = viewController.View.Frame.Size.Height;
            var viewWidth = viewController.View.Frame.Size.Width;

            descriptionLabel = new UILabel(new CGRect(0, viewHeight - 100, viewWidth, 44))
            {
                Text = "Tap on the screen to add a new sprite. Interactive example with custom font. (This is a native label)",
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

                DismissViewController(true, null);
                RemoveFromParentViewController();

                var windowCount = UIApplication.SharedApplication.Windows.Count();
                if (windowCount > 1)
                {
                    Console.WriteLine("Multiple windows found...");

                    descriptionLabel.RemoveFromSuperview();

                    UIApplication.SharedApplication.Windows[0].Hidden = true;
                    UIApplication.SharedApplication.Windows[1].MakeKeyAndVisible();
                }
            }
        }
    }
}

