---
sidebar_position: 4
---

# Part 3: Player Character

In this part, we'll create our player character with movement controls, animations, and physics integration.

## What We'll Accomplish

By the end of this part, you'll have:
- A controllable player character
- Basic movement with arrow keys or WASD
- Sprite animations for idle, walking, and jumping
- Physics-based movement using Box2D
- Proper character collision detection

## Prerequisites

- Completed [Part 2: Physics Foundation](./part-2-physics)
- Understanding of sprite animations
- Basic input handling concepts

## Step 1: Player Character Assets

For this tutorial, we'll need the following sprite assets:
- `player_idle.png` - Idle animation frames
- `player_run_1.png`, `player_run_2.png`, `player_run_3.png`, `player_run_4.png` 
   - Running animation frames  
- `player_jump.png` - Jumping frame
- `jump.wav` - Jumping sound
- `land.wav` - Landing sound

You can find these assets in the [complete project reference](https://github.com/brandmooffin/cocos2d-mono-samples/tree/main/Tutorial%20Samples/Platformer/Final/Platformer/Content).

Add these files to your Content folder and ensure they're set to "Content" build action.

## Step 2: Create Player Class

Create a new file called `Player.cs`:

```csharp
using System;
using Cocos2D;
using Box2D.Dynamics;
using Box2D.Common;
using Box2D.Collision.Shapes;
using CocosDenshion;

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
        private bool _isRunning = false;

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
                ContentSize.Width * 0.4f / PhysicsHelper.PTM_RATIO,
                ContentSize.Height * 0.45f / PhysicsHelper.PTM_RATIO);

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
                ContentSize.Width * 0.3f / PhysicsHelper.PTM_RATIO,
                0.1f / PhysicsHelper.PTM_RATIO,
                new b2Vec2(0, -ContentSize.Height * 0.45f / PhysicsHelper.PTM_RATIO),
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
            _runAnimation.AddSpriteFrameWithFileName("player_run_1");
            _runAnimation.AddSpriteFrameWithFileName("player_run_2");
            _runAnimation.AddSpriteFrameWithFileName("player_run_3");
            _runAnimation.AddSpriteFrameWithFileName("player_run_4");
            _runAnimation.DelayPerUnit = 0.1f;

            _jumpAnimation = new CCAnimation();
            _jumpAnimation.AddSpriteFrameWithFileName("player_jump");
            _jumpAnimation.DelayPerUnit = 0.1f;
        }

        public void Update(float dt)
        {
            // Update sprite position based on physics body
            Position = PhysicsHelper.ToCocosVector(_body.Position);

            // Check if player fell off the screen
            if (Position.Y < -100)
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
            FlipX = true;

            // Play run animation if on ground
            if (_canJump && _jumpCount == 0 && !_isRunning)
            {
                _isRunning = true; // Set running state
                RunAction(new CCRepeatForever(new CCAnimate(_runAnimation)));
            }
        }

        public void MoveRight()
        {
            _body.LinearVelocity = new b2Vec2(MOVE_SPEED, _body.LinearVelocity.y);

            // Flip sprite to face right
            FlipX = false;

            // Play run animation if on ground
            if (_canJump && _jumpCount == 0 && !_isRunning)
            {
                _isRunning = true; // Set running state
                RunAction(new CCRepeatForever(new CCAnimate(_runAnimation)));
            }
        }

        public void StopMoving()
        {
            _body.LinearVelocity = new b2Vec2(0, _body.LinearVelocity.y);

            // Play idle animation if on ground
            if (_canJump && _jumpCount == 0)
            {
                _isRunning = false; // Stop running animation
                StopAllActions();
                RunAction(new CCRepeatForever(new CCAnimate(_idleAnimation)));
            }
        }

        public void Jump()
        {
            if (_canJump && _jumpCount < MAX_JUMPS)
            {
                _body.LinearVelocity = new b2Vec2(_body.LinearVelocity.x, JUMP_FORCE);
                _jumpCount++;
                _canJump = (_jumpCount < MAX_JUMPS);

                _isRunning = false; // Stop running animation when jumping
                // Play jump animation
                StopAllActions();
                RunAction(new CCAnimate(_jumpAnimation));

                // Play jump sound
                PlayJumpSound();
            }
        }

        public void SetCanJump(bool canJump)
        {
            if (canJump && !_canJump)
            {
                // Player just landed
                _jumpCount = 0;

                // Play landing sound
                PlayLandSound();

                // Play idle or run animation based on horizontal velocity
                StopAllActions();

                if (Math.Abs(_body.LinearVelocity.x) > 0.1f)
                {
                    RunAction(new CCRepeatForever(new CCAnimate(_runAnimation)));
                }
                else
                {
                    RunAction(new CCRepeatForever(new CCAnimate(_idleAnimation)));
                }
            }

            _canJump = canJump;
        }

        private void PlayJumpSound()
        {
            CCSimpleAudioEngine.SharedEngine.PlayEffect("jump");
        }

        private void PlayLandSound()
        {
            CCSimpleAudioEngine.SharedEngine.PlayEffect("land");
        }

        // User data for foot sensor
        public class FootSensorUserData
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

## Step 3: Integrating Player into Game

Update your `GameLayer.cs` to add the player, handle keyboard input & load sounds:

```csharp
using System;
using System.Collections.Generic;
using Cocos2D;
using Box2D.Dynamics;
using Box2D.Common;
using Box2D.Dynamics.Contacts;
using Box2D.Collision;
using Microsoft.Xna.Framework.Input;
using CocosDenshion;

namespace Platformer
{
    public class GameLayer : CCLayer
    {
        // Box2D world
        private b2World _world;

        // Game objects
        private Player _player;

        // Input state
        private bool _isLeftPressed;
        private bool _isRightPressed;
        private bool _isJumpPressed;
        
        private int _score = 0;
        private CCLabelTTF _scoreLabel;
        private CCMenuItemLabel _restartButton;

        public GameLayer()
        {
            // Initialize physics world with gravity
            _world = new b2World(new b2Vec2(0, -10.0f));

            // Create level
            CreateLevel();

            // Load sounds
            CCSimpleAudioEngine.SharedEngine.PreloadEffect("jump");
            CCSimpleAudioEngine.SharedEngine.PreloadEffect("land");

            ScheduleUpdate();
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

            // Create player
            _player = new Player(_world);
            _player.Position = new CCPoint(100, 300);
            AddChild(_player);

            // Create score label
            _scoreLabel = new CCLabelTTF($"Score: {_score}", "MarkerFelt", 22);
            _scoreLabel.Position = new CCPoint(100, visibleSize.Height - 30);
            _scoreLabel.Color = CCColor3B.Black;
            AddChild(_scoreLabel, 10);

            // Create restart button
            CCLabelTTF restartLabel = new CCLabelTTF("Restart", "MarkerFelt", 22);
            restartLabel.Color = CCColor3B.Black;
            _restartButton = new CCMenuItemLabel(restartLabel, RestartGame);
            _restartButton.Position = new CCPoint(visibleSize.Width - 100, visibleSize.Height - 30);

            CCMenu menu = new CCMenu(_restartButton);
            menu.Position = CCPoint.Zero;
            AddChild(menu, 10);
        }

        private void RestartGame(object sender)
        {
            // Reset score
            _score = 0;

            RemoveAllChildren();
            CreateLevel();
        }

        public void IncreaseScore(int points)
        {
            _score += points;
            _scoreLabel.Text = $"Score: {_score}";
        }

        public override void Update(float dt)
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

            base.Update(dt);

            // Handle keyboard state every frame            
            HandleInput();
        }

        public void HandleInput()
        {
            // Reset input state
            _isLeftPressed = false;
            _isRightPressed = false;
            _isJumpPressed = false;

            // Handle keyboard input
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Left))
                _isLeftPressed = true;
            if (state.IsKeyDown(Keys.Right))
                _isRightPressed = true;
            if (state.IsKeyDown(Keys.Space))
                _isJumpPressed = true;
        }
    }
}
```

## 🎯 Checkpoint: Controllable Character

At this point, you should have:
- ✅ A player character that responds to keyboard input
- ✅ Physics-based movement with Box2D
- ✅ Basic sprite animation system
- ✅ Character that can move left/right and has jump input

**Test your game**: Run the project and verify that:
1. The player character appears on screen
2. Arrow keys or WASD move the character left/right
3. Spacebar or Up arrow makes the character jump
4. The character sprite flips when changing direction

## Troubleshooting

**Character not moving**: Check that keyboard input events are properly set up and the physics body is created correctly.

**Character moving too fast/slow**: Adjust the `MovementSpeed` and `JumpForce` properties in the Player class.

**Animations not working**: Ensure sprite assets are properly added to the Content folder with "Content" build action.

## Next Steps

In [Part 4: Platforms and Collision](./part-4-platforms), we'll create platforms for the player to jump on and implement proper collision detection.

## Complete Code Reference

You can find the complete code for this part in the [tutorial samples repository](https://github.com/brandmooffin/cocos2d-mono-samples/tree/main/Tutorial%20Samples/Platformer/Checkpoints/Part%203).
