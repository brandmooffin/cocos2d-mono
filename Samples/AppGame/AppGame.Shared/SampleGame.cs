using Cocos2D;
using CocosDenshion;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
#if ANDROID
using Android.App;
#endif
#if IOS
using AppGame.iOS;
using UIKit;
#endif

namespace AppGame.Shared
{
    public class SampleGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        AppDelegate application;

        CCScene sceneToLoad;

        public static bool IsExiting = false;
#if IOS

        UIViewController AppViewController;
#endif

#if ANDROID
        Activity AppViewActivity;
#endif
        public SampleGame()
        {
            Console.WriteLine("Initializing game & delegate...");
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            // This is the main Cocos2D connection. The CCApplication is the manager of the
            // nodes that define your game.
            //
            application = new AppDelegate(this, graphics, sceneToLoad);
            this.Components.Add(application);
        }

#if ANDROID
        public void LoadGameScene(CCScene sceneToLoad, Activity appViewActivity) {
            AppViewActivity = appViewActivity;
            this.sceneToLoad = sceneToLoad;

            application.LoadGameScene(sceneToLoad);
        }
#endif
#if IOS
        public void LoadGameScene(CCScene sceneToLoad, UIViewController appViewController) {
            AppViewController = appViewController;
            this.sceneToLoad = sceneToLoad;

            application.LoadGameScene(sceneToLoad);
        }
#endif

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Console.WriteLine("Game initializing...");

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Console.WriteLine("Game loading content...");
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Console.WriteLine("Game unloading content...");
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (CCDirector.SharedDirector.IsPaused) {
                return;
            }
#if !__IOS__ && !__TVOS__
            // For Mobile devices, this logic will close the Game when the Back button is pressed
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Console.WriteLine("Attempting to exit game...");
                ExitGame();
                return;
            }
#endif

            if (IsExiting) {
#if ANDROID
                IsExiting = false;
#endif
                Console.WriteLine("Attempting to exit game...");
                ExitGame();
                return;
            }

            //Console.WriteLine("Game running...");

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (CCDirector.SharedDirector.IsPaused)
            {
                return;
            }

            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        public void ExitGame()
        {
            Console.WriteLine("Game exiting...");
            // TODO: add your exit code here to restore the device to its per-game environment.
            CCSimpleAudioEngine.SharedEngine.RestoreMediaState();

            Console.WriteLine("Exiting game...");

            CCDirector.SharedDirector.RunningScene.RemoveAllChildren();
            CCDirector.SharedDirector.RunningScene.Visible = false;

#if ANDROID
            AppViewActivity.Finish();
#endif
#if IOS
            AppViewController.ViewDidAppear(false);
#endif
        }
    }
}