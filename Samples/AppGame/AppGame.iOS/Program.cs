using System.Diagnostics;
using AppGame.Shared;
using AppGame.Shared.Scenes;
using Foundation;
using UIKit;

namespace AppGame.iOS
{
    [Register("AppDelegate")]
    internal class Program : UIApplicationDelegate
    {
        // private static SampleGame game;
        public override UIWindow Window
        {
            get;
            set;
        }
        internal static void RunGame()
        {
          //  game = new SampleGame(new IntroScene());
          //  game.Run();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            UIApplication.Main(args, null, "AppDelegate");
        }

        public override void FinishedLaunching(UIApplication app)
        {
           // RunGame();
        }
    }
}