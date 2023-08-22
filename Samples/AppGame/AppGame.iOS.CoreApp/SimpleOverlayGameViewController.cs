using AppGame.Shared;
using AppGame.Shared.Scenes;

namespace AppGame.iOS.CoreApp
{
    [Register("SimpleOverlayGameViewController")]
    public class SimpleOverlayGameViewController : UIViewController
    {
        UINavigationBar navbar;
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

            AppDelegate.Game.LoadGameScene(new SimpleOverlayScene(), this);

            var viewController = AppDelegate.Game.Services.GetService(typeof(UIViewController)) as UIViewController;

            var viewHeight = viewController.View.Frame.Size.Height;
            var viewWidth = viewController.View.Frame.Size.Width;

            navbar = new UINavigationBar(new CGRect(0, 50, viewWidth, 50))
            {
                BackgroundColor = UIColor.White,
                Items = new[] { new UINavigationItem("Simple Overlay") }
            };

         
            viewController.View.AddSubview(navbar);
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

                    navbar.RemoveFromSuperview();

                    UIApplication.SharedApplication.Windows[0].Hidden = true;
                    UIApplication.SharedApplication.Windows[1].MakeKeyAndVisible();
                }
            }
        }
    }
}

