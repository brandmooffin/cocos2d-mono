using Cocos2D;
using CocosDenshion;
using Microsoft.Xna.Framework;
using System;
using TetrisGame.Core.Managers;
using TetrisGame.Core.Scenes;

namespace TetrisGame.DesktopGL
{
    /// <summary>
    /// This is your extension of the main Cocos2D application object.
    /// </summary>
    internal class AppDelegate : CCApplication
    {
        GraphicsDeviceManager graphics;
        public AppDelegate(Game game, GraphicsDeviceManager graphics)
            : base(game, graphics)
        {
            this.graphics = graphics;
            s_pSharedApplication = this;
            //
            // TODO: Set the display orientation that you want for this game.
            // 
            CCDrawManager.InitializeDisplay(game, graphics, DisplayOrientation.Default);
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
                CCApplication.SharedApplication.GraphicsDevice.Clear(Color.Black);
                //initialize director
               
                pDirector = CCDirector.SharedDirector;
                pDirector.SetOpenGlView();

                // Set your design resolution here, which is the target resolution of your primary
                // design hardware.
                //
                CCDrawManager.SetDesignResolutionSize(352f, 410f, CCResolutionPolicy.ExactFit);

                //turn on display FPS
                pDirector.DisplayStats = false;

                graphics.ApplyChanges();

                // set FPS. the default value is 1.0/60 if you don't call this
#if ANDROID
                pDirector.AnimationInterval = 1f / 30f;
#else
                pDirector.AnimationInterval = 1.0 / 60;
#endif
                AudioManager.Instance.PlayBackgroundMusic("main");
                CCScene pScene = new TetrisScene();

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
