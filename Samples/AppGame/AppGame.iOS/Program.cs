using System;
using System.Diagnostics;
using System.Linq;
using AppGame.Shared;
using AppGame.Shared.Scenes;
using Foundation;
using UIKit;

namespace AppGame.iOS
{
    [Register("AppDelegate")]
    internal class Program : UIApplicationDelegate
    {
        public static SampleGame Game;
        public override UIWindow Window
        {
            get;
            set;
        }
        internal static void RunGame()
        {
            Game = new SampleGame();
            Game.Run();

            var windowCount = UIApplication.SharedApplication.Windows.Count();
            if (windowCount > 1)
            {
                Console.WriteLine("Multiple windows found...");

                UIApplication.SharedApplication.Windows[1].Hidden = true;
                UIApplication.SharedApplication.Windows[0].MakeKeyAndVisible();
            }
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
           RunGame();
        }
    }
}