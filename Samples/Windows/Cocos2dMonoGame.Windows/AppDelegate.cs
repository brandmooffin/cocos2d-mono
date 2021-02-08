using System;
using Cocos2D;
using CocosDenshion;
using Microsoft.Xna.Framework;

namespace Cocos2DMonoGame.Windows
{
    /// <summary>
    /// This is your extension of the main Cocos2D application object.
    /// </summary>
    internal class AppDelegate : CCApplication
    {
        public AppDelegate(Game game, GraphicsDeviceManager graphics)
            : base(game, graphics)
        {
            s_pSharedApplication = this;
            //
            // TODO: Set the display orientation that you want for this game.
            // 
            CCDrawManager.InitializeDisplay(game, graphics, DisplayOrientation.Portrait);
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
            CCSimpleAudioEngine.SharedEngine.SaveMediaState();

            CCDirector pDirector = null;
            try
            {
                // Set your design resolution here, which is the target resolution of your primary
                // design hardware.
                //
                CCDrawManager.SetDesignResolutionSize(1280f, 720f, CCResolutionPolicy.ShowAll);
                CCApplication.SharedApplication.GraphicsDevice.Clear(Color.Black);
                //initialize director
                pDirector = CCDirector.SharedDirector;
                pDirector.SetOpenGlView();

                //turn on display FPS
                pDirector.DisplayStats = false;

                // set FPS. the default value is 1.0/60 if you don't call this
                pDirector.AnimationInterval = 1.0 / 60;

                CCScene pScene = IntroLayer.Scene;

                pDirector.RunWithScene(pScene);
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
        }
    }
}