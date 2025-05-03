---
sidebar_position: 5
---

# Building a 2D Platformer

This tutorial will guide you through creating a simple 2D platformer game using cocos2d-mono with DesktopGL. We'll cover project setup, implementing physics, character movement, collision detection, and more!

## Introduction

Platformers are one of the most popular game genres, featuring characters that jump between platforms, avoid obstacles, and collect items. In this tutorial, we'll build a simple yet functional 2D platformer using the cocos2d-mono framework and Box2D physics.

What you'll learn:
- Setting up a cocos2d-mono DesktopGL project
- Implementing Box2D physics for realistic movement
- Creating a player character with controls
- Building platforms and obstacles
- Implementing collision detection
- Adding game mechanics like jumping and double-jumping
- Creating a simple level design
- Adding game UI elements
- Polishing the game with effects and sound

## Prerequisites

Before you begin, make sure you have:
- Visual Studio 2019 or newer installed
- .NET Core SDK installed
- Basic C# knowledge
- cocos2d-mono installed (see [Environment Setup](../getting-started/environment-setup.md))

## Setting Up Your Project

### Create a New DesktopGL Project

1. Open Visual Studio and select "Create a new project"
2. Search for "cocos2d-mono" and select the "Cocos2D-Mono for DesktopGL (OpenGL Desktop Platforms)" template
3. Name your project "Platformer" and click "Create"

If you don't have the template installed, you can:
- Install the [Visual Studio Extension](https://marketplace.visualstudio.com/items?itemName=Cocos2D-MonoTeamBrokenWallsStudios.cocos2dmonoprojecttemplates)
- Or create a new MonoGame DesktopGL project and add cocos2d-mono.DesktopGL through NuGet packages

### Project Structure Overview

Once created, your project will have the following structure:

```
Platformer/
├── Content/ - Game assets like images and sounds
├── Game1.cs - Main entry point and game loop
├── Program.cs - Program initialization
└── app.manifest - Application manifest file
```

For our platformer, we'll add several classes to organize our code:
- `GameLayer.cs` - Main game logic
- `Player.cs` - Player character implementation
- `Platform.cs` - Platform objects
- `PhysicsHelper.cs` - Helper class for Box2D physics

## Creating Game Assets

For this tutorial, you'll need a few basic assets:

1. Player character (sprite sheet or individual images)
2. Platform tiles
3. Background image
4. Optional collectibles and obstacles

You can create these yourself or use free assets from sites like:
- [OpenGameArt](https://opengameart.org/)
- [Kenney Assets](https://kenney.nl/assets) (recommended for beginners)
- [itch.io](https://itch.io/game-assets/free)

In order to add content to your project, not only should the assets exist within the `Content` folder but they must be registered in the `Content.mgcb` file.

[MGCB Editor](https://docs.monogame.net/articles/getting_started/tools/mgcb_editor.html) is the easiest way to do this and can be installed as a dotnet tool.

```
dotnet tool install --global dotnet-mgcb-editor
```

### Setting Up Content Pipeline

1. In Visual Studio, right-click the "Content" folder
2. Select "Add" > "New Item" > "MonoGame Content Item"
3. Name it "Content.mgcb"
4. Double-click the created file to open the MonoGame Content Pipeline Tool
5. Add your assets using "Add" > "Existing Item" and select appropriate processors for each

For this tutorial, download these sample assets or use your own:
- A simple character sprite (64x64px)
- Platform tiles (32x32px)
- Background image (800x600px)

## Implementing Box2D Physics

Box2D is included with cocos2d-mono and will help us implement realistic physics for our platformer.

Create a new file called `PhysicsHelper.cs`:

```csharp
using Cocos2D;
using Box2D.Dynamics;
using Box2D.Collision.Shapes;
using Box2D.Common;

namespace Platformer
{
    public static class PhysicsHelper
    {
        // Physics constants
        public const float PTM_RATIO = 32.0f; // Pixels to meters ratio
        
        // Categories for collision filtering
        public const ushort CATEGORY_PLAYER = 0x0001;
        public const ushort CATEGORY_PLATFORM = 0x0002;
        public const ushort CATEGORY_COLLECTIBLE = 0x0004;
        
        // Convert from cocos2d coordinates to Box2D coordinates
        public static b2Vec2 ToPhysicsVector(CCPoint point)
        {
            return new b2Vec2(point.X / PTM_RATIO, point.Y / PTM_RATIO);
        }
        
        // Convert from Box2D coordinates to cocos2d coordinates
        public static CCPoint ToCocosVector(b2Vec2 vector)
        {
            return new CCPoint(vector.x * PTM_RATIO, vector.y * PTM_RATIO);
        }
        
        // Create a rectangular physics body
        public static b2Body CreateBoxBody(b2World world, float x, float y, float width, float height, 
            bool isDynamic = false, float density = 1.0f, float friction = 0.3f, float restitution = 0.1f)
        {
            // Define body
            b2BodyDef bodyDef = new b2BodyDef();
            bodyDef.position = new b2Vec2(x / PTM_RATIO, y / PTM_RATIO);
            bodyDef.type = isDynamic ? b2BodyType.b2_dynamicBody : b2BodyType.b2_staticBody;
            
            // Create body
            b2Body body = world.CreateBody(bodyDef);
            
            // Define fixture
            b2PolygonShape shape = new b2PolygonShape();
            shape.SetAsBox(width / (2 * PTM_RATIO), height / (2 * PTM_RATIO));
            
            b2FixtureDef fixtureDef = new b2FixtureDef();
            fixtureDef.shape = shape;
            fixtureDef.density = density;
            fixtureDef.friction = friction;
            fixtureDef.restitution = restitution;
            
            // Add fixture to body
            body.CreateFixture(fixtureDef);
            
            return body;
        }
    }
}
```

## Implementing the Player Character

Create `Player.cs`:

```csharp
using System;
using Cocos2D;
using Box2D.Dynamics;
using Box2D.Common;
using Box2D.Collision.Shapes;

namespace Platformer
{
    public class Player : CCSprite
    {
        // Physics body
        private b2Body _body;
        
        // Movement parameters
        private const float MOVE_SPEED = 5.0f;
        private const float JUMP_FORCE = 7.0f;
        private bool _canJump = false;
        private int _jumpCount = 0;
        private const int MAX_JUMPS = 2; // Allow double jump
        
        // Animation states
        private CCAnimation _idleAnimation;
        private CCAnimation _runAnimation;
        private CCAnimation _jumpAnimation;
        
        public Player(b2World world) : base("player_idle")
        {
            // Create physics body
            b2BodyDef bodyDef = new b2BodyDef();
            bodyDef.type = b2BodyType.b2_dynamicBody;
            bodyDef.fixedRotation = true; // Prevent rotation
            bodyDef.allowSleep = false;
            
            _body = world.CreateBody(bodyDef);
            
            // Create fixture
            b2PolygonShape shape = new b2PolygonShape();
            // Make the collision box slightly smaller than the sprite
            shape.SetAsBox(
                this.ContentSize.Width * 0.4f / PhysicsHelper.PTM_RATIO,
                this.ContentSize.Height * 0.45f / PhysicsHelper.PTM_RATIO);
            
            b2FixtureDef fixtureDef = new b2FixtureDef();
            fixtureDef.shape = shape;
            fixtureDef.density = 1.0f;
            fixtureDef.friction = 0.2f;
            fixtureDef.restitution = 0.0f;
            
            // Set collision filtering
            fixtureDef.filter.categoryBits = PhysicsHelper.CATEGORY_PLAYER;
            fixtureDef.filter.maskBits = PhysicsHelper.CATEGORY_PLATFORM | PhysicsHelper.CATEGORY_COLLECTIBLE;
            
            _body.CreateFixture(fixtureDef);
            
            // Add foot sensor for jump detection
            b2PolygonShape footShape = new b2PolygonShape();
            footShape.SetAsBox(
                this.ContentSize.Width * 0.3f / PhysicsHelper.PTM_RATIO, 
                0.1f / PhysicsHelper.PTM_RATIO,
                new b2Vec2(0, -this.ContentSize.Height * 0.45f / PhysicsHelper.PTM_RATIO),
                0);
                
            b2FixtureDef footFixtureDef = new b2FixtureDef();
            footFixtureDef.shape = footShape;
            footFixtureDef.isSensor = true;
            
            b2Fixture footSensor = _body.CreateFixture(footFixtureDef);
            footSensor.UserData = new FootSensorUserData(this);
            
            // Load animations
            LoadAnimations();
        }
        
        private void LoadAnimations()
        {
            // In a real game, you would load animation frames
            // For this tutorial, we'll use placeholder logic
            
            _idleAnimation = new CCAnimation();
            // Add frames to animation
            _idleAnimation.AddSpriteFrameWithFileName("player_idle");
            _idleAnimation.DelayPerUnit = 0.2f;
            
            _runAnimation = new CCAnimation();
            // Add multiple frames for run animation
            _runAnimation.AddSpriteFrameWithFileName("player_run1");
            _runAnimation.AddSpriteFrameWithFileName("player_run2");
            _runAnimation.AddSpriteFrameWithFileName("player_run3");
            _runAnimation.AddSpriteFrameWithFileName("player_run4");
            _runAnimation.DelayPerUnit = 0.1f;
            
            _jumpAnimation = new CCAnimation();
            _jumpAnimation.AddSpriteFrameWithFileName("player_jump");
            _jumpAnimation.DelayPerUnit = 0.1f;
        }
        
        public void Update(float dt)
        {
            // Update sprite position based on physics body
            this.Position = PhysicsHelper.ToCocosVector(_body.Position);
            
            // Check if player fell off the screen
            if (this.Position.Y < -100)
            {
                // Reset position
                _body.SetTransform(new b2Vec2(100 / PhysicsHelper.PTM_RATIO, 300 / PhysicsHelper.PTM_RATIO), 0);
                _body.LinearVelocity = b2Vec2.Zero;
            }
        }
        
        public void MoveLeft()
        {
            _body.LinearVelocity = new b2Vec2(-MOVE_SPEED, _body.LinearVelocity.y);
            
            // Flip sprite to face left
            this.ScaleX = -Math.Abs(this.ScaleX);
            
            // Play run animation if on ground
            if (_canJump && _jumpCount == 0)
            {
                this.RunAction(new CCRepeatForever(new CCAnimate(_runAnimation)));
            }
        }
        
        public void MoveRight()
        {
            _body.LinearVelocity = new b2Vec2(MOVE_SPEED, _body.LinearVelocity.y);
            
            // Flip sprite to face right
            this.ScaleX = Math.Abs(this.ScaleX);
            
            // Play run animation if on ground
            if (_canJump && _jumpCount == 0)
            {
                this.RunAction(new CCRepeatForever(new CCAnimate(_runAnimation)));
            }
        }
        
        public void StopMoving()
        {
            _body.SetLinearVelocity(new b2Vec2(0, _body.GetLinearVelocity().y));
            
            // Play idle animation if on ground
            if (_canJump && _jumpCount == 0)
            {
                this.StopAllActions();
                this.RunAction(new CCRepeatForever(new CCAnimate(_idleAnimation)));
            }
        }
        
        public void Jump()
        {
            if (_canJump && _jumpCount < MAX_JUMPS)
            {
                _body.SetLinearVelocity(new b2Vec2(_body.GetLinearVelocity().x, JUMP_FORCE));
                _jumpCount++;
                _canJump = (_jumpCount < MAX_JUMPS);
                
                // Play jump animation
                this.StopAllActions();
                this.RunAction(new CCAnimate(_jumpAnimation));
            }
        }
        
        public void SetCanJump(bool canJump)
        {
            if (canJump && !_canJump)
            {
                // Player just landed
                _jumpCount = 0;
                
                // Play idle or run animation based on horizontal velocity
                this.StopAllActions();
                
                if (Math.Abs(_body.GetLinearVelocity().x) > 0.1f)
                {
                    this.RunAction(new CCRepeatForever(new CCAnimate(_runAnimation)));
                }
                else
                {
                    this.RunAction(new CCRepeatForever(new CCAnimate(_idleAnimation)));
                }
            }
            
            _canJump = canJump;
        }
        
        // User data for foot sensor
        private class FootSensorUserData
        {
            public Player Player { get; private set; }
            
            public FootSensorUserData(Player player)
            {
                Player = player;
            }
        }
    }
}
```


## Creating Platform Objects

Create `Platform.cs`:

```csharp
using Cocos2D;
using Box2D.Dynamics;

namespace Platformer
{
    public class Platform : CCSprite
    {
        // Physics body
        private b2Body _body;
        
        public Platform(b2World world, float posX, float posY, float width, float height) : base("platform")
        {
            // Set position and scale to match desired dimensions
            this.Position = new CCPoint(posX, posY);
            this.ScaleX = width / this.ContentSize.Width;
            this.ScaleY = height / this.ContentSize.Height;
            
            // Create physics body
            _body = PhysicsHelper.CreateBoxBody(
                world, 
                posX, 
                posY, 
                width, 
                height, 
                false,  // Static body
                1.0f,   // Density
                0.3f,   // Friction
                0.0f    // No bounce
            );
            
            // Set collision filtering
            b2Fixture fixture = _body.FixtureList;
            b2Filter filter = fixture.Filter;
            filter.categoryBits = PhysicsHelper.CATEGORY_PLATFORM;
            filter.maskBits = PhysicsHelper.CATEGORY_PLAYER;
            fixture.SetFilterData(filter);
            
            // Store reference to this platform
            _body.UserData = this;
        }
    }
}
```

## Creating the Main Game Layer

Create `GameLayer.cs`:

```csharp
using System;
using System.Collections.Generic;
using Cocos2D;
using Box2D.Dynamics;
using Box2D.Common;

namespace Platformer
{
    public class GameLayer : CCLayer
    {
        // Box2D world
        private b2World _world;
        
        // Game objects
        private Player _player;
        private List<Platform> _platforms = new List<Platform>();
        
        // Input state
        private bool _isLeftPressed;
        private bool _isRightPressed;
        private bool _isJumpPressed;
        
        public GameLayer()
        {
            // Enable keyboard
            this.KeyboardEnabled = true;
            
            // Initialize physics world with gravity
            _world = new b2World(new b2Vec2(0, -10.0f));
            
            // Schedule physics updates
            Schedule(Update);
            
            // Create level
            CreateLevel();
        }
        
        private void CreateLevel()
        {
            // Get visible area size
            CCSize visibleSize = CCDirector.SharedDirector.WinSize;
            
            // Create background
            CCSprite background = new CCSprite("background");
            background.Position = new CCPoint(visibleSize.Width / 2, visibleSize.Height / 2);
            background.Scale = Math.Max(visibleSize.Width / background.ContentSize.Width,
                                       visibleSize.Height / background.ContentSize.Height);
            AddChild(background, -1);
            
            // Create floor platform
            Platform floor = new Platform(_world, visibleSize.Width / 2, 32, visibleSize.Width, 64);
            _platforms.Add(floor);
            AddChild(floor);
            
            // Create some platforms
            Platform platform1 = new Platform(_world, 200, 150, 200, 32);
            _platforms.Add(platform1);
            AddChild(platform1);
            
            Platform platform2 = new Platform(_world, 400, 250, 200, 32);
            _platforms.Add(platform2);
            AddChild(platform2);
            
            Platform platform3 = new Platform(_world, 600, 350, 200, 32);
            _platforms.Add(platform3);
            AddChild(platform3);
            
            // Create player
            _player = new Player(_world);
            _player.Position = new CCPoint(100, 300);
            AddChild(_player);
        }
        
        private void Update(float dt)
        {
            // Update physics world
            _world.Step(dt, 8, 3);
            
            // Update player movement based on input
            if (_isLeftPressed)
                _player.MoveLeft();
            else if (_isRightPressed)
                _player.MoveRight();
            else
                _player.StopMoving();
                
            if (_isJumpPressed)
                _player.Jump();
                
            // Update all game objects
            _player.Update(dt);
        }
        
        // Handle keyboard events
        public override void OnKeyPressed(Microsoft.Xna.Framework.Input.Keys key)
        {
            switch (key)
            {
                case Microsoft.Xna.Framework.Input.Keys.Left:
                    _isLeftPressed = true;
                    break;
                case Microsoft.Xna.Framework.Input.Keys.Right:
                    _isRightPressed = true;
                    break;
                case Microsoft.Xna.Framework.Input.Keys.Space:
                    _isJumpPressed = true;
                    break;
            }
        }
        
        public override void OnKeyReleased(Microsoft.Xna.Framework.Input.Keys key)
        {
            switch (key)
            {
                case Microsoft.Xna.Framework.Input.Keys.Left:
                    _isLeftPressed = false;
                    break;
                case Microsoft.Xna.Framework.Input.Keys.Right:
                    _isRightPressed = false;
                    break;
                case Microsoft.Xna.Framework.Input.Keys.Space:
                    _isJumpPressed = false;
                    break;
            }
        }
    }
}
```


## Updating the Main Game Class

Update `Game1.cs`:

```csharp
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Cocos2D;

namespace Platformer
{
    public class Game1 : Game
    {
        private CCDirector _director;
        
        public Game1()
        {
            CCApplication application = new CCApplication(this);
            _director = CCDirector.SharedDirector;
            
            application.ApplicationDelegate = new AppDelegate();
            application.StartGame();
        }
        
        // AppDelegate class
        internal class AppDelegate : CCApplicationDelegate
        {
            public override void ApplicationDidFinishLaunching()
            {
                CCDirector director = CCDirector.SharedDirector;
                
                // Set display settings
                CCDrawManager.SetDesignResolutionSize(800, 600, CCResolutionPolicy.ShowAll);
                
                // Create and run the game scene
                CCScene scene = new CCScene();
                scene.AddChild(new GameLayer());
                director.RunWithScene(scene);
            }
        }
    }
}
```

## Adding Collision Detection

To properly implement collision detection, we need to create a contact listener for Box2D:

```csharp
// Add this to GameLayer.cs

// Add this field to the class
private ContactListener _contactListener;

// Add this to the constructor
_contactListener = new ContactListener();
_world.SetContactListener(_contactListener);

// Add this class inside GameLayer.cs
private class ContactListener : b2ContactListener
{
    public override void BeginContact(b2Contact contact)
    {
        // Check for foot sensor contacts to enable jumping
        object userDataA = contact.GetFixtureA().GetUserData();
        object userDataB = contact.GetFixtureB().GetUserData();
        
        Player.FootSensorUserData footData = userDataA as Player.FootSensorUserData 
                                         ?? userDataB as Player.FootSensorUserData;
        
        if (footData != null)
        {
            footData.Player.SetCanJump(true);
        }
    }
    
    public override void EndContact(b2Contact contact)
    {
        // Check for foot sensor contacts to disable jumping
        object userDataA = contact.GetFixtureA().GetUserData();
        object userDataB = contact.GetFixtureB().GetUserData();
        
        Player.FootSensorUserData footData = userDataA as Player.FootSensorUserData 
                                         ?? userDataB as Player.FootSensorUserData;
        
        if (footData != null)
        {
            footData.Player.SetCanJump(false);
        }
    }
}
```

## Adding Game UI

Let's add a simple UI showing the player's score and a restart button:

```csharp
// Add these fields to GameLayer.cs
private int _score = 0;
private CCLabelTTF _scoreLabel;
private CCMenuItemLabel _restartButton;

// Add this to the CreateLevel method in GameLayer.cs
// Create score label
_scoreLabel = new CCLabelTTF($"Score: {_score}", "Arial", 24);
_scoreLabel.Position = new CCPoint(100, visibleSize.Height - 30);
_scoreLabel.Color = CCColor3B.White;
AddChild(_scoreLabel, 10);

// Create restart button
CCLabelTTF restartLabel = new CCLabelTTF("Restart", "Arial", 24);
_restartButton = new CCMenuItemLabel(restartLabel, RestartGame);
_restartButton.Position = new CCPoint(visibleSize.Width - 100, visibleSize.Height - 30);

CCMenu menu = new CCMenu(_restartButton);
menu.Position = CCPoint.Zero;
AddChild(menu, 10);

// Add this method to GameLayer.cs
private void RestartGame(object sender)
{
    // Reset score
    _score = 0;
    _scoreLabel.Text = $"Score: {_score}";
    
    // Reset player position
    _player.Position = new CCPoint(100, 300);
}
```

## Adding Collectibles

Let's add collectible items that increase the player's score:

```csharp
// Add this class to a new file called Collectible.cs
using Cocos2D;
using Box2D.Dynamics;
using Box2D.Common;
using Box2D.Collision.Shapes;

namespace Platformer
{
    public class Collectible : CCSprite
    {
        private b2Body _body;
        
        public Collectible(b2World world, float x, float y) : base("coin")
        {
            Position = new CCPoint(x, y);
            
            // Create physics body
            b2BodyDef bodyDef = new b2BodyDef();
            bodyDef.type = b2BodyType.b2_staticBody;
            bodyDef.position = new b2Vec2(x / PhysicsHelper.PTM_RATIO, y / PhysicsHelper.PTM_RATIO);
            
            _body = world.CreateBody(bodyDef);
            
            // Create circle shape for collectible
            b2CircleShape shape = new b2CircleShape();
            shape.Radius = ContentSize.Width * 0.3f / PhysicsHelper.PTM_RATIO;
            
            b2FixtureDef fixtureDef = new b2FixtureDef();
            fixtureDef.shape = shape;
            fixtureDef.isSensor = true; // Make it a sensor (no collision response)
            fixtureDef.filter.categoryBits = PhysicsHelper.CATEGORY_COLLECTIBLE;
            fixtureDef.filter.maskBits = PhysicsHelper.CATEGORY_PLAYER;
            
            _body.CreateFixture(fixtureDef).SetUserData(this);
            
            // Add a rotation action
            RunAction(new CCRepeatForever(new CCRotateBy(2.0f, 360)));
        }
        
        public void Collect(GameLayer gameLayer)
        {
            // Remove from physics world
            _body.GetWorld().DestroyBody(_body);
            _body = null;
            
            // Play collection animation
            RunAction(new CCSequence(
                new CCScaleTo(0.2f, 1.5f),
                new CCScaleTo(0.2f, 0.0f),
                new CCCallFunc(() => RemoveFromParent())
            ));
            
            // Increase score
            gameLayer.IncreaseScore(10);
        }
    }
}

// Add this method to GameLayer class
public void IncreaseScore(int points)
{
    _score += points;
    _scoreLabel.Text = $"Score: {_score}";
}

// In the CreateLevel method of GameLayer, add some collectibles
// Create collectibles
for (int i = 0; i < 5; i++)
{
    Collectible coin = new Collectible(_world, 200 + i * 100, 400);
    AddChild(coin);
}

// Update the ContactListener to handle collectible collisions
private class ContactListener : b2ContactListener
{
    public override void BeginContact(b2Contact contact)
    {
        // Existing code for foot sensors...
        
        // Check for collectible contacts
        CheckCollectibleContact(contact.GetFixtureA(), contact.GetFixtureB());
        CheckCollectibleContact(contact.GetFixtureB(), contact.GetFixtureA());
    }
    
    private void CheckCollectibleContact(b2Fixture fixtureA, b2Fixture fixtureB)
    {
        // Check if fixA is a collectible and fixB is the player
        Collectible collectible = fixtureA.GetBody().GetUserData() as Collectible;
        if (collectible != null && 
            fixtureB.GetFilterData().categoryBits == PhysicsHelper.CATEGORY_PLAYER)
        {
            // Get the game layer from the player's parent
            CCNode playerNode = fixtureB.GetBody().GetUserData() as CCNode;
            if (playerNode != null && playerNode.Parent is GameLayer gameLayer)
            {
                collectible.Collect(gameLayer);
            }
        }
    }
    
    // Existing EndContact method...
}
```

## Adding Sound Effects

Sound effects make your platformer more engaging:

```csharp
// Add these methods to the Player class

private void PlayJumpSound()
{
    CCSimpleAudioEngine.SharedEngine.PlayEffect("jump");
}

private void PlayLandSound()
{
    CCSimpleAudioEngine.SharedEngine.PlayEffect("land");
}

// Update the Jump method to play sound
public void Jump()
{
    if (_canJump && _jumpCount < MAX_JUMPS)
    {
        _body.SetLinearVelocity(new b2Vec2(_body.GetLinearVelocity().x, JUMP_FORCE));
        _jumpCount++;
        _canJump = (_jumpCount < MAX_JUMPS);
        
        // Play jump animation
        this.StopAllActions();
        this.RunAction(new CCAnimate(_jumpAnimation));
        
        // Play jump sound
        PlayJumpSound();
    }
}

// Update the SetCanJump method to play landing sound
public void SetCanJump(bool canJump)
{
    if (canJump && !_canJump)
    {
        // Player just landed
        _jumpCount = 0;
        
        // Play landing sound
        PlayLandSound();
        
        // Play idle or run animation based on horizontal velocity
        // Existing animation code...
    }
    
    _canJump = canJump;
}

// Add this to GameLayer's constructor
// Load sounds
CCSimpleAudioEngine.SharedEngine.PreloadEffect("jump");
CCSimpleAudioEngine.SharedEngine.PreloadEffect("land");
CCSimpleAudioEngine.SharedEngine.PreloadEffect("coin");
CCSimpleAudioEngine.SharedEngine.PreloadBackgroundMusic("game_music");
CCSimpleAudioEngine.SharedEngine.PlayBackgroundMusic("game_music", true);

// Update Collectible's Collect method
public void Collect(GameLayer gameLayer)
{
    // Existing code...
    
    // Play collection sound
    CCSimpleAudioEngine.SharedEngine.PlayEffect("coin");
    
    // Existing code...
}
```

## Building and Running the Game

1. Make sure all source files are saved
2. Build the project by pressing F6 or selecting Build > Build Solution
3. Run the game by pressing F5 or selecting Debug > Start Debugging

If everything is set up correctly, you should see your platformer running with:
- A player character controllable with arrow keys and spacebar
- Platforms to jump on
- Collectible coins to gather
- Physics-based movement and collisions
- Sound effects for jumping, landing, and collecting coins
- A score display and restart button

## Further Enhancements

Here are some ideas to enhance your platformer:
- Add enemies and combat mechanics
- Create multiple levels
- Add power-ups like high jump or speed boost
- Implement a level editor
- Add a game over condition and victory screen
- Create moving platforms
- Add particle effects for jumps and coin collection

## Conclusion

Congratulations! You've built a basic 2D platformer game using cocos2d-mono and Box2D physics. This tutorial covered:
- Setting up a cocos2d-mono project
- Implementing physics with Box2D
- Creating game objects like player and platforms
- Handling user input
- Implementing collision detection
- Adding game mechanics like jumping and collecting items
- Adding UI elements and sound effects

With this foundation, you can continue to build and expand your game with more features and polish. Happy game development!