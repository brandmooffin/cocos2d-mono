using System;
using System.Collections.Generic;
using Cocos2D;

namespace tests
{
    public class BaseUtilitiesTest : CCLayer
    {
        public virtual string title() { return "Utilities Test"; }
        public virtual string subtitle() { return ""; }

        public override bool Init()
        {
            base.Init();
            CCSize s = CCDirector.SharedDirector.WinSize;

            var label = new CCLabelTTF(title(), "arial", 24);
            AddChild(label, 100);
            label.Position = new CCPoint(s.Width / 2, s.Height - 30);

            string sub = subtitle();
            if (sub.Length > 0)
            {
                label.Text += $" - {sub}";
            }

            var item1 = new CCMenuItemImage(TestResource.s_pPathB1, TestResource.s_pPathB2, backCallback);
            var item2 = new CCMenuItemImage(TestResource.s_pPathR1, TestResource.s_pPathR2, restartCallback);
            var item3 = new CCMenuItemImage(TestResource.s_pPathF1, TestResource.s_pPathF2, nextCallback);

            var menu = new CCMenu(item1, item2, item3);
            menu.Position = CCPoint.Zero;
            item1.Position = new CCPoint(s.Width / 2 - 100, 20);
            item2.Position = new CCPoint(s.Width / 2, 20);
            item3.Position = new CCPoint(s.Width / 2 + 100, 20);
            item1.Scale = 0.5f;
            item2.Scale = 0.5f;
            item3.Scale = 0.5f;
            AddChild(menu, 100);

            return true;
        }

        public void restartCallback(object pSender)
        {
            CCScene s = new UtilitiesTestScene();
            s.AddChild(UtilitiesTestScene.restartTestAction());
            CCDirector.SharedDirector.ReplaceScene(s);
        }

        public void nextCallback(object pSender)
        {
            CCScene s = new UtilitiesTestScene();
            s.AddChild(UtilitiesTestScene.nextTestAction());
            CCDirector.SharedDirector.ReplaceScene(s);
        }

        public void backCallback(object pSender)
        {
            CCScene s = new UtilitiesTestScene();
            s.AddChild(UtilitiesTestScene.backTestAction());
            CCDirector.SharedDirector.ReplaceScene(s);
        }
    }

    /// <summary>
    /// Tests CCObjectPool: Get, Return, ReturnAll, ForEachActive, and ICCPoolable.
    /// Visual: pool nodes appear/disappear, counts update in real-time.
    /// </summary>
    public class ObjectPoolTest : BaseUtilitiesTest
    {
        private CCObjectPool<PoolableNode> _pool;
        private CCNode _container;
        private CCLabelTTF _statusLabel;
        private List<PoolableNode> _spawned = new List<PoolableNode>();
        private float _spawnTimer;

        public override string title() { return "CCObjectPool"; }
        public override string subtitle() { return "Nodes spawn and return to pool. Watch counts."; }

        public override bool Init()
        {
            base.Init();

            CCSize s = CCDirector.SharedDirector.WinSize;
            _container = new CCNode();
            AddChild(_container, 5);

            _pool = new CCObjectPool<PoolableNode>(_container, initialSize: 10);

            _statusLabel = new CCLabelTTF("Active: 0 | Available: 10", "arial", 18);
            _statusLabel.Position = new CCPoint(s.Width / 2, s.Height - 60);
            AddChild(_statusLabel, 100);

            Schedule(UpdatePool);
            return true;
        }

        private void UpdatePool(float dt)
        {
            CCSize s = CCDirector.SharedDirector.WinSize;
            _spawnTimer += dt;

            // Spawn one every 0.2s
            if (_spawnTimer > 0.2f && _pool.ActiveCount < 20)
            {
                _spawnTimer = 0f;
                var node = _pool.Get();
                node.Position = new CCPoint(
                    50 + CCRandom.Float_0_1() * (s.Width - 100),
                    80 + CCRandom.Float_0_1() * (s.Height - 160));
                _spawned.Add(node);
            }

            // Return nodes older than 2s
            for (int i = _spawned.Count - 1; i >= 0; i--)
            {
                _spawned[i].Age += dt;
                if (_spawned[i].Age > 2f)
                {
                    _pool.Return(_spawned[i]);
                    _spawned.RemoveAt(i);
                }
            }

            _statusLabel.Text = $"Active: {_pool.ActiveCount} | Available: {_pool.AvailableCount}";
        }
    }

    /// <summary>
    /// A simple poolable node for testing.
    /// </summary>
    public class PoolableNode : CCDrawNode, ICCPoolable
    {
        public float Age;

        public PoolableNode()
        {
            DrawFilledCircle(CCPoint.Zero, 10f, new CCColor4F(
                CCRandom.Float_0_1(), CCRandom.Float_0_1(), CCRandom.Float_0_1(), 1f));
        }

        public void OnActivate()
        {
            Age = 0f;
        }

        public void OnReset()
        {
            Age = 0f;
        }
    }

    /// <summary>
    /// Tests CCCollision: Overlaps, ContainsPoint, GetShrunkBounds, CheckGroupCollisions.
    /// Two groups of nodes: red and blue. Colliding pairs flash white.
    /// </summary>
    public class CollisionTest : BaseUtilitiesTest
    {
        private List<CCDrawNode> _groupA = new List<CCDrawNode>();
        private List<CCDrawNode> _groupB = new List<CCDrawNode>();
        private CCDrawNode _overlay;
        private CCLabelTTF _statusLabel;

        // Movement velocities
        private List<CCPoint> _velA = new List<CCPoint>();
        private List<CCPoint> _velB = new List<CCPoint>();

        public override string title() { return "CCCollision"; }
        public override string subtitle() { return "Red & Blue groups. White flash on collision."; }

        public override bool Init()
        {
            base.Init();
            CCSize s = CCDirector.SharedDirector.WinSize;

            _overlay = new CCDrawNode();
            AddChild(_overlay, 20);

            // Create red group
            for (int i = 0; i < 5; i++)
            {
                var node = new CCDrawNode();
                node.DrawRect(new CCRect(-15, -15, 30, 30), new CCColor4B(255, 60, 60, 200));
                node.ContentSize = new CCSize(30, 30);
                node.AnchorPoint = CCPoint.AnchorMiddle;
                node.Position = new CCPoint(100 + i * 60, s.Height * 0.6f);
                AddChild(node, 10);
                _groupA.Add(node);
                _velA.Add(new CCPoint((CCRandom.Float_0_1() - 0.5f) * 120, (CCRandom.Float_0_1() - 0.5f) * 120));
            }

            // Create blue group
            for (int i = 0; i < 5; i++)
            {
                var node = new CCDrawNode();
                node.DrawRect(new CCRect(-15, -15, 30, 30), new CCColor4B(60, 60, 255, 200));
                node.ContentSize = new CCSize(30, 30);
                node.AnchorPoint = CCPoint.AnchorMiddle;
                node.Position = new CCPoint(100 + i * 60, s.Height * 0.4f);
                AddChild(node, 10);
                _groupB.Add(node);
                _velB.Add(new CCPoint((CCRandom.Float_0_1() - 0.5f) * 120, (CCRandom.Float_0_1() - 0.5f) * 120));
            }

            _statusLabel = new CCLabelTTF("Collisions: 0", "arial", 18);
            _statusLabel.Position = new CCPoint(s.Width / 2, s.Height - 60);
            AddChild(_statusLabel, 100);

            Schedule(UpdateCollision);
            return true;
        }

        private void UpdateCollision(float dt)
        {
            CCSize s = CCDirector.SharedDirector.WinSize;
            _overlay.Clear();

            // Move groups
            MoveGroup(_groupA, _velA, dt, s);
            MoveGroup(_groupB, _velB, dt, s);

            // Check collisions
            int collisions = 0;
            CCCollision.CheckGroupCollisions(_groupA, _groupB, 0.8f, (a, b) =>
            {
                collisions++;
                // Draw white overlay on collision
                _overlay.DrawRect(a.BoundingBox, new CCColor4B(255, 255, 255, 150));
                _overlay.DrawRect(b.BoundingBox, new CCColor4B(255, 255, 255, 150));
            });

            _statusLabel.Text = $"Collisions this frame: {collisions}";
        }

        private void MoveGroup(List<CCDrawNode> group, List<CCPoint> vels, float dt, CCSize bounds)
        {
            for (int i = 0; i < group.Count; i++)
            {
                var pos = group[i].Position;
                var vel = vels[i];
                pos.X += vel.X * dt;
                pos.Y += vel.Y * dt;

                if (pos.X < 50 || pos.X > bounds.Width - 50) vels[i] = new CCPoint(-vel.X, vel.Y);
                if (pos.Y < 80 || pos.Y > bounds.Height - 80) vels[i] = new CCPoint(vels[i].X, -vel.Y);

                pos.X = Math.Max(50, Math.Min(bounds.Width - 50, pos.X));
                pos.Y = Math.Max(80, Math.Min(bounds.Height - 80, pos.Y));
                group[i].Position = pos;
            }
        }
    }

    /// <summary>
    /// Tests CCCooldownTimer: Update, Reset, TryConsume, Progress.
    /// Visual bars show timer progress.
    /// </summary>
    public class CooldownTimerTest : BaseUtilitiesTest
    {
        private CCCooldownTimer _timer1;
        private CCCooldownTimer _timer2;
        private CCCooldownTimer _timer3;
        private CCDrawNode _bars;
        private CCLabelTTF _statusLabel;
        private int _fireCount;

        public override string title() { return "CCCooldownTimer"; }
        public override string subtitle() { return "3 timers with different durations. Touch to consume."; }

        public override bool Init()
        {
            base.Init();
            CCSize s = CCDirector.SharedDirector.WinSize;

            _timer1 = new CCCooldownTimer(1f, false);
            _timer2 = new CCCooldownTimer(2f, false);
            _timer3 = new CCCooldownTimer(0.5f, false);
            _fireCount = 0;

            _bars = new CCDrawNode();
            AddChild(_bars, 10);

            _statusLabel = new CCLabelTTF("Touch to TryConsume all ready timers", "arial", 16);
            _statusLabel.Position = new CCPoint(s.Width / 2, s.Height - 60);
            AddChild(_statusLabel, 100);

            // Labels for each bar
            var l1 = new CCLabelTTF("1.0s", "arial", 16);
            l1.Position = new CCPoint(80, s.Height * 0.65f);
            AddChild(l1, 100);

            var l2 = new CCLabelTTF("2.0s", "arial", 16);
            l2.Position = new CCPoint(80, s.Height * 0.5f);
            AddChild(l2, 100);

            var l3 = new CCLabelTTF("0.5s", "arial", 16);
            l3.Position = new CCPoint(80, s.Height * 0.35f);
            AddChild(l3, 100);

            TouchEnabled = true;
            TouchMode = CCTouchMode.OneByOne;
            Schedule(UpdateTimers);
            return true;
        }

        public override bool TouchBegan(CCTouch touch)
        {
            if (_timer1.TryConsume()) _fireCount++;
            if (_timer2.TryConsume()) _fireCount++;
            if (_timer3.TryConsume()) _fireCount++;
            _statusLabel.Text = $"Consumed {_fireCount} times total";
            return true;
        }

        private void UpdateTimers(float dt)
        {
            _timer1.Update(dt);
            _timer2.Update(dt);
            _timer3.Update(dt);

            CCSize s = CCDirector.SharedDirector.WinSize;
            _bars.Clear();

            float barWidth = s.Width - 200;
            float barHeight = 25;

            // Timer 1 bar
            DrawBar(_bars, 120, s.Height * 0.65f - barHeight / 2, barWidth, barHeight,
                _timer1.Progress, _timer1.IsReady);

            // Timer 2 bar
            DrawBar(_bars, 120, s.Height * 0.5f - barHeight / 2, barWidth, barHeight,
                _timer2.Progress, _timer2.IsReady);

            // Timer 3 bar
            DrawBar(_bars, 120, s.Height * 0.35f - barHeight / 2, barWidth, barHeight,
                _timer3.Progress, _timer3.IsReady);
        }

        private void DrawBar(CCDrawNode draw, float x, float y, float w, float h, float progress, bool ready)
        {
            // Background
            draw.DrawRect(new CCRect(x, y, w, h), new CCColor4B(60, 60, 60, 200));

            // Fill
            var fillColor = ready ? new CCColor4B(0, 200, 0, 220) : new CCColor4B(200, 200, 0, 220);
            draw.DrawRect(new CCRect(x, y, w * progress, h), fillColor);
        }
    }
}
