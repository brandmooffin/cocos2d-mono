using System;
using Cocos2D;
using Microsoft.Xna.Framework;

namespace tests
{
    /// <summary>
    /// Tests DrawFilledCircle with various sizes, colors, and segment counts.
    /// Verifies that circles render correctly as triangle fans without compositing artifacts.
    /// </summary>
    public class DrawNodeFilledCircleTest : BaseDrawNodeTest
    {
        public override string title() { return "DrawFilledCircle"; }
        public override string subtitle() { return "Various sizes, colors, segment counts"; }

        public override bool Init()
        {
            base.Init();

            CCSize s = CCDirector.SharedDirector.WinSize;
            CCDrawNode draw = new CCDrawNode();
            AddChild(draw, 10);

            // Small circle (default segments)
            draw.DrawFilledCircle(new CCPoint(s.Width * 0.2f, s.Height * 0.5f), 20f,
                new CCColor4F(1f, 0f, 0f, 1f));

            // Medium circle with explicit segment count
            draw.DrawFilledCircle(new CCPoint(s.Width * 0.4f, s.Height * 0.5f), 50f,
                new CCColor4F(0f, 1f, 0f, 0.7f), 64);

            // Large circle with low segments (should appear as polygon)
            draw.DrawFilledCircle(new CCPoint(s.Width * 0.65f, s.Height * 0.5f), 80f,
                new CCColor4F(0f, 0f, 1f, 0.5f), 8);

            // Overlapping semi-transparent circles to verify alpha compositing
            draw.DrawFilledCircle(new CCPoint(s.Width * 0.85f, s.Height * 0.55f), 40f,
                new CCColor4F(1f, 1f, 0f, 0.5f));
            draw.DrawFilledCircle(new CCPoint(s.Width * 0.88f, s.Height * 0.45f), 40f,
                new CCColor4F(0f, 1f, 1f, 0.5f));

            // Tiny circle (should still render, not a square)
            draw.DrawFilledCircle(new CCPoint(s.Width * 0.1f, s.Height * 0.3f), 3f,
                new CCColor4F(1f, 1f, 1f, 1f));

            // Labels
            var label1 = new CCLabelTTF("r=20", "arial", 14);
            label1.Position = new CCPoint(s.Width * 0.2f, s.Height * 0.35f);
            AddChild(label1);

            var label2 = new CCLabelTTF("r=50, 64seg", "arial", 14);
            label2.Position = new CCPoint(s.Width * 0.4f, s.Height * 0.35f);
            AddChild(label2);

            var label3 = new CCLabelTTF("r=80, 8seg", "arial", 14);
            label3.Position = new CCPoint(s.Width * 0.65f, s.Height * 0.35f);
            AddChild(label3);

            var label4 = new CCLabelTTF("alpha blend", "arial", 14);
            label4.Position = new CCPoint(s.Width * 0.86f, s.Height * 0.35f);
            AddChild(label4);

            return true;
        }
    }

    /// <summary>
    /// Tests DrawTriangle with various colors and configurations.
    /// </summary>
    public class DrawNodeTriangleTest : BaseDrawNodeTest
    {
        public override string title() { return "DrawTriangle"; }
        public override string subtitle() { return "Colored triangles with alpha"; }

        public override bool Init()
        {
            base.Init();

            CCSize s = CCDirector.SharedDirector.WinSize;
            CCDrawNode draw = new CCDrawNode();
            AddChild(draw, 10);

            // Solid red triangle
            draw.DrawTriangle(
                new CCPoint(s.Width * 0.1f, s.Height * 0.3f),
                new CCPoint(s.Width * 0.25f, s.Height * 0.3f),
                new CCPoint(s.Width * 0.175f, s.Height * 0.7f),
                new CCColor4F(1f, 0f, 0f, 1f));

            // Semi-transparent green triangle
            draw.DrawTriangle(
                new CCPoint(s.Width * 0.3f, s.Height * 0.3f),
                new CCPoint(s.Width * 0.5f, s.Height * 0.3f),
                new CCPoint(s.Width * 0.4f, s.Height * 0.7f),
                new CCColor4F(0f, 1f, 0f, 0.5f));

            // Overlapping triangles to test compositing
            draw.DrawTriangle(
                new CCPoint(s.Width * 0.55f, s.Height * 0.3f),
                new CCPoint(s.Width * 0.75f, s.Height * 0.3f),
                new CCPoint(s.Width * 0.65f, s.Height * 0.65f),
                new CCColor4F(0f, 0f, 1f, 0.6f));

            draw.DrawTriangle(
                new CCPoint(s.Width * 0.6f, s.Height * 0.4f),
                new CCPoint(s.Width * 0.8f, s.Height * 0.4f),
                new CCPoint(s.Width * 0.7f, s.Height * 0.75f),
                new CCColor4F(1f, 1f, 0f, 0.6f));

            // Tiny triangle
            draw.DrawTriangle(
                new CCPoint(s.Width * 0.9f, s.Height * 0.5f - 3f),
                new CCPoint(s.Width * 0.9f + 6f, s.Height * 0.5f - 3f),
                new CCPoint(s.Width * 0.9f + 3f, s.Height * 0.5f + 3f),
                new CCColor4F(1f, 1f, 1f, 1f));

            return true;
        }
    }

    /// <summary>
    /// Tests DrawRectOutline and DrawCircleOutline.
    /// </summary>
    public class DrawNodeOutlinesTest : BaseDrawNodeTest
    {
        public override string title() { return "DrawRectOutline / DrawCircleOutline"; }
        public override string subtitle() { return "Outlined shapes with varying line widths"; }

        public override bool Init()
        {
            base.Init();

            CCSize s = CCDirector.SharedDirector.WinSize;
            CCDrawNode draw = new CCDrawNode();
            AddChild(draw, 10);

            // Rect outlines with different line widths
            draw.DrawRectOutline(new CCRect(s.Width * 0.05f, s.Height * 0.35f, 100, 80), 1f,
                new CCColor4F(1f, 0f, 0f, 1f));
            draw.DrawRectOutline(new CCRect(s.Width * 0.25f, s.Height * 0.35f, 100, 80), 3f,
                new CCColor4F(0f, 1f, 0f, 1f));
            draw.DrawRectOutline(new CCRect(s.Width * 0.45f, s.Height * 0.35f, 100, 80), 6f,
                new CCColor4F(0f, 0f, 1f, 0.7f));

            // Circle outlines with different line widths
            draw.DrawCircleOutline(new CCPoint(s.Width * 0.7f, s.Height * 0.5f), 40f, 1f,
                new CCColor4B(255, 255, 0, 255));
            draw.DrawCircleOutline(new CCPoint(s.Width * 0.85f, s.Height * 0.5f), 40f, 4f,
                new CCColor4B(0, 255, 255, 255));

            // Labels
            var l1 = new CCLabelTTF("width=1", "arial", 14);
            l1.Position = new CCPoint(s.Width * 0.1f, s.Height * 0.28f);
            AddChild(l1);

            var l2 = new CCLabelTTF("width=3", "arial", 14);
            l2.Position = new CCPoint(s.Width * 0.3f, s.Height * 0.28f);
            AddChild(l2);

            var l3 = new CCLabelTTF("width=6", "arial", 14);
            l3.Position = new CCPoint(s.Width * 0.5f, s.Height * 0.28f);
            AddChild(l3);

            return true;
        }
    }

    /// <summary>
    /// Tests DrawDot at large radii (should use DrawFilledCircle internally, not squares).
    /// Also tests DrawCircle (filled) to verify compositing fix.
    /// </summary>
    public class DrawNodeDotCircleTest : BaseDrawNodeTest
    {
        public override string title() { return "DrawDot / DrawCircle (Fixed)"; }
        public override string subtitle() { return "Large dots should be round, not square"; }

        public override bool Init()
        {
            base.Init();

            CCSize s = CCDirector.SharedDirector.WinSize;
            CCDrawNode draw = new CCDrawNode();
            AddChild(draw, 10);

            // DrawDot at various sizes - all should be round
            float[] radii = { 2f, 4f, 8f, 16f, 32f, 50f };
            float x = s.Width * 0.1f;

            for (int i = 0; i < radii.Length; i++)
            {
                draw.DrawDot(new CCPoint(x, s.Height * 0.5f), radii[i],
                    new CCColor4F(CCRandom.Float_0_1(), CCRandom.Float_0_1(), CCRandom.Float_0_1(), 1f));

                var label = new CCLabelTTF($"r={radii[i]}", "arial", 12);
                label.Position = new CCPoint(x, s.Height * 0.3f);
                AddChild(label);

                x += radii[i] * 2 + 30;
            }

            // DrawCircle - multiple overlapping should not have compositing artifacts
            var colors = new CCColor4B[] {
                new CCColor4B(255, 0, 0, 128),
                new CCColor4B(0, 255, 0, 128),
                new CCColor4B(0, 0, 255, 128)
            };

            for (int i = 0; i < 3; i++)
            {
                draw.DrawCircle(
                    new CCPoint(s.Width * 0.75f + i * 30, s.Height * 0.5f),
                    40, colors[i]);
            }

            var labelOverlap = new CCLabelTTF("Overlapping circles - no artifacts", "arial", 14);
            labelOverlap.Position = new CCPoint(s.Width * 0.78f, s.Height * 0.3f);
            AddChild(labelOverlap);

            return true;
        }
    }

    /// <summary>
    /// Tests CCColor4B <-> CCColor4F implicit conversions and color constants.
    /// </summary>
    public class DrawNodeColorConversionsTest : BaseDrawNodeTest
    {
        public override string title() { return "Color Conversions & Constants"; }
        public override string subtitle() { return "CCColor4B/4F implicit conversion + named colors"; }

        public override bool Init()
        {
            base.Init();

            CCSize s = CCDirector.SharedDirector.WinSize;
            CCDrawNode draw = new CCDrawNode();
            AddChild(draw, 10);

            // Test CCColor4F named constants
            CCColor4F[] namedColors = {
                CCColor4F.Red, CCColor4F.Green, CCColor4F.Blue,
                CCColor4F.Yellow, CCColor4F.Magenta, CCColor4F.Orange,
                CCColor4F.Gray, CCColor4F.White
            };
            string[] colorNames = {
                "Red", "Green", "Blue", "Yellow", "Magenta", "Orange", "Gray", "White"
            };

            float startX = s.Width * 0.05f;
            float spacing = (s.Width * 0.9f) / namedColors.Length;

            for (int i = 0; i < namedColors.Length; i++)
            {
                float cx = startX + i * spacing + spacing * 0.5f;

                // Draw using CCColor4F constant
                draw.DrawFilledCircle(new CCPoint(cx, s.Height * 0.6f), 20f, namedColors[i]);

                // Test implicit conversion: CCColor4F -> CCColor4B -> CCColor4F roundtrip
                CCColor4B asBytes = namedColors[i]; // implicit CCColor4F -> CCColor4B
                CCColor4F backToFloat = asBytes;     // implicit CCColor4B -> CCColor4F
                draw.DrawFilledCircle(new CCPoint(cx, s.Height * 0.4f), 15f, backToFloat);

                var label = new CCLabelTTF(colorNames[i], "arial", 11);
                label.Position = new CCPoint(cx, s.Height * 0.28f);
                AddChild(label);
            }

            var topLabel = new CCLabelTTF("Top: CCColor4F constants", "arial", 14);
            topLabel.Position = new CCPoint(s.Width * 0.5f, s.Height * 0.75f);
            AddChild(topLabel);

            var bottomLabel = new CCLabelTTF("Bottom: After B->F roundtrip (should match)", "arial", 14);
            bottomLabel.Position = new CCPoint(s.Width * 0.5f, s.Height * 0.22f);
            AddChild(bottomLabel);

            return true;
        }
    }
}
