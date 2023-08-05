using AppGame.Shared;

namespace AppGame.iOS.CoreApp;

[Register ("AppDelegate")]
public class AppDelegate : UIApplicationDelegate {
    public static SampleGame Game;

    public override UIWindow? Window {
        get;
        set;
    }

    public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
    {
        Game = new SampleGame();
        Game.Run();

        // create a new window instance based on the screen size
        Window = new UIWindow (UIScreen.MainScreen.Bounds);

        // create a UIViewController with a single UILabel
        var vc = new MainViewController();
        Window.RootViewController = vc;

        // make the window visible
        Window.MakeKeyAndVisible ();

        return true;
    }
}

