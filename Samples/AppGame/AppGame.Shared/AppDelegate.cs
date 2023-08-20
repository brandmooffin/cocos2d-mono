using AppGame.Shared.Scenes;
using Cocos2D;
using CocosDenshion;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AppGame.Shared
{
    /// <summary>
    /// This is your extension of the main Cocos2D application object.
    /// </summary>
    internal class AppDelegate : CCApplication
    {
        GraphicsDeviceManager graphics;
        CCScene currentScene;
        public AppDelegate(Game game, GraphicsDeviceManager graphics, CCScene currentScene)
            : base(game, graphics)
        {
            Console.WriteLine("Initializing delegate...");
            this.graphics = graphics;
            s_pSharedApplication = this;
            //
            // TODO: Set the display orientation that you want for this game.
            // 
            CCDrawManager.InitializeDisplay(game, graphics, DisplayOrientation.Portrait);
            this.currentScene = currentScene;
        }

        public void LoadGameScene(CCScene sceneToLoad) {
            var pDirector = CCDirector.SharedDirector;
            currentScene = sceneToLoad;
            if (pDirector.RunningScene != null)
            {
                Console.WriteLine("Replacing running scene...");
                pDirector.ReplaceScene(currentScene);
            }
            else
            {
                Console.WriteLine("Running with scene...");
                pDirector.RunWithScene(currentScene);
            }
        }


        /// <summary>
        ///  Implement CCDirector and CCScene init code here.
        /// </summary>
        /// <returns>
        ///  true  Initialize success, app should continue.
        ///  false Initialize failed, app should terminate.
        /// </returns>
        public override bool ApplicationDidFinishLaunching()
        {
            Console.WriteLine("Launching game...");
            CCSimpleAudioEngine.SharedEngine.SaveMediaState();

            CCDirector pDirector = null;
            try
            {
                // Set your design resolution here, which is the target resolution of your primary
                // design hardware.
                //
                CCDrawManager.SetDesignResolutionSize(720, 1280, CCResolutionPolicy.ShowAll);
                CCApplication.SharedApplication.GraphicsDevice.Clear(Color.DarkRed);
                //initialize director
                pDirector = CCDirector.SharedDirector;
                pDirector.SetOpenGlView();

                //turn on display FPS
                pDirector.DisplayStats = false;

                // set FPS. the default value is 1.0/60 if you don't call this
                pDirector.AnimationInterval = 1.0 / 60;

                pDirector.ResetSceneStack();

                graphics.ApplyChanges();
                if (currentScene != null)
                {
                    if (pDirector.RunningScene != null)
                    {
                        Console.WriteLine("Replacing running scene...");
                        pDirector.ReplaceScene(currentScene);
                    }
                    else
                    {
                        Console.WriteLine("Running with scene...");
                        pDirector.RunWithScene(currentScene);
                    }
                }
            }
            catch (Exception ex)
            {
                CCLog.Log("ApplicationDidFinishLaunching(): Error " + ex.ToString());
            }
            return true;
        }

        /// <summary>
        /// The function be called when the application enter background
        /// </summary>
        public override void ApplicationDidEnterBackground()
        {
            base.ApplicationDidEnterBackground();
            //
            // TODO: Save the game state and pause your music
            //
            CCDirector.SharedDirector.Pause();

            Console.WriteLine("Pausing game...");
        }

        /// <summary>
        /// The function be called when the application enter foreground  
        /// </summary>
        public override void ApplicationWillEnterForeground()
        {
            base.ApplicationWillEnterForeground();
            //
            // reset the playback of audio
            //
            CCDirector.SharedDirector.ResumeFromBackground();

            Console.WriteLine("Resuming game...");
        }
    }
}
