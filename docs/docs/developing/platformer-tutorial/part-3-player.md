---
sidebar_position: 3
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
- `player_walk.png` - Walking animation frames  
- `player_jump.png` - Jumping frame

You can find these assets in the [complete project reference](https://github.com/brandmooffin/cocos2d-mono-samples/tree/main/Tutorial%20Samples/Platformer/Content/Images).

Add these files to your Content folder and ensure they're set to "Content" build action.

## Step 2: Create Player Class

Create a new file called `Player.cs`:

```csharp
using Cocos2D;
using Box2D.Dynamics;
using Box2D.Common;
using Box2D.Collision.Shapes;

namespace Platformer
{
    public class Player : CCNode
    {
        private CCSprite playerSprite;
        private b2Body physicsBody;
        private b2World world;
        
        // Animation actions
        private CCRepeatForever idleAnimation;
        private CCRepeatForever walkAnimation;
        
        // Movement properties
        public float MovementSpeed { get; set; } = 5.0f;
        public float JumpForce { get; set; } = 12.0f;
        
        // State tracking
        private bool isOnGround = false;
        private bool isMovingLeft = false;
        private bool isMovingRight = false;
        
        public Player(b2World physicsWorld)
        {
            world = physicsWorld;
            InitializeSprite();
            CreatePhysicsBody();
            SetupAnimations();
            ScheduleUpdate();
        }
        
        private void InitializeSprite()
        {
            playerSprite = new CCSprite("player_idle.png");
            AddChild(playerSprite);
        }
        
        private void CreatePhysicsBody()
        {
            // Create physics body definition
            var bodyDef = new b2BodyDef();
            bodyDef.type = b2BodyType.b2_dynamicBody;
            bodyDef.position = PhysicsHelper.PixelsToMeters(PositionX, PositionY);
            bodyDef.fixedRotation = true; // Prevent character from rotating
            
            // Create the physics body
            physicsBody = world.CreateBody(bodyDef);
            
            // Create character shape (rectangle)
            var shape = new b2PolygonShape();
            var size = PhysicsHelper.PixelsToMeters(
                playerSprite.ContentSize.Width * 0.8f, 
                playerSprite.ContentSize.Height);
            shape.SetAsBox(size.X / 2, size.Y / 2);
            
            // Create fixture definition
            var fixtureDef = new b2FixtureDef();
            fixtureDef.shape = shape;
            fixtureDef.density = 1.0f;
            fixtureDef.friction = 0.3f;
            fixtureDef.restitution = 0.0f; // No bouncing
            
            // Create the fixture
            physicsBody.CreateFixture(fixtureDef);
            
            // Store reference to this player in user data
            physicsBody.UserData = this;
        }
        
        private void SetupAnimations()
        {
            // TODO: Set up sprite frame animations
            // For now, we'll use simple sprite swapping
        }
        
        public override void Update(float dt)
        {
            base.Update(dt);
            
            // Sync sprite position with physics body
            var position = PhysicsHelper.MetersToPixels(physicsBody.Position);
            Position = position;
            
            // Apply movement based on input
            ApplyMovement();
            
            // Update animations based on state
            UpdateAnimations();
        }
        
        private void ApplyMovement()
        {
            var velocity = physicsBody.LinearVelocity;
            
            // Horizontal movement
            if (isMovingLeft)
            {
                velocity.X = -MovementSpeed;
                playerSprite.FlippedX = true;
            }
            else if (isMovingRight)
            {
                velocity.X = MovementSpeed;
                playerSprite.FlippedX = false;
            }
            else
            {
                // Apply friction when not moving
                velocity.X *= 0.8f;
            }
            
            physicsBody.LinearVelocity = velocity;
        }
        
        private void UpdateAnimations()
        {
            // Update animation based on movement state
            bool isMoving = Math.Abs(physicsBody.LinearVelocity.X) > 0.1f;
            
            if (!isOnGround)
            {
                // Jumping animation
                playerSprite.Texture = CCTextureCache.SharedTextureCache.AddImage("player_jump.png");
            }
            else if (isMoving)
            {
                // Walking animation
                playerSprite.Texture = CCTextureCache.SharedTextureCache.AddImage("player_walk.png");
            }
            else
            {
                // Idle animation
                playerSprite.Texture = CCTextureCache.SharedTextureCache.AddImage("player_idle.png");
            }
        }
        
        // Input handling methods
        public void MoveLeft()
        {
            isMovingLeft = true;
        }
        
        public void MoveRight()
        {
            isMovingRight = true;
        }
        
        public void StopMoving()
        {
            isMovingLeft = false;
            isMovingRight = false;
        }
        
        public void Jump()
        {
            if (isOnGround)
            {
                var velocity = physicsBody.LinearVelocity;
                velocity.Y = JumpForce;
                physicsBody.LinearVelocity = velocity;
                isOnGround = false;
            }
        }
        
        // Collision detection methods
        public void SetOnGround(bool onGround)
        {
            isOnGround = onGround;
        }
    }
}
```

## Step 3: Input Handling

Update your `GameLayer.cs` to handle keyboard input:

```csharp
public override void OnEnter()
{
    base.OnEnter();
    
    // Enable keyboard input
    var keyboardListener = new CCEventListenerKeyboard();
    keyboardListener.OnKeyPressed = OnKeyPressed;
    keyboardListener.OnKeyReleased = OnKeyReleased;
    
    EventDispatcher.AddEventListener(keyboardListener, this);
}

private void OnKeyPressed(CCKeys key, CCEvent e)
{
    switch (key)
    {
        case CCKeys.Left:
        case CCKeys.A:
            player.MoveLeft();
            break;
        case CCKeys.Right:
        case CCKeys.D:
            player.MoveRight();
            break;
        case CCKeys.Space:
        case CCKeys.Up:
        case CCKeys.W:
            player.Jump();
            break;
    }
}

private void OnKeyReleased(CCKeys key, CCEvent e)
{
    switch (key)
    {
        case CCKeys.Left:
        case CCKeys.A:
        case CCKeys.Right:
        case CCKeys.D:
            player.StopMoving();
            break;
    }
}
```

## Step 4: Integrating Player into Game

Add the player to your `GameLayer.cs`:

```csharp
private Player player;

protected override void AddedToScene()
{
    base.AddedToScene();
    
    // Create player at starting position
    player = new Player(world);
    player.Position = new CCPoint(100, 200);
    AddChild(player);
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

You can find the complete code for this part in the [tutorial samples repository](https://github.com/brandmooffin/cocos2d-mono-samples/tree/main/Tutorial%20Samples/Platformer).
