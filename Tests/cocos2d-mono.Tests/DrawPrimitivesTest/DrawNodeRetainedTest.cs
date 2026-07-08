using System;
using System.Collections.Generic;
using Cocos2D;

namespace tests;

/// <summary>
/// Tests CCDrawNodeRetained: adding shapes, moving, recoloring, and removing
/// individual shapes without clearing the entire buffer.
/// </summary>
public class DrawNodeRetainedTest : BaseDrawNodeTest
{
    private CCDrawNodeRetained _drawNode;
    private CCShapeHandle _movingDot;
    private CCShapeHandle _colorRect;
    private CCShapeHandle _fadingCircle;
    private CCLabelTTF _statusLabel;
    private float _elapsed;
    private int _colorPhase;

    public override string title() { return "CCDrawNodeRetained"; }
    public override string subtitle() { return "Shapes move/recolor without Clear()"; }

    public override bool Init()
    {
        base.Init();

        CCSize s = CCDirector.SharedDirector.WinSize;

        _drawNode = new CCDrawNodeRetained();
        AddChild(_drawNode, 10);

        // Static shapes that never change
        _drawNode.AddFilledCircle(new CCPoint(80, s.Height * 0.7f), 30f,
            new CCColor4F(0.3f, 0.3f, 0.3f, 1f));
        _drawNode.AddTriangle(
            new CCPoint(160, s.Height * 0.6f),
            new CCPoint(200, s.Height * 0.8f),
            new CCPoint(240, s.Height * 0.6f),
            new CCColor4F(0.5f, 0.5f, 0.5f, 1f));

        // Moving dot — will be repositioned every frame
        _movingDot = _drawNode.AddDot(new CCPoint(s.Width / 2, s.Height * 0.5f), 15f,
            CCColor4F.Red);

        // Color-cycling rectangle
        _colorRect = _drawNode.AddRect(
            new CCRect(s.Width * 0.6f, s.Height * 0.3f, 100, 60),
            CCColor4F.Green);

        // Fading circle
        _fadingCircle = _drawNode.AddFilledCircle(
            new CCPoint(s.Width * 0.8f, s.Height * 0.6f), 40f,
            CCColor4F.Blue);

        _statusLabel = new CCLabelTTF("Shapes: 5 | No Clear() calls", "arial", 14);
        _statusLabel.Position = new CCPoint(s.Width / 2, 60);
        AddChild(_statusLabel);

        _elapsed = 0;
        _colorPhase = 0;

        Schedule(UpdateShapes);

        return true;
    }

    private void UpdateShapes(float dt)
    {
        _elapsed += dt;
        CCSize s = CCDirector.SharedDirector.WinSize;

        // Move dot in a circle
        float cx = s.Width / 2 + (float)Math.Cos(_elapsed * 2.0) * 120f;
        float cy = s.Height * 0.5f + (float)Math.Sin(_elapsed * 2.0) * 80f;
        _drawNode.SetShapePosition(_movingDot, new CCPoint(cx, cy));

        // Cycle rectangle color every 0.5s
        if ((int)(_elapsed * 2) != _colorPhase)
        {
            _colorPhase = (int)(_elapsed * 2);
            switch (_colorPhase % 4)
            {
                case 0: _drawNode.RecolorShape(_colorRect, CCColor4F.Red); break;
                case 1: _drawNode.RecolorShape(_colorRect, CCColor4F.Green); break;
                case 2: _drawNode.RecolorShape(_colorRect, CCColor4F.Blue); break;
                case 3: _drawNode.RecolorShape(_colorRect, CCColor4F.Yellow); break;
            }
        }

        // Pulse circle opacity
        byte opacity = (byte)(128 + (int)(127 * Math.Sin(_elapsed * 3.0)));
        _drawNode.SetShapeOpacity(_fadingCircle, opacity);

        _statusLabel.Text = string.Format("Shapes: {0} | Verts: {1} | No Clear()",
            _drawNode.ShapeCount, _drawNode.VertexCount);
    }
}

/// <summary>
/// Tests removing shapes from CCDrawNodeRetained and verifying
/// that remaining shapes stay correctly positioned.
/// </summary>
public class DrawNodeRetainedRemoveTest : BaseDrawNodeTest
{
    private CCDrawNodeRetained _drawNode;
    private List<CCShapeHandle> _shapes;
    private CCLabelTTF _statusLabel;
    private float _elapsed;
    private float _nextRemoveTime;

    public override string title() { return "CCDrawNodeRetained Remove"; }
    public override string subtitle() { return "Shapes removed one-by-one every 1s"; }

    public override bool Init()
    {
        base.Init();

        CCSize s = CCDirector.SharedDirector.WinSize;

        _drawNode = new CCDrawNodeRetained();
        AddChild(_drawNode, 10);

        _shapes = new List<CCShapeHandle>();

        // Create a row of colored circles
        for (int i = 0; i < 8; i++)
        {
            float x = 60 + i * (s.Width - 120) / 7f;
            float hue = i / 8f;
            var color = new CCColor4F(
                Math.Max(0, 1f - Math.Abs(hue - 0.0f) * 3f),
                Math.Max(0, 1f - Math.Abs(hue - 0.33f) * 3f),
                Math.Max(0, 1f - Math.Abs(hue - 0.66f) * 3f),
                1f);
            var handle = _drawNode.AddFilledCircle(new CCPoint(x, s.Height * 0.5f), 25f, color);
            _shapes.Add(handle);
        }

        _statusLabel = new CCLabelTTF("Shapes: 8", "arial", 16);
        _statusLabel.Position = new CCPoint(s.Width / 2, 60);
        AddChild(_statusLabel);

        _elapsed = 0;
        _nextRemoveTime = 1f;

        Schedule(UpdateRemoval);
        return true;
    }

    private void UpdateRemoval(float dt)
    {
        _elapsed += dt;

        if (_elapsed >= _nextRemoveTime && _shapes.Count > 0)
        {
            // Remove from the middle to test index compaction
            int idx = _shapes.Count / 2;
            _drawNode.RemoveShape(_shapes[idx]);
            _shapes.RemoveAt(idx);
            _nextRemoveTime += 1f;
        }

        _statusLabel.Text = string.Format("Shapes: {0} | Verts: {1}",
            _drawNode.ShapeCount, _drawNode.VertexCount);
    }

}
