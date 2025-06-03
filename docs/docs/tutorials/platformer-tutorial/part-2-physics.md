---
sidebar_position: 3
---

# Part 2: Physics Foundation

In this part, we'll implement Box2D physics in our platformer game, creating the foundation for realistic movement and collision detection.

## What We'll Accomplish

By the end of this part, you'll have:
- A working Box2D physics world with gravity
- Understanding of physics coordinate systems
- Helper utilities for physics conversions
- A basic physics debug renderer
- Foundation for character and platform physics

## Prerequisites

- Completed [Part 1: Project Setup and Assets](./part-1-setup)
- Understanding of basic physics concepts (gravity, velocity, forces)

## Step 1: Understanding Box2D Integration

cocos2d-mono comes with Box2D physics built-in. Box2D uses a different coordinate system and units than cocos2d:

- **Box2D units**: Meters (optimal for objects 0.1 to 10 meters)
- **cocos2d units**: Pixels
- **Coordinate system**: Box2D uses bottom-left origin, cocos2d uses variable origin

We need conversion utilities to bridge these differences.

## Step 2: Create Physics Helper Class

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

## Step 3: Update GameLayer with Physics

Now let's update our `GameLayer.cs` to include Box2D physics:

```csharp
using Cocos2D;
using Box2D.Dynamics;
using System;

namespace Platformer
{
    public class GameLayer : CCLayer
    {
        // Physics world
        private b2World physicsWorld;
        private b2Body groundBody;
        
        // Physics settings
        private const int VELOCITY_ITERATIONS = 8;
        private const int POSITION_ITERATIONS = 3;
        private const float TIME_STEP = 1.0f / 60.0f;
        
        public GameLayer()
        {
            // Get visible area size
            CCSize visibleSize = CCDirector.SharedDirector.WinSize;
            
            // Initialize physics world
            InitializePhysics(visibleSize);
            
            // Create background
            CCSprite background = new CCSprite("background");
            background.Position = new CCPoint(visibleSize.Width / 2, visibleSize.Height / 2);
            
            // Scale background to fit screen
            float scaleX = visibleSize.Width / background.ContentSize.Width;
            float scaleY = visibleSize.Height / background.ContentSize.Height;
            background.Scale = Math.Max(scaleX, scaleY);
            
            AddChild(background, -1);
            
            // Create some test physics objects
            CreateTestObjects(visibleSize);
            
            // Add labels for information
            CCLabelTTF titleLabel = new CCLabelTTF("Platformer Tutorial - Part 2: Physics", "Arial", 24);
            titleLabel.Position = new CCPoint(visibleSize.Width / 2, visibleSize.Height - 30);
            titleLabel.Color = CCColor3B.White;
            AddChild(titleLabel);
            
            CCLabelTTF infoLabel = new CCLabelTTF("Physics World Active - Objects will fall!", "Arial", 16);
            infoLabel.Position = new CCPoint(visibleSize.Width / 2, visibleSize.Height - 60);
            infoLabel.Color = CCColor3B.Yellow;
            AddChild(infoLabel);
            
            // Enable updates to step the physics world
            ScheduleUpdate();
        }
        
        private void InitializePhysics(CCSize worldSize)
        {
            // Create physics world
            physicsWorld = PhysicsHelper.CreateWorld();
            
            // Create world boundaries
            groundBody = PhysicsHelper.CreateWorldBoundaries(physicsWorld, worldSize);
            
            // Set up contact listener for collision detection (we'll expand this later)
            // physicsWorld.SetContactListener(new ContactListener());
        }
        
        private void CreateTestObjects(CCSize visibleSize)
        {
            // Create some test boxes to demonstrate physics
            for (int i = 0; i < 3; i++)
            {
                // Create a visual sprite
                CCSprite testBox = new CCSprite("platform"); // Using platform texture as test
                testBox.Position = new CCPoint(200 + i * 100, 400 + i * 50);
                testBox.Color = new CCColor3B((byte)(100 + i * 50), (byte)(150 - i * 30), (byte)(200));
                AddChild(testBox, 1);
                
                // Create corresponding physics body
                b2Body physicsBody = PhysicsHelper.CreateDynamicBody(
                    physicsWorld, 
                    testBox.Position, 
                    testBox.ContentSize,
                    1.0f
                );
                
                // Store reference to sprite in physics body user data
                physicsBody.UserData = testBox;
            }
            
            // Create a static platform to catch falling objects
            CCSprite platform = new CCSprite("platform");
            platform.Position = new CCPoint(visibleSize.Width / 2, 150);
            platform.ScaleX = 4.0f; // Make it wider
            AddChild(platform, 1);
            
            // Create static physics body for platform
            b2Body platformBody = PhysicsHelper.CreateStaticBody(
                physicsWorld,
                platform.Position,
                new CCSize(platform.ContentSize.Width * platform.ScaleX, platform.ContentSize.Height)
            );
            platformBody.UserData = platform;
        }
        
        public override void Update(float dt)
        {
            base.Update(dt);
            
            // Step the physics world
            physicsWorld.Step(TIME_STEP, VELOCITY_ITERATIONS, POSITION_ITERATIONS);
            
            // Update visual positions based on physics bodies
            UpdateVisualPositions();
        }
        
        private void UpdateVisualPositions()
        {
            // Iterate through all physics bodies and update their corresponding sprites
            for (b2Body body = physicsWorld.BodyList; body != null; body = body.Next)
            {
                if (body.UserData is CCSprite sprite)
                {
                    // Convert physics position back to cocos2d coordinates
                    CCPoint newPosition = PhysicsHelper.VectorToPoint(body.Position);
                    sprite.Position = newPosition;
                    
                    // Update rotation if needed
                    sprite.Rotation = -CCMathHelper.ToDegrees(body.Angle);
                }
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Clean up physics world
                physicsWorld?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
```

## Step 4: Understanding Physics Concepts

### Key Physics Concepts:

1. **World**: The physics simulation container
2. **Bodies**: Objects that can move and collide
   - **Static**: Never moves (platforms, walls)
   - **Dynamic**: Affected by forces (player, enemies)
   - **Kinematic**: Moves but not affected by forces (moving platforms)

3. **Fixtures**: Attach shapes to bodies and define material properties
4. **Shapes**: Define collision geometry (box, circle, polygon)

### Important Settings:

- **Density**: Mass per unit area (affects how heavy objects feel)
- **Friction**: Resistance to sliding (0 = ice, 1 = rough surface)
- **Restitution**: Bounciness (0 = no bounce, 1 = perfectly elastic)

## Step 5: Build and Test

1. Build the project (F6)
2. Run the game (F5)

You should see:
- Your background
- Several colored boxes falling due to gravity
- Boxes landing on a platform and coming to rest
- Title showing "Part 2: Physics"

## Step 6: Adding Debug Rendering (Optional)

For development, it's helpful to see physics shapes. Add this to your GameLayer:

```csharp
// Add to the top of GameLayer.cs
using Box2D.Dynamics;

// Add these fields to GameLayer class
private bool showPhysicsDebug = true;

// Add this method to GameLayer
private void DrawPhysicsDebug()
{
    if (!showPhysicsDebug) return;
    
    // Simple debug drawing - draw rectangles for physics bodies
    for (b2Body body = physicsWorld.BodyList; body != null; body = body.Next)
    {
        if (body.UserData is CCSprite sprite)
        {
            // Draw a simple outline around physics bodies
            CCPoint position = PhysicsHelper.VectorToPoint(body.Position);
            
            // This is a simplified debug visualization
            // In a full implementation, you'd want proper debug drawing
        }
    }
}
```

## Checkpoint: What Your Project Should Look Like

At this point, your project should have:

### New Files:
- `PhysicsHelper.cs` - Physics utility functions
- Updated `GameLayer.cs` - Physics integration

### What You Should See:
- Boxes falling from the top of the screen
- Objects landing on a platform and stopping
- Realistic physics behavior with gravity
- Smooth 60 FPS with physics simulation

### Key Concepts Learned:
- Box2D coordinate system and unit conversion
- Creating physics worlds and bodies
- Static vs dynamic physics bodies
- Physics simulation loop
- Synchronizing visual sprites with physics bodies

## Troubleshooting

Common issues and solutions:

1. **Objects falling through platforms**: Check that static bodies are created correctly
2. **Objects moving too fast**: Adjust TIME_STEP or increase iteration counts
3. **Jittery movement**: Ensure consistent frame rate and proper conversion ratios
4. **Memory issues**: Make sure to dispose of physics world properly

## Understanding Performance

Physics simulation can be expensive. Key performance tips:

- Use appropriate iteration counts (8 velocity, 3 position is usually good)
- Don't create too many dynamic bodies
- Use static bodies for non-moving objects
- Consider sleeping inactive bodies

## Next Steps

Great work! You now have a solid physics foundation. In [Part 3: Player Character](./part-3-player), we'll create a controllable player character that uses physics for movement and can interact with the world.

Key topics in Part 3:
- Creating a player character class
- Implementing keyboard/gamepad input
- Physics-based movement and jumping
- Character animations
- Basic state management

## Download Checkpoint Project

You can download the complete Part 2 project [here](https://github.com/brandmooffin/cocos2d-mono-samples/tree/main/Tutorial%20Samples/Platformer/Checkpoints/Part%202) to compare with your implementation.
