using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Cocos2D;

namespace tests
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager graphics;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";

            graphics.IsFullScreen = false;
            graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;

#if WINDOWS || MACOS
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
#endif

            // Frame rate is 30 fps by default for Windows Phone.
            // Divide by 2 to make it 60 fps
            TargetElapsedTime = TimeSpan.FromTicks(333333 / 2);
            IsFixedTimeStep = true;

            IsMouseVisible = true;

#if WINDOWS || WINDOWSGL || WINDOWSDX || MACOS || LINUX
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
#endif

            // Extend battery life under lock.
            //InactiveSleepTime = TimeSpan.FromSeconds(1);

            CCApplication application = new AppDelegate(this, graphics);
            Components.Add(application);

#if !WINDOWS_PHONE && !XBOX && !WINRT && !WINDOWSDX && !NETFX_CORE
            //GamerServicesComponent component = new GamerServicesComponent(this);
            //this.Components.Add(component);
#endif
        }

        void graphics_DeviceCreated(object sender, EventArgs e)
        {
            CCLog.Log("Graphics device was created!");
        }

        protected override void Update(GameTime gameTime)
        {
#if !IOS
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
#if WINRT || WINDOWS_STOREAPP || WINDOWS_PHONE8 || IOS
                return;
#elif ANDROID
                 Game.Activity.MoveTaskToBack(true);

#else
                Exit();
#endif
            }
#endif

            // TODO: Add your update logic here


            base.Update(gameTime);
        }
    }
}