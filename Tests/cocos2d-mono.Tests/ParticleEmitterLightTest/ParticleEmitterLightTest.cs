using System;
using Cocos2D;
using Microsoft.Xna.Framework;

namespace tests
{
    public class BaseParticleEmitterLightTest : CCLayer
    {
        public virtual string title() { return "Particle Emitter Light"; }
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
            CCScene s = new ParticleEmitterLightTestScene();
            s.AddChild(ParticleEmitterLightTestScene.restartTestAction());
            CCDirector.SharedDirector.ReplaceScene(s);
        }

        public void nextCallback(object pSender)
        {
            CCScene s = new ParticleEmitterLightTestScene();
            s.AddChild(ParticleEmitterLightTestScene.nextTestAction());
            CCDirector.SharedDirector.ReplaceScene(s);
        }

        public void backCallback(object pSender)
        {
            CCScene s = new ParticleEmitterLightTestScene();
            s.AddChild(ParticleEmitterLightTestScene.backTestAction());
            CCDirector.SharedDirector.ReplaceScene(s);
        }
    }

    /// <summary>
    /// Tests basic emission with default behavior (velocity + drag + alpha fade).
    /// Touch to emit particles at touch location.
    /// </summary>
    public class ParticleEmitterLightBasicTest : BaseParticleEmitterLightTest
    {
        private CCParticleEmitterLight _emitter;
        private CCLabelTTF _countLabel;

        public override string title() { return "Basic Emission"; }
        public override string subtitle() { return "Touch to emit. Default drag + fade."; }

        public override bool Init()
        {
            base.Init();

            CCSize s = CCDirector.SharedDirector.WinSize;

            _emitter = new CCParticleEmitterLight(512);
            AddChild(_emitter, 10);

            _countLabel = new CCLabelTTF("Particles: 0", "arial", 16);
            _countLabel.Position = new CCPoint(s.Width / 2, s.Height - 60);
            AddChild(_countLabel, 100);

            TouchEnabled = true;
            TouchMode = CCTouchMode.OneByOne;
            Schedule(UpdateParticles);
            return true;
        }

        public override bool TouchBegan(CCTouch touch)
        {
            var pos = touch.Location;
            _emitter.Emit(new CCPoint(pos.X, pos.Y), 30, 200f,
                MathHelper.TwoPi, 0f, CCColor4F.Yellow, 1.5f, 2f);
            return true;
        }

        private void UpdateParticles(float dt)
        {
            _emitter.UpdateParticles(dt);
            _countLabel.Text = $"Particles: {_emitter.ActiveParticleCount}";
        }
    }

    /// <summary>
    /// Tests emission with per-particle colors from an array.
    /// </summary>
    public class ParticleEmitterLightColorsTest : BaseParticleEmitterLightTest
    {
        private CCParticleEmitterLight _emitter;
        private CCLabelTTF _countLabel;
        private float _emitTimer;

        private CCColor4F[] _explosionColors = {
            CCColor4F.Red, CCColor4F.Orange, CCColor4F.Yellow,
            new CCColor4F(1f, 0.5f, 0f, 1f),
            new CCColor4F(1f, 0.3f, 0f, 1f),
        };

        public override string title() { return "Per-Particle Colors"; }
        public override string subtitle() { return "Auto-emitting with color array"; }

        public override bool Init()
        {
            base.Init();

            CCSize s = CCDirector.SharedDirector.WinSize;
            _emitter = new CCParticleEmitterLight(1024);
            AddChild(_emitter, 10);

            _countLabel = new CCLabelTTF("Particles: 0", "arial", 16);
            _countLabel.Position = new CCPoint(s.Width / 2, s.Height - 60);
            AddChild(_countLabel, 100);

            Schedule(UpdateParticles);
            return true;
        }

        private void UpdateParticles(float dt)
        {
            CCSize s = CCDirector.SharedDirector.WinSize;

            _emitTimer += dt;
            if (_emitTimer > 0.3f)
            {
                _emitTimer = 0f;
                float x = CCRandom.Float_0_1() * s.Width;
                float y = s.Height * 0.3f + CCRandom.Float_0_1() * s.Height * 0.4f;

                _emitter.Emit(new CCPoint(x, y), 40, 150f,
                    MathHelper.TwoPi, 0f, _explosionColors, 1.2f, 2.5f, 0.4f, 0.3f);
            }

            _emitter.UpdateParticles(dt);
            _countLabel.Text = $"Particles: {_emitter.ActiveParticleCount}";
        }
    }

    /// <summary>
    /// Tests custom particle update delegate.
    /// Particles follow a gravity-like trajectory.
    /// </summary>
    public class ParticleEmitterLightCustomTest : BaseParticleEmitterLightTest
    {
        private CCParticleEmitterLight _emitter;
        private CCLabelTTF _countLabel;

        public override string title() { return "Custom Update Delegate"; }
        public override string subtitle() { return "Touch to emit. Gravity + wind effect."; }

        public override bool Init()
        {
            base.Init();

            CCSize s = CCDirector.SharedDirector.WinSize;
            _emitter = new CCParticleEmitterLight(512);
            AddChild(_emitter, 10);

            // Custom update: gravity + wind
            _emitter.OnUpdateParticle = (ref CCParticleEmitterLight.Particle p, float dt) =>
            {
                p.Velocity.Y -= 200f * dt;  // gravity
                p.Velocity.X += 30f * dt;   // wind
                p.Position += p.Velocity * dt;
            };

            _countLabel = new CCLabelTTF("Particles: 0", "arial", 16);
            _countLabel.Position = new CCPoint(s.Width / 2, s.Height - 60);
            AddChild(_countLabel, 100);

            TouchEnabled = true;
            TouchMode = CCTouchMode.OneByOne;
            Schedule(UpdateParticles);
            return true;
        }

        public override bool TouchBegan(CCTouch touch)
        {
            var pos = touch.Location;
            _emitter.Emit(new CCPoint(pos.X, pos.Y), 25, 250f,
                MathHelper.Pi * 0.5f, MathHelper.PiOver2,
                new CCColor4F(0.3f, 0.6f, 1f, 1f), 2f, 2f);
            return true;
        }

        private void UpdateParticles(float dt)
        {
            _emitter.UpdateParticles(dt);
            _countLabel.Text = $"Particles: {_emitter.ActiveParticleCount}";
        }
    }

    /// <summary>
    /// Tests StopAll and IsActive.
    /// </summary>
    public class ParticleEmitterLightStopTest : BaseParticleEmitterLightTest
    {
        private CCParticleEmitterLight _emitter;
        private CCLabelTTF _statusLabel;

        public override string title() { return "StopAll / IsActive"; }
        public override string subtitle() { return "Touch left=emit, right=StopAll"; }

        public override bool Init()
        {
            base.Init();

            CCSize s = CCDirector.SharedDirector.WinSize;
            _emitter = new CCParticleEmitterLight(256);
            AddChild(_emitter, 10);

            _statusLabel = new CCLabelTTF("IsActive: false | Count: 0", "arial", 16);
            _statusLabel.Position = new CCPoint(s.Width / 2, s.Height - 60);
            AddChild(_statusLabel, 100);

            TouchEnabled = true;
            TouchMode = CCTouchMode.OneByOne;
            Schedule(UpdateParticles);
            return true;
        }

        public override bool TouchBegan(CCTouch touch)
        {
            CCSize s = CCDirector.SharedDirector.WinSize;
            if (touch.Location.X < s.Width / 2)
            {
                _emitter.Emit(new CCPoint(s.Width / 4, s.Height / 2), 50, 180f,
                    MathHelper.TwoPi, 0f, CCColor4F.Green, 2f, 3f);
            }
            else
            {
                _emitter.StopAll();
            }
            return true;
        }

        private void UpdateParticles(float dt)
        {
            _emitter.UpdateParticles(dt);
            _statusLabel.Text = $"IsActive: {_emitter.IsActive} | Count: {_emitter.ActiveParticleCount}";
        }
    }
}
