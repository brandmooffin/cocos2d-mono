---
sidebar_position: 5
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
- `platform.png` - Simple platform sprite

Add these to your Content folder. You can find these assets in the [complete project reference](https://github.com/brandmooffin/cocos2d-mono-samples/tree/main/Tutorial%20Samples/Platformer/Final/Platformer/Content).

## Step 2: Create Platform Class

Create a new file called `Platform.cs`:

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

## Step 3: Collision Detection System

Create a contact listener to handle collisions. Create `ContactListener.cs`:

```csharp
using Box2D.Collision;
using Box2D.Dynamics;
using Box2D.Dynamics.Contacts;

namespace Platformer
{
    public class ContactListener : b2ContactListener
    {
        public override void BeginContact(b2Contact contact)
        {
            // Check for foot sensor contacts to enable jumping
            object userDataA = contact.GetFixtureA().UserData;
            object userDataB = contact.GetFixtureB().UserData;

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
            object userDataA = contact.GetFixtureA().UserData;
            object userDataB = contact.GetFixtureB().UserData;

            Player.FootSensorUserData footData = userDataA as Player.FootSensorUserData
                                             ?? userDataB as Player.FootSensorUserData;

            if (footData != null)
            {
                footData.Player.SetCanJump(false);
            }
        }

        public override void PostSolve(b2Contact contact, ref b2ContactImpulse impulse)
        {
            
        }

        public override void PreSolve(b2Contact contact, b2Manifold oldManifold)
        {
            
        }
    }
}
```

## Step 4: Level Creation

Update your `GameLayer.cs` to create a level with platforms:

```csharp
private List<Platform> _platforms = new List<Platform>();
private ContactListener contactListener;

protected override void AddedToScene()
{
    base.AddedToScene();

    // ...existing code
    
    // Set up contact listener for collisions
    contactListener = new ContactListener();
    _world.SetContactListener(contactListener);
    
    // Create platforms
    CreateLevel();

    // ...existing code
}

private void CreateLevel()
{
    // ...existing code

    // Create floor platform
    Platform floor = new Platform(_world, visibleSize.Width / 2, 32, visibleSize.Width, 64);
    _platforms.Add(floor);
    AddChild(floor);

    // Create some platforms
    Platform platform1 = new Platform(_world, 200, 100, 200, 32);
    _platforms.Add(platform1);
    AddChild(platform1);

    Platform platform2 = new Platform(_world, 600, 100, 200, 32);
    _platforms.Add(platform2);
    AddChild(platform2);
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

You can find the complete code for this part in the [tutorial samples repository](https://github.com/brandmooffin/cocos2d-mono-samples/tree/main/Tutorial%20Samples/Platformer/Checkpoints/Part%204).
