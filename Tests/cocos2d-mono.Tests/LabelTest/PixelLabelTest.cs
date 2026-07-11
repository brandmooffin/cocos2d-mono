using System;
using Cocos2D;

namespace tests;

/// <summary>
/// Tests CCPixelLabel — optimized label for frequent text updates.
/// Demonstrates that changing .Text only repositions sprites without
/// regenerating textures, and shows alignment and color options.
/// </summary>
public class PixelLabelTest : AtlasDemo
{
    private CCPixelLabel _scoreLabel;
    private CCPixelLabel _centerLabel;
    private CCPixelLabel _rightLabel;
    private CCPixelLabel _colorLabel;
    private CCLabelTTF _fpsLabel;
    private int _counter;
    private float _elapsed;
    private int _frames;

    public PixelLabelTest()
    {
        CCSize s = CCDirector.SharedDirector.WinSize;

        // Left-aligned score (most common HUD use case)
        _scoreLabel = new CCPixelLabel("Score: 0", "arial", 28, CCTextAlignment.Left);
        _scoreLabel.Position = new CCPoint(20, s.Height - 40);
        _scoreLabel.AnchorPoint = CCPoint.Zero;
        AddChild(_scoreLabel);

        // Center-aligned label
        _centerLabel = new CCPixelLabel("CENTER", "arial", 24, CCTextAlignment.Center);
        _centerLabel.Position = new CCPoint(s.Width / 2, s.Height * 0.6f);
        AddChild(_centerLabel);

        // Right-aligned label
        _rightLabel = new CCPixelLabel("Right: 0", "arial", 24, CCTextAlignment.Right);
        _rightLabel.Position = new CCPoint(s.Width - 20, s.Height * 0.45f);
        AddChild(_rightLabel);

        // Color and opacity demo
        _colorLabel = new CCPixelLabel("Colored Text!", "arial", 22, CCTextAlignment.Center);
        _colorLabel.Position = new CCPoint(s.Width / 2, s.Height * 0.3f);
        _colorLabel.Color = new CCColor3B(255, 200, 0);
        AddChild(_colorLabel);

        // FPS-style comparison: shows update count
        _fpsLabel = new CCLabelTTF("Updates: 0", "arial", 14);
        _fpsLabel.Position = new CCPoint(s.Width / 2, 30);
        _fpsLabel.Color = CCColor3B.Gray;
        AddChild(_fpsLabel);

        _counter = 0;
        _elapsed = 0;
        _frames = 0;

        Schedule(UpdateLabels);
    }

    private void UpdateLabels(float dt)
    {
        _counter++;
        _elapsed += dt;
        _frames++;

        // Update score every frame — this is the key performance test
        _scoreLabel.Text = "Score: " + (_counter * 10);

        // Update center with changing text length
        if (_counter % 60 < 20)
            _centerLabel.Text = "SHORT";
        else if (_counter % 60 < 40)
            _centerLabel.Text = "MEDIUM LENGTH";
        else
            _centerLabel.Text = "THIS IS A LONGER STRING";

        // Update right-aligned
        _rightLabel.Text = "Right: " + _counter;

        // Cycle color label opacity
        byte opacity = (byte)(128 + (int)(127 * Math.Sin(_elapsed * 2.0)));
        _colorLabel.Opacity = opacity;

        // Update FPS counter less frequently (every 30 frames)
        if (_frames % 30 == 0)
        {
            _fpsLabel.Text = "Updates: " + _counter + " (CCPixelLabel = no texture regen)";
        }
    }

    public override string title()
    {
        return "CCPixelLabel Performance Test";
    }

    public override string subtitle()
    {
        return "Text updates every frame with no texture regeneration";
    }
}
