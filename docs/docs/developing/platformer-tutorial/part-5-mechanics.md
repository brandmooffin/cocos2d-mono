---
sidebar_position: 5
---

# Part 5: Game Mechanics

In this part, we'll add game mechanics that make our platformer engaging: collectible coins, a scoring system, and user interface elements.

## What We'll Accomplish

By the end of this part, you'll have:
- Collectible coin objects with physics
- A scoring system that tracks collected coins
- UI elements displaying score and other game information
- Win condition when all coins are collected
- Sound effects for collecting coins
- Level completion detection

## Prerequisites

- Completed [Part 4: Platforms and Collision](./part-4-platforms)
- Understanding of UI elements in cocos2d-mono
- Basic game state management concepts

## Step 1: Collectible Assets

You'll need these assets for collectibles:
- `coin.png` - Coin sprite
- `coin_collect.wav` - Collection sound effect (optional)

Add these to your Content folder. Find them in the [complete project reference](https://github.com/brandmooffin/cocos2d-mono-samples/tree/main/Tutorial%20Samples/Platformer/Content).

## Step 2: Create Collectible Class

Create a new file called `Collectible.cs`:

```csharp
using Cocos2D;
using Box2D.Dynamics;
using Box2D.Common;
using Box2D.Collision.Shapes;

namespace Platformer
{
    public class Collectible : CCNode
    {
        private CCSprite coinSprite;
        private b2Body physicsBody;
        private b2World world;
        private bool isCollected = false;
        
        public bool IsCollected => isCollected;
        public int Value { get; set; } = 10;
        
        // Animation properties
        private float rotationSpeed = 180.0f; // degrees per second
        private float bobAmount = 10.0f; // pixels
        private float bobSpeed = 2.0f; // cycles per second
        private float originalY;
        private float bobTimer = 0.0f;
        
        public Collectible(b2World physicsWorld)
        {
            world = physicsWorld;
            
            InitializeSprite();
            CreatePhysicsBody();
            SetupAnimation();
            ScheduleUpdate();
        }
        
        private void InitializeSprite()
        {
            coinSprite = new CCSprite("coin.png");
            AddChild(coinSprite);
            
            // Store original position for bobbing animation
            originalY = PositionY;
        }
        
        private void CreatePhysicsBody()
        {
            // Create sensor body (detects collisions but doesn't block movement)
            var bodyDef = new b2BodyDef();
            bodyDef.type = b2BodyType.b2_staticBody;
            bodyDef.position = PhysicsHelper.PixelsToMeters(PositionX, PositionY);
            
            physicsBody = world.CreateBody(bodyDef);
            
            // Create circular sensor shape
            var shape = new b2CircleShape();
            shape.Radius = PhysicsHelper.PixelsToMeters(coinSprite.ContentSize.Width / 2);
            
            // Create fixture as sensor
            var fixtureDef = new b2FixtureDef();
            fixtureDef.shape = shape;
            fixtureDef.isSensor = true; // This makes it a sensor (trigger)
            
            physicsBody.CreateFixture(fixtureDef);
            physicsBody.UserData = this;
        }
        
        private void SetupAnimation()
        {
            // Set up rotation animation
            var rotateAction = CCRotateBy.Create(1.0f, rotationSpeed);
            var rotateForever = CCRepeatForever.Create(rotateAction);
            coinSprite.RunAction(rotateForever);
        }
        
        public override void Update(float dt)
        {
            base.Update(dt);
            
            if (isCollected)
                return;
                
            // Update bobbing animation
            bobTimer += dt;
            float bobOffset = (float)Math.Sin(bobTimer * bobSpeed * Math.PI * 2) * bobAmount;
            
            // Update position with bobbing effect
            var newPos = PhysicsHelper.MetersToPixels(physicsBody.Position);
            newPos.Y = originalY + bobOffset;
            Position = newPos;
        }
        
        public void Collect()
        {
            if (isCollected)
                return;
                
            isCollected = true;
            
            // Play collection sound effect
            CCSimpleAudioEngine.SharedEngine.PlayEffect("coin_collect.wav");
            
            // Create collection animation
            var scaleUp = CCScaleTo.Create(0.1f, 1.5f);
            var fadeOut = CCFadeOut.Create(0.2f);
            var remove = CCCallFunc.Create(() => RemoveFromParent());
            
            var collectSequence = CCSequence.Create(scaleUp, fadeOut, remove);
            coinSprite.RunAction(collectSequence);
            
            // Remove physics body
            world.DestroyBody(physicsBody);
            physicsBody = null;
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing && physicsBody != null)
            {
                world.DestroyBody(physicsBody);
                physicsBody = null;
            }
            base.Dispose(disposing);
        }
    }
}
```

## Step 3: Game Manager for Score Tracking

Create a `GameManager.cs` class to handle game state:

```csharp
using System;
using System.Collections.Generic;

namespace Platformer
{
    public class GameManager
    {
        private static GameManager instance;
        public static GameManager Instance => instance ?? (instance = new GameManager());
        
        public int Score { get; private set; } = 0;
        public int CoinsCollected { get; private set; } = 0;
        public int TotalCoins { get; set; } = 0;
        
        public event Action<int> ScoreChanged;
        public event Action<int, int> CoinsChanged; // collected, total
        public event Action LevelCompleted;
        
        private GameManager() { }
        
        public void ResetGame()
        {
            Score = 0;
            CoinsCollected = 0;
            TotalCoins = 0;
        }
        
        public void AddScore(int points)
        {
            Score += points;
            ScoreChanged?.Invoke(Score);
        }
        
        public void CollectCoin(int coinValue)
        {
            CoinsCollected++;
            AddScore(coinValue);
            CoinsChanged?.Invoke(CoinsCollected, TotalCoins);
            
            // Check if all coins collected
            if (CoinsCollected >= TotalCoins)
            {
                LevelCompleted?.Invoke();
            }
        }
        
        public bool IsLevelComplete()
        {
            return CoinsCollected >= TotalCoins;
        }
    }
}
```

## Step 4: UI System

Create a `GameUI.cs` class for the user interface:

```csharp
using Cocos2D;

namespace Platformer
{
    public class GameUI : CCNode
    {
        private CCLabel scoreLabel;
        private CCLabel coinsLabel;
        private CCLabel levelCompleteLabel;
        
        public GameUI()
        {
            CreateUI();
            SetupEventHandlers();
        }
        
        private void CreateUI()
        {
            // Score label
            scoreLabel = new CCLabel("Score: 0", "Arial", 24);
            scoreLabel.Color = CCColor3B.White;
            scoreLabel.AnchorPoint = CCPoint.AnchorUpperLeft;
            scoreLabel.Position = new CCPoint(20, CCDirector.SharedDirector.WinSize.Height - 20);
            AddChild(scoreLabel);
            
            // Coins label
            coinsLabel = new CCLabel("Coins: 0/0", "Arial", 24);
            coinsLabel.Color = CCColor3B.Yellow;
            coinsLabel.AnchorPoint = CCPoint.AnchorUpperLeft;
            coinsLabel.Position = new CCPoint(20, CCDirector.SharedDirector.WinSize.Height - 60);
            AddChild(coinsLabel);
            
            // Level complete label (initially hidden)
            levelCompleteLabel = new CCLabel("Level Complete!\nPress R to Restart", "Arial", 36);
            levelCompleteLabel.Color = CCColor3B.Green;
            levelCompleteLabel.AnchorPoint = CCPoint.AnchorMiddle;
            levelCompleteLabel.Position = CCDirector.SharedDirector.WinSize.Center;
            levelCompleteLabel.Visible = false;
            AddChild(levelCompleteLabel);
        }
        
        private void SetupEventHandlers()
        {
            GameManager.Instance.ScoreChanged += OnScoreChanged;
            GameManager.Instance.CoinsChanged += OnCoinsChanged;
            GameManager.Instance.LevelCompleted += OnLevelCompleted;
        }
        
        private void OnScoreChanged(int newScore)
        {
            scoreLabel.Text = $"Score: {newScore}";
        }
        
        private void OnCoinsChanged(int collected, int total)
        {
            coinsLabel.Text = $"Coins: {collected}/{total}";
        }
        
        private void OnLevelCompleted()
        {
            levelCompleteLabel.Visible = true;
            
            // Add celebration animation
            var pulse = CCSequence.Create(
                CCScaleTo.Create(0.5f, 1.2f),
                CCScaleTo.Create(0.5f, 1.0f)
            );
            var pulseForever = CCRepeatForever.Create(pulse);
            levelCompleteLabel.RunAction(pulseForever);
        }
        
        public void HideLevelComplete()
        {
            levelCompleteLabel.Visible = false;
            levelCompleteLabel.StopAllActions();
            levelCompleteLabel.Scale = 1.0f;
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GameManager.Instance.ScoreChanged -= OnScoreChanged;
                GameManager.Instance.CoinsChanged -= OnCoinsChanged;
                GameManager.Instance.LevelCompleted -= OnLevelCompleted;
            }
            base.Dispose(disposing);
        }
    }
}
```

## Step 5: Update Contact Listener

Update your `ContactListener.cs` to handle coin collection:

```csharp
// Add this to the BeginContact method in ContactListener.cs
public override void BeginContact(b2Contact contact)
{
    var fixtureA = contact.FixtureA;
    var fixtureB = contact.FixtureB;
    
    var bodyA = fixtureA.Body;
    var bodyB = fixtureB.Body;
    
    var objectA = bodyA.UserData;
    var objectB = bodyB.UserData;
    
    // Handle player-platform collision (existing code)
    Player player = null;
    Platform platform = null;
    Collectible collectible = null;
    
    // Check for player-platform collision
    if (objectA is Player && objectB is Platform)
    {
        player = objectA as Player;
        platform = objectB as Platform;
        HandlePlayerPlatformCollision(player, platform, contact);
    }
    else if (objectA is Platform && objectB is Player)
    {
        player = objectB as Player;
        platform = objectA as Platform;
        HandlePlayerPlatformCollision(player, platform, contact);
    }
    
    // Check for player-collectible collision
    if (objectA is Player && objectB is Collectible)
    {
        player = objectA as Player;
        collectible = objectB as Collectible;
        HandlePlayerCollectibleCollision(player, collectible);
    }
    else if (objectA is Collectible && objectB is Player)
    {
        player = objectB as Player;
        collectible = objectA as Collectible;
        HandlePlayerCollectibleCollision(player, collectible);
    }
}

private void HandlePlayerCollectibleCollision(Player player, Collectible collectible)
{
    if (!collectible.IsCollected)
    {
        GameManager.Instance.CollectCoin(collectible.Value);
        collectible.Collect();
    }
}
```

## Step 6: Update GameLayer

Update your `GameLayer.cs` to include collectibles and UI:

```csharp
private List<Collectible> collectibles;
private GameUI gameUI;

protected override void AddedToScene()
{
    base.AddedToScene();
    
    // Initialize game manager
    GameManager.Instance.ResetGame();
    
    // Set up contact listener
    contactListener = new ContactListener();
    world.SetContactListener(contactListener);
    
    // Create level with platforms and collectibles
    CreateLevel();
    CreateCollectibles();
    
    // Create player
    player = new Player(world);
    player.Position = new CCPoint(100, 300);
    AddChild(player);
    
    // Create UI
    gameUI = new GameUI();
    AddChild(gameUI);
}

private void CreateCollectibles()
{
    collectibles = new List<Collectible>();
    
    // Place coins on platforms
    var coinPositions = new CCPoint[]
    {
        new CCPoint(200, 200), // On platform1
        new CCPoint(500, 250), // On platform2
        new CCPoint(300, 350), // On platform3
        new CCPoint(600, 400), // On platform4
        new CCPoint(150, 100), // On ground
        new CCPoint(650, 100)  // On ground
    };
    
    foreach (var position in coinPositions)
    {
        var coin = new Collectible(world);
        coin.Position = position;
        collectibles.Add(coin);
        AddChild(coin);
    }
    
    // Set total coins in game manager
    GameManager.Instance.TotalCoins = collectibles.Count;
}

// Add restart functionality
private void OnKeyPressed(CCKeys key, CCEvent e)
{
    // ... existing movement code ...
    
    if (key == CCKeys.R && GameManager.Instance.IsLevelComplete())
    {
        RestartLevel();
    }
}

private void RestartLevel()
{
    // Remove all existing objects
    foreach (var collectible in collectibles)
    {
        collectible.RemoveFromParent();
    }
    
    // Reset game state
    GameManager.Instance.ResetGame();
    gameUI.HideLevelComplete();
    
    // Recreate collectibles
    CreateCollectibles();
    
    // Reset player position
    player.Position = new CCPoint(100, 300);
    var playerBody = player.GetPhysicsBody(); // You'll need to add this getter to Player class
    playerBody.LinearVelocity = new b2Vec2(0, 0);
}
```

## 🎯 Checkpoint: Game Mechanics

At this point, you should have:
- ✅ Collectible coins placed throughout the level
- ✅ Working score system that updates when coins are collected
- ✅ UI showing current score and coin collection progress
- ✅ Level completion detection when all coins are collected
- ✅ Restart functionality with R key
- ✅ Sound effects for collecting coins (if audio files are included)

**Test your game**: Run the project and verify that:
1. Coins appear on platforms and animate (rotate and bob)
2. Walking into coins collects them and increases score
3. UI updates correctly showing score and coin count
4. "Level Complete" message appears when all coins are collected
5. Pressing R restarts the level after completion

## Troubleshooting

**Coins not collecting**: Check that contact listener is handling sensor collisions correctly.

**UI not updating**: Verify that event handlers are properly connected to GameManager events.

**Sound not playing**: Ensure audio files are added to Content folder with proper build action.

**Level not completing**: Check that TotalCoins count matches the actual number of collectibles created.

## Next Steps

In [Part 6: Polish and Effects](./part-6-polish), we'll add the final touches including particle effects, better animations, and enhanced game feel.

## Complete Code Reference

You can find the complete code for this part in the [tutorial samples repository](https://github.com/brandmooffin/cocos2d-mono-samples/tree/main/Tutorial%20Samples/Platformer).
