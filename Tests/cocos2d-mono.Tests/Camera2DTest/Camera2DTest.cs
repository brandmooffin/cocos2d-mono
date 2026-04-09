using System;
using Cocos2D;

namespace tests
{
    public class BaseCamera2DTest : CCLayer
    {
        public virtual string title() { return "Camera2D Test"; }
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
            CCScene s = new Camera2DTestScene();
            s.AddChild(Camera2DTestScene.restartTestAction());
            CCDirector.SharedDirector.ReplaceScene(s);
        }

        public void nextCallback(object pSender)
        {
            CCScene s = new Camera2DTestScene();
            s.AddChild(Camera2DTestScene.nextTestAction());
            CCDirector.SharedDirector.ReplaceScene(s);
        }

        public void backCallback(object pSender)
        {
            CCScene s = new Camera2DTestScene();
            s.AddChild(Camera2DTestScene.backTestAction());
            CCDirector.SharedDirector.ReplaceScene(s);
        }
    }

    /// <summary>
    /// Tests basic camera scrolling with auto-scroll.
    /// A grid of dots scrolls by as the camera pans automatically.
    /// </summary>
    public class Camera2DScrollTest : BaseCamera2DTest
    {
        private CCCamera2D _camera;
        private CCNode _gameLayer;

        public override string title() { return "CCCamera2D Scroll"; }
        public override string subtitle() { return "Auto-scroll left. Grid should move."; }

        public override bool Init()
        {
            base.Init();

            CCSize s = CCDirector.SharedDirector.WinSize;
            _camera = new CCCamera2D(s.Width, s.Height);
            _camera.AutoScrollSpeedX = 50f;

            _gameLayer = new CCNode();
            AddChild(_gameLayer, 0);

            // Draw a grid of dots to show scrolling
            CCDrawNode grid = new CCDrawNode();
            _gameLayer.AddChild(grid);

            for (int x = -200; x < 1200; x += 40)
            {
                for (int y = 50; y < (int)s.Height - 50; y += 40)
                {
                    var color = new CCColor4F(
                        (x + 200) / 1400f, 0.5f, (y - 50f) / (s.Height - 100f), 1f);
                    grid.DrawDot(new CCPoint(x, y), 3f, color);
                }
            }

            Schedule(UpdateCamera);
            return true;
        }

        private void UpdateCamera(float dt)
        {
            _camera.Update(dt);
            _camera.ApplyTo(_gameLayer);
        }
    }

    /// <summary>
    /// Tests camera follow with smoothing.
    /// A sprite bounces around and the camera follows it.
    /// </summary>
    public class Camera2DFollowTest : BaseCamera2DTest
    {
        private CCCamera2D _camera;
        private CCNode _gameLayer;
        private CCNode _target;
        private float _vx = 120f, _vy = 80f;

        public override string title() { return "CCCamera2D Follow"; }
        public override string subtitle() { return "Camera follows bouncing target with smoothing"; }

        public override bool Init()
        {
            base.Init();

            CCSize s = CCDirector.SharedDirector.WinSize;
            _camera = new CCCamera2D(s.Width, s.Height);

            _gameLayer = new CCNode();
            AddChild(_gameLayer, 0);

            // World background
            CCDrawNode bg = new CCDrawNode();
            _gameLayer.AddChild(bg);
            for (int x = -500; x < 1500; x += 60)
            {
                for (int y = -500; y < 1000; y += 60)
                {
                    bg.DrawDot(new CCPoint(x, y), 2f, new CCColor4F(0.3f, 0.3f, 0.3f, 1f));
                }
            }

            // Target
            CCDrawNode targetDraw = new CCDrawNode();
            targetDraw.DrawFilledCircle(CCPoint.Zero, 15f, CCColor4F.Yellow);
            _gameLayer.AddChild(targetDraw);
            _target = targetDraw;
            _target.Position = new CCPoint(s.Width / 2, s.Height / 2);

            _camera.Follow(_target, 0.08f);

            // Status label
            var info = new CCLabelTTF("Smoothing: 0.08", "arial", 16);
            info.Position = new CCPoint(s.Width / 2, s.Height - 60);
            AddChild(info, 100);

            Schedule(UpdateCamera);
            return true;
        }

        private void UpdateCamera(float dt)
        {
            CCSize s = CCDirector.SharedDirector.WinSize;

            // Bounce the target around
            var pos = _target.Position;
            pos.X += _vx * dt;
            pos.Y += _vy * dt;

            if (pos.X > s.Width + 200 || pos.X < -200) _vx = -_vx;
            if (pos.Y > s.Height + 200 || pos.Y < -200) _vy = -_vy;
            _target.Position = pos;

            _camera.Update(dt);
            _camera.ApplyTo(_gameLayer);
        }
    }

    /// <summary>
    /// Tests camera shake effect. Touch/click to trigger shake.
    /// </summary>
    public class Camera2DShakeTest : BaseCamera2DTest
    {
        private CCCamera2D _camera;
        private CCNode _gameLayer;
        private CCLabelTTF _statusLabel;

        public override string title() { return "CCCamera2D Shake"; }
        public override string subtitle() { return "Touch/Click to shake"; }

        public override bool Init()
        {
            base.Init();

            CCSize s = CCDirector.SharedDirector.WinSize;
            _camera = new CCCamera2D(s.Width, s.Height);

            _gameLayer = new CCNode();
            AddChild(_gameLayer, 0);

            // Static grid to see shake
            CCDrawNode grid = new CCDrawNode();
            _gameLayer.AddChild(grid);
            for (int x = 20; x < (int)s.Width; x += 30)
            {
                for (int y = 50; y < (int)s.Height - 50; y += 30)
                {
                    grid.DrawDot(new CCPoint(x, y), 2f, CCColor4F.White);
                }
            }

            _statusLabel = new CCLabelTTF("IsShaking: false", "arial", 18);
            _statusLabel.Position = new CCPoint(s.Width / 2, s.Height - 60);
            AddChild(_statusLabel, 100);

            TouchEnabled = true;
            TouchMode = CCTouchMode.OneByOne;
            Schedule(UpdateCamera);
            return true;
        }

        public override bool TouchBegan(CCTouch touch)
        {
            _camera.Shake(10f, 0.5f);
            return true;
        }

        private void UpdateCamera(float dt)
        {
            _camera.Update(dt);
            _camera.ApplyTo(_gameLayer);
            _statusLabel.Text = $"IsShaking: {_camera.IsShaking} | Offset: {_camera.ShakeOffset.X:F1}, {_camera.ShakeOffset.Y:F1}";
        }
    }

    /// <summary>
    /// Tests camera zoom. Touch left half to zoom in, right half to zoom out.
    /// </summary>
    public class Camera2DZoomTest : BaseCamera2DTest
    {
        private CCCamera2D _camera;
        private CCNode _gameLayer;
        private CCLabelTTF _zoomLabel;

        public override string title() { return "CCCamera2D Zoom"; }
        public override string subtitle() { return "Touch left=zoom in, right=zoom out"; }

        public override bool Init()
        {
            base.Init();

            CCSize s = CCDirector.SharedDirector.WinSize;
            _camera = new CCCamera2D(s.Width, s.Height);
            _camera.ScrollX = -s.Width * 0.25f;
            _camera.ScrollY = -s.Height * 0.25f;

            _gameLayer = new CCNode();
            AddChild(_gameLayer, 0);

            CCDrawNode content = new CCDrawNode();
            _gameLayer.AddChild(content);

            // Draw a colorful pattern
            for (int x = 0; x < (int)s.Width; x += 50)
            {
                for (int y = 50; y < (int)s.Height - 50; y += 50)
                {
                    content.DrawFilledCircle(new CCPoint(x, y), 8f,
                        new CCColor4F((float)x / s.Width, 0.5f, (float)y / s.Height, 1f));
                }
            }

            // Center crosshair
            content.DrawSegment(new CCPoint(s.Width / 2 - 20, s.Height / 2),
                new CCPoint(s.Width / 2 + 20, s.Height / 2), 1, CCColor4F.Red);
            content.DrawSegment(new CCPoint(s.Width / 2, s.Height / 2 - 20),
                new CCPoint(s.Width / 2, s.Height / 2 + 20), 1, CCColor4F.Red);

            _zoomLabel = new CCLabelTTF("Zoom: 1.0x", "arial", 18);
            _zoomLabel.Position = new CCPoint(s.Width / 2, s.Height - 60);
            AddChild(_zoomLabel, 100);

            TouchEnabled = true;
            TouchMode = CCTouchMode.OneByOne;
            Schedule(UpdateCamera);
            return true;
        }

        public override bool TouchBegan(CCTouch touch)
        {
            CCSize s = CCDirector.SharedDirector.WinSize;
            if (touch.Location.X < s.Width / 2)
                _camera.Zoom = Math.Min(_camera.Zoom + 0.1f, 4f);
            else
                _camera.Zoom = Math.Max(_camera.Zoom - 0.1f, 0.25f);
            return true;
        }

        private void UpdateCamera(float dt)
        {
            _camera.Update(dt);
            _camera.ApplyTo(_gameLayer);
            _zoomLabel.Text = $"Zoom: {_camera.Zoom:F1}x";
        }
    }

    /// <summary>
    /// Tests IsVisible viewport culling.
    /// Objects outside camera view should be marked not visible.
    /// </summary>
    public class Camera2DCullingTest : BaseCamera2DTest
    {
        private CCCamera2D _camera;
        private CCNode _gameLayer;
        private CCLabelTTF _statusLabel;

        public override string title() { return "CCCamera2D Culling"; }
        public override string subtitle() { return "Green=visible, Red=culled. Camera scrolls right."; }

        public override bool Init()
        {
            base.Init();

            CCSize s = CCDirector.SharedDirector.WinSize;
            _camera = new CCCamera2D(s.Width, s.Height);
            _camera.AutoScrollSpeedX = 30f;

            _gameLayer = new CCNode();
            AddChild(_gameLayer, 0);

            _statusLabel = new CCLabelTTF("Visible: 0 / Culled: 0", "arial", 16);
            _statusLabel.Position = new CCPoint(s.Width / 2, s.Height - 60);
            AddChild(_statusLabel, 100);

            Schedule(UpdateCamera);
            return true;
        }

        private void UpdateCamera(float dt)
        {
            _camera.Update(dt);
            _camera.ApplyTo(_gameLayer);

            // Recalculate which rects are visible
            _gameLayer.RemoveAllChildren();
            CCDrawNode draw = new CCDrawNode();
            _gameLayer.AddChild(draw);

            CCSize s = CCDirector.SharedDirector.WinSize;
            int visible = 0, culled = 0;

            for (int x = -200; x < 1200; x += 80)
            {
                for (int y = 80; y < (int)s.Height - 80; y += 80)
                {
                    var bounds = new CCRect(x - 15, y - 15, 30, 30);
                    bool isVis = _camera.IsVisible(bounds);

                    if (isVis)
                    {
                        draw.DrawRect(bounds, new CCColor4B(0, 255, 0, 180));
                        visible++;
                    }
                    else
                    {
                        draw.DrawRect(bounds, new CCColor4B(255, 0, 0, 80));
                        culled++;
                    }
                }
            }

            _statusLabel.Text = $"Visible: {visible} / Culled: {culled}";
        }
    }
}
