using System;
using Cocos2D;

namespace tests
{
    public class BaseGradientTest : CCLayer
    {
        public virtual string title() { return "Gradient Test"; }
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
            CCScene s = new GradientTestScene();
            s.AddChild(GradientTestScene.restartTestAction());
            CCDirector.SharedDirector.ReplaceScene(s);
        }

        public void nextCallback(object pSender)
        {
            CCScene s = new GradientTestScene();
            s.AddChild(GradientTestScene.nextTestAction());
            CCDirector.SharedDirector.ReplaceScene(s);
        }

        public void backCallback(object pSender)
        {
            CCScene s = new GradientTestScene();
            s.AddChild(GradientTestScene.backTestAction());
            CCDirector.SharedDirector.ReplaceScene(s);
        }
    }

    /// <summary>
    /// Tests vertical 3-stop gradient (sky effect).
    /// </summary>
    public class MultiGradientVerticalTest : BaseGradientTest
    {
        public override string title() { return "Multi-Gradient Vertical"; }
        public override string subtitle() { return "3-stop sky gradient (dark blue -> light blue -> white)"; }

        public override bool Init()
        {
            base.Init();
            CCSize s = CCDirector.SharedDirector.WinSize;

            var gradient = new CCLayerMultiGradient(
                new CCColor4B[] {
                    new CCColor4B(10, 10, 40, 255),      // dark blue (bottom)
                    new CCColor4B(60, 120, 200, 255),     // mid blue
                    new CCColor4B(200, 220, 255, 255)     // light (top)
                },
                new float[] { 0f, 0.5f, 1f },
                s.Width, s.Height, true);

            AddChild(gradient, 0);
            return true;
        }
    }

    /// <summary>
    /// Tests horizontal gradient.
    /// </summary>
    public class MultiGradientHorizontalTest : BaseGradientTest
    {
        public override string title() { return "Multi-Gradient Horizontal"; }
        public override string subtitle() { return "4-stop horizontal (rainbow)"; }

        public override bool Init()
        {
            base.Init();
            CCSize s = CCDirector.SharedDirector.WinSize;

            var gradient = new CCLayerMultiGradient(
                new CCColor4B[] {
                    new CCColor4B(255, 0, 0, 255),
                    new CCColor4B(255, 255, 0, 255),
                    new CCColor4B(0, 255, 0, 255),
                    new CCColor4B(0, 0, 255, 255)
                },
                new float[] { 0f, 0.33f, 0.66f, 1f },
                s.Width, s.Height, false);

            AddChild(gradient, 0);
            return true;
        }
    }

    /// <summary>
    /// Tests dynamic gradient update via SetGradient and SetSize.
    /// Cycles through different color schemes.
    /// </summary>
    public class MultiGradientDynamicTest : BaseGradientTest
    {
        private CCLayerMultiGradient _gradient;
        private float _timer;
        private int _scheme;

        private CCColor4B[][] _schemes = new CCColor4B[][] {
            new CCColor4B[] { new CCColor4B(255, 100, 0, 255), new CCColor4B(200, 0, 100, 255), new CCColor4B(50, 0, 150, 255) },
            new CCColor4B[] { new CCColor4B(0, 50, 0, 255), new CCColor4B(0, 150, 50, 255), new CCColor4B(100, 200, 100, 255) },
            new CCColor4B[] { new CCColor4B(40, 40, 40, 255), new CCColor4B(100, 100, 100, 255), new CCColor4B(200, 200, 200, 255) },
        };

        public override string title() { return "Dynamic Gradient"; }
        public override string subtitle() { return "SetGradient cycles every 2s"; }

        public override bool Init()
        {
            base.Init();
            CCSize s = CCDirector.SharedDirector.WinSize;

            _gradient = new CCLayerMultiGradient(
                _schemes[0],
                new float[] { 0f, 0.5f, 1f },
                s.Width, s.Height, true);
            AddChild(_gradient, 0);

            _scheme = 0;
            Schedule(UpdateGradient);
            return true;
        }

        private void UpdateGradient(float dt)
        {
            _timer += dt;
            if (_timer > 2f)
            {
                _timer = 0f;
                _scheme = (_scheme + 1) % _schemes.Length;
                _gradient.SetGradient(_schemes[_scheme], new float[] { 0f, 0.5f, 1f });
            }
        }
    }

    /// <summary>
    /// Tests gradient with many stops for smooth sunrise effect.
    /// </summary>
    public class MultiGradientSunriseTest : BaseGradientTest
    {
        public override string title() { return "Sunrise Gradient"; }
        public override string subtitle() { return "6-stop vertical gradient"; }

        public override bool Init()
        {
            base.Init();
            CCSize s = CCDirector.SharedDirector.WinSize;

            var gradient = new CCLayerMultiGradient(
                new CCColor4B[] {
                    new CCColor4B(10, 5, 30, 255),        // deep night
                    new CCColor4B(30, 10, 60, 255),       // dark purple
                    new CCColor4B(180, 60, 30, 255),      // orange horizon
                    new CCColor4B(255, 150, 50, 255),     // golden
                    new CCColor4B(255, 200, 120, 255),    // warm yellow
                    new CCColor4B(150, 200, 255, 255)     // pale sky
                },
                new float[] { 0f, 0.15f, 0.35f, 0.5f, 0.7f, 1f },
                s.Width, s.Height, true);

            AddChild(gradient, 0);
            return true;
        }
    }
}
