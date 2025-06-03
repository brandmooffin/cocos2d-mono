---
sidebar_position: 2
---

# Part 1: Project Setup and Assets

In this first part of the platformer tutorial, we'll set up our cocos2d-mono project and prepare the assets we'll need for our game.

## What We'll Accomplish

By the end of this part, you'll have:
- A working cocos2d-mono DesktopGL project
- Game assets properly configured
- A basic scene displaying a background
- Understanding of the project structure

## Prerequisites

Before you begin, make sure you have:
- Visual Studio 2019 or newer installed
- .NET Core SDK installed
- Basic C# knowledge
- cocos2d-mono installed (see [Environment Setup](../../getting-started/environment-setup.md))

## Step 1: Create a New DesktopGL Project

1. Open Visual Studio and select "Create a new project"
2. Search for "cocos2d-mono" and select the "Cocos2D-Mono for DesktopGL (OpenGL Desktop Platforms)" template
3. Name your project "Platformer" and click "Create"

If you don't have the template installed, you can:
- Install the [Visual Studio Extension](https://marketplace.visualstudio.com/items?itemName=Cocos2D-MonoTeamBrokenWallsStudios.cocos2dmonoprojecttemplates)
- Install the dotnet templates by running: `dotnet new install Cocos2DMono.Samples` and then create a new project with `dotnet new c2mdesktopgl -n Platformer`
- Or create a new MonoGame DesktopGL project and add cocos2d-mono.DesktopGL through NuGet packages

## Step 2: Understanding Project Structure

Once created, your project will have the following structure:

```
Platformer/
├── Content/           - Game assets like images and sounds
├── AppDelegate.cs     - cocos2d-mono initialization and configuration
├── Game1.cs           - Main entry point and game loop
├── IntroLayer.cs      - Default template layer (you'll delete this)
├── Program.cs         - Program initialization
└── app.manifest       - Application manifest file
```

> **Note:** The template includes `IntroLayer.cs`, but we'll be replacing it with our own `GameLayer.cs` in this tutorial. You can delete `IntroLayer.cs` once you've created the `GameLayer.cs` file.

For our platformer, we'll eventually add several classes to organize our code:
- `GameLayer.cs` - Main game logic
- `Player.cs` - Player character implementation
- `Platform.cs` - Platform objects
- `Collectible.cs` - Coin object
- `PhysicsHelper.cs` - Helper class for Box2D physics

## Step 3: Preparing Game Assets

For this tutorial, you'll need a few basic assets:

1. Player character (sprite sheet or individual images)
2. Platform tiles
3. Background image
4. Optional collectibles and obstacles

You can create these yourself or use free assets from sites like:
- [OpenGameArt](https://opengameart.org/)
- [Kenney Assets](https://kenney.nl/assets) (recommended for beginners)
- [itch.io](https://itch.io/game-assets/free)

For this tutorial, we recommend downloading these sample assets:
- A simple character sprite (64x64px)
- Platform tiles (32x32px)
- Background image (800x600px)

## Step 4: Setting Up Content Pipeline

In order to add content to your project, not only should the assets exist within the `Content` folder but they must be registered in the `Content.mgcb` file.

[MGCB Editor](https://docs.monogame.net/articles/getting_started/tools/mgcb_editor.html) is the easiest way to do this and can be installed as a dotnet tool:

```bash
dotnet tool install --global dotnet-mgcb-editor
```

### Adding Content to Your Project

The cocos2d-mono template already includes a Content.mgcb file in the Content folder, so you can proceed with adding your assets:

1. Double-click the Content.mgcb file in the Content folder to open the MonoGame Content Pipeline Tool
2. Add your assets using "Add" > "Existing Item" and select appropriate processors for each

NOTE: For more information on working with the MonoGame Content Pipeline Tool please check out MonoGame's docs [here](https://docs.monogame.net/articles/getting_started/content_pipeline/index.html).

Make sure to add these assets (use placeholder images if you don't have them yet):
- `background.png` - Background image
- `player_idle.png` - Player idle sprite
- `platform.png` - Platform tile
- `coin.png` - Collectible coin

## Step 5: Creating a Basic Game Scene

Now let's create a simple scene that displays our background. First, let's create our main game layer.

Create a new file called `GameLayer.cs`:

```csharp
using System;
using Cocos2D;

namespace Platformer
{
    public class GameLayer : CCLayer
    {
        public GameLayer()
        {
            // Get visible area size
            CCSize visibleSize = CCDirector.SharedDirector.WinSize;
            
            // Create background
            CCSprite background = new CCSprite("background");
            background.Position = new CCPoint(visibleSize.Width / 2, visibleSize.Height / 2);
            
            // Scale background to fit screen
            float scaleX = visibleSize.Width / background.ContentSize.Width;
            float scaleY = visibleSize.Height / background.ContentSize.Height;
            background.Scale = Math.Max(scaleX, scaleY);
            
            AddChild(background, -1);
            
            // Add a simple label to confirm everything is working
            CCLabelTTF label = new CCLabelTTF("Platformer Tutorial - Part 1", "MarkerFelt", 22);
            label.Position = new CCPoint(visibleSize.Width / 2, visibleSize.Height - 50);
            label.Color = CCColor3B.Blue;
            AddChild(label);
        }
    }
}
```

## Step 6: Update the Main Game Class

Now we need to update our main game class to use our new GameLayer. Update `AppDelegate.cs`:

```csharp
using System;
using Cocos2D;
using CocosDenshion;
using Microsoft.Xna.Framework;

namespace Platformer
{
    public class AppDelegate : CCApplication
    {
        public AppDelegate(Game game, GraphicsDeviceManager graphics)
            : base(game, graphics)
        {
            s_pSharedApplication = this;
            CCDrawManager.InitializeDisplay(game, graphics, DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight);
        }

        public override bool ApplicationDidFinishLaunching()
        {
            CCSimpleAudioEngine.SharedEngine.SaveMediaState();

            CCDirector pDirector = null;
            try
            {
                // Set design resolution
                CCDrawManager.SetDesignResolutionSize(800, 600, CCResolutionPolicy.ShowAll);
                CCApplication.SharedApplication.GraphicsDevice.Clear(Color.Black);
                
                // Initialize director
                pDirector = CCDirector.SharedDirector;
                pDirector.SetOpenGlView();

                // Turn on display FPS (optional, for debugging)
                pDirector.DisplayStats = true;

                // Set FPS
                pDirector.AnimationInterval = 1.0 / 60;
                
                // Create and run scene
                CCScene scene = new CCScene();
                scene.AddChild(new GameLayer());
                pDirector.RunWithScene(scene);
            }
            catch (Exception ex)
            {
                CCLog.Log("ApplicationDidFinishLaunching(): Error " + ex.ToString());
            }
            return true;
        }

        public override void ApplicationDidEnterBackground()
        {
            CCDirector.SharedDirector.Pause();
            CCSimpleAudioEngine.SharedEngine.PauseBackgroundMusic();
        }

        public override void ApplicationWillEnterForeground()
        {
            CCDirector.SharedDirector.Resume();
            CCSimpleAudioEngine.SharedEngine.ResumeBackgroundMusic();
        }
    }
}
```

## Step 7: Build and Test

1. Make sure all source files are saved
2. Build the project by pressing F6 or selecting Build > Build Solution
3. Run the game by pressing F5 or selecting Debug > Start Debugging

If everything is set up correctly, you should see:
- Your background image filling the screen
- A label showing "Platformer Tutorial - Part 1"
- FPS counter in the corner (if enabled)

## Checkpoint: What Your Project Should Look Like

At this point, your project should have:

### Project Structure:
```
Platformer/
├── Content/
│   ├── Content.mgcb
│   ├── background.png
│   ├── player_idle.png
│   ├── platform.png
│   └── coin.png
├── GameLayer.cs
├── AppDelegate.cs
├── Game1.cs
└── Program.cs
```

### What You Should See:
- A window opens with your background image
- The title "Platformer Tutorial - Part 1" at the top
- Smooth 60 FPS performance
- No errors in the console

### Key Concepts Learned:
- How to set up a cocos2d-mono project
- Understanding the content pipeline
- Creating basic sprites and labels
- Setting up a game layer
- Managing screen resolution and scaling

## Troubleshooting

If you encounter issues:

1. **Content not loading**: Make sure your assets are added to Content.mgcb with the correct processor
2. **Black screen**: Check that your background asset exists and is properly named
3. **Build errors**: Ensure all using statements are correct and assemblies are referenced
4. **Performance issues**: Make sure you're running in Release mode for best performance

## Next Steps

Congratulations! You've successfully set up your platformer project and created a basic scene. In [Part 2: Physics Foundation](./part-2-physics), we'll add Box2D physics to our game and learn how to create a physics world with gravity.

## Download Checkpoint Project

You can download the complete Part 1 project [here](https://github.com/brandmooffin/cocos2d-mono-samples/tree/main/Tutorial%20Samples/Platformer/Checkpoints/Part%201) to compare with your implementation or use as a starting point.
