---
sidebar_position: 4
---

# Part 4: Platforms and Collision

In this part, we'll create platforms for our player to jump on and implement proper collision detection between the player and platform objects.

## What We'll Accomplish

By the end of this part, you'll have:
- Platform objects with physics bodies
- Collision detection between player and platforms
- Proper ground detection for jumping
- A basic level layout with multiple platforms
- Contact listener for physics interactions

## Prerequisites

- Completed [Part 3: Player Character](./part-3-player)
- Understanding of Box2D collision detection
- Basic knowledge of contact listeners

## Step 1: Platform Assets

For platforms, we'll need:
- `platform_grass.png` - Grass platform sprite
- `platform_dirt.png` - Dirt platform sprite (optional)

Add these to your Content folder. You can find these assets in the [complete project reference](https://github.com/brandmooffin/cocos2d-mono-samples/tree/main/Tutorial%20Samples/Platformer/Content/Images).

## Step 2: Create Platform Class

Create a new file called `Platform.cs`:

```csharp
using Cocos2D;
using Box2D.Dynamics;
using Box2D.Common;
using Box2D.Collision.Shapes;

namespace Platformer
{
    public class Platform : CCNode
    {
        private CCSprite platformSprite;
        private b2Body physicsBody;
        private b2World world;
        
        public float Width { get; private set; }
        public float Height { get; private set; }
        
        public Platform(b2World physicsWorld, string spriteFileName, float width, float height)
        {
            world = physicsWorld;
            Width = width;
            Height = height;
            
            InitializeSprite(spriteFileName);
            CreatePhysicsBody();
        }
        
        private void InitializeSprite(string spriteFileName)
        {
            platformSprite = new CCSprite(spriteFileName);
            
            // Scale the sprite to match our desired width/height
            platformSprite.ScaleX = Width / platformSprite.ContentSize.Width;
            platformSprite.ScaleY = Height / platformSprite.ContentSize.Height;
            
            AddChild(platformSprite);
        }
        
        private void CreatePhysicsBody()
        {
            // Create static physics body for platform
            var bodyDef = new b2BodyDef();
            bodyDef.type = b2BodyType.b2_staticBody;
            bodyDef.position = PhysicsHelper.PixelsToMeters(PositionX, PositionY);
            
            physicsBody = world.CreateBody(bodyDef);
            
            // Create platform shape
            var shape = new b2PolygonShape();
            var size = PhysicsHelper.PixelsToMeters(Width, Height);
            shape.SetAsBox(size.X / 2, size.Y / 2);
            
            // Create fixture
            var fixtureDef = new b2FixtureDef();
            fixtureDef.shape = shape;
            fixtureDef.friction = 0.7f; // Good grip for platforms
            fixtureDef.restitution = 0.0f; // No bouncing
            
            physicsBody.CreateFixture(fixtureDef);
            
            // Store reference in user data
            physicsBody.UserData = this;
        }
        
        public override void Update(float dt)
        {
            base.Update(dt);
            
            // Static platforms don't need position updates
            // but this is where you'd sync if the platform moved
        }
    }
}
```

## Step 3: Collision Detection System

Create a contact listener to handle collisions. Create `ContactListener.cs`:

```csharp
using Box2D.Dynamics;
using Box2D.Dynamics.Contacts;
using Box2D.Common;

namespace Platformer
{
    public class ContactListener : b2ContactListener
    {
        public override void BeginContact(b2Contact contact)
        {
            var fixtureA = contact.FixtureA;
            var fixtureB = contact.FixtureB;
            
            var bodyA = fixtureA.Body;
            var bodyB = fixtureB.Body;
            
            // Get the user data (our game objects)
            var objectA = bodyA.UserData;
            var objectB = bodyB.UserData;
            
            // Check if player is colliding with platform
            Player player = null;
            Platform platform = null;
            
            if (objectA is Player && objectB is Platform)
            {
                player = objectA as Player;
                platform = objectB as Platform;
            }
            else if (objectA is Platform && objectB is Player)
            {
                player = objectB as Player;
                platform = objectA as Platform;
            }
            
            if (player != null && platform != null)
            {
                HandlePlayerPlatformCollision(player, platform, contact);
            }
        }
        
        public override void EndContact(b2Contact contact)
        {
            var fixtureA = contact.FixtureA;
            var fixtureB = contact.FixtureB;
            
            var bodyA = fixtureA.Body;
            var bodyB = fixtureB.Body;
            
            var objectA = bodyA.UserData;
            var objectB = bodyB.UserData;
            
            // Check if player is leaving platform
            Player player = null;
            Platform platform = null;
            
            if (objectA is Player && objectB is Platform)
            {
                player = objectA as Player;
                platform = objectB as Platform;
            }
            else if (objectA is Platform && objectB is Player)
            {
                player = objectB as Player;
                platform = objectA as Platform;
            }
            
            if (player != null && platform != null)
            {
                HandlePlayerLeavePlatform(player, platform);
            }
        }
        
        private void HandlePlayerPlatformCollision(Player player, Platform platform, b2Contact contact)
        {
            // Get collision normal to determine if player landed on top
            b2WorldManifold worldManifold = new b2WorldManifold();
            contact.GetWorldManifold(out worldManifold);
            
            // Check if player is landing on top of platform
            // Normal points from A to B, so if player is A and normal.Y > 0, player is on top
            var playerIsA = contact.FixtureA.Body.UserData is Player;
            var normal = worldManifold.normal;
            
            if (playerIsA && normal.Y > 0.7f || !playerIsA && normal.Y < -0.7f)
            {
                // Player landed on top of platform
                player.SetOnGround(true);
            }
        }
        
        private void HandlePlayerLeavePlatform(Player player, Platform platform)
        {
            // For simplicity, we'll set this to false
            // In a more complex system, you'd check if player is still touching any platforms
            player.SetOnGround(false);
        }
    }
}
```

## Step 4: Level Creation

Update your `GameLayer.cs` to create a level with platforms:

```csharp
private List<Platform> platforms;
private ContactListener contactListener;

protected override void AddedToScene()
{
    base.AddedToScene();
    
    // Set up contact listener for collisions
    contactListener = new ContactListener();
    world.SetContactListener(contactListener);
    
    // Create platforms
    CreateLevel();
    
    // Create player
    player = new Player(world);
    player.Position = new CCPoint(100, 300); // Start above the ground
    AddChild(player);
}

private void CreateLevel()
{
    platforms = new List<Platform>();
    
    // Ground platform
    var ground = new Platform(world, "platform_grass.png", 800, 32);
    ground.Position = new CCPoint(400, 50);
    platforms.Add(ground);
    AddChild(ground);
    
    // Floating platforms
    var platform1 = new Platform(world, "platform_grass.png", 200, 32);
    platform1.Position = new CCPoint(200, 150);
    platforms.Add(platform1);
    AddChild(platform1);
    
    var platform2 = new Platform(world, "platform_grass.png", 200, 32);
    platform2.Position = new CCPoint(500, 200);
    platforms.Add(platform2);
    AddChild(platform2);
    
    var platform3 = new Platform(world, "platform_grass.png", 150, 32);
    platform3.Position = new CCPoint(300, 300);
    platforms.Add(platform3);
    AddChild(platform3);
    
    // Higher platform for advanced jumping
    var platform4 = new Platform(world, "platform_grass.png", 100, 32);
    platform4.Position = new CCPoint(600, 350);
    platforms.Add(platform4);
    AddChild(platform4);
}
```

## Step 5: Improved Ground Detection

Update the `Player.cs` class to better handle ground detection:

```csharp
// Add this property to Player class
private int groundContactCount = 0;

// Update the ground detection methods
public void SetOnGround(bool onGround)
{
    if (onGround)
    {
        groundContactCount++;
    }
    else
    {
        groundContactCount--;
        if (groundContactCount < 0)
            groundContactCount = 0;
    }
    
    isOnGround = groundContactCount > 0;
}

// Update the Jump method
public void Jump()
{
    if (isOnGround && Math.Abs(physicsBody.LinearVelocity.Y) < 0.1f)
    {
        var velocity = physicsBody.LinearVelocity;
        velocity.Y = JumpForce;
        physicsBody.LinearVelocity = velocity;
    }
}
```

## Step 6: Contact Filtering (Optional)

To improve collision detection accuracy, you can add contact filtering:

```csharp
// In ContactListener.cs, add this method
public override bool ShouldCollide(b2Fixture fixtureA, b2Fixture fixtureB)
{
    // Add any custom collision filtering logic here
    // For now, allow all collisions
    return true;
}

// You can also override PreSolve for more advanced collision handling
public override void PreSolve(b2Contact contact, b2Manifold oldManifold)
{
    // Handle collision response before physics resolution
    // Useful for one-way platforms or special collision behaviors
}
```

## 🎯 Checkpoint: Platforms and Collision

At this point, you should have:
- ✅ Multiple platforms that the player can stand on
- ✅ Proper collision detection between player and platforms
- ✅ Ground detection that allows jumping only when on platforms
- ✅ A basic level layout with different platform heights
- ✅ Smooth character movement and jumping

**Test your game**: Run the project and verify that:
1. The player can walk on all platforms
2. The player can jump from platform to platform
3. The player cannot jump while in mid-air
4. Collision detection works smoothly without jittering
5. The player properly lands on platform surfaces

## Troubleshooting

**Player falls through platforms**: Check that platform physics bodies are created correctly and collision detection is properly set up.

**Jumping doesn't work**: Verify that ground detection is working and the contact listener is properly registered.

**Jittery movement**: Adjust physics timestep or add damping to the player's physics body.

**Player sticks to walls**: Reduce friction between player and platforms, or adjust the collision normal detection logic.

## Next Steps

In [Part 5: Game Mechanics](./part-5-mechanics), we'll add collectible items, a scoring system, and UI elements to make our platformer more engaging.

## Complete Code Reference

You can find the complete code for this part in the [tutorial samples repository](https://github.com/brandmooffin/cocos2d-mono-samples/tree/main/Tutorial%20Samples/Platformer).
