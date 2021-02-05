﻿using Cocos2D;

namespace Cocos2DMonoGame.Android
{
    public class IntroLayer : CCLayerColor
    {
        public IntroLayer()
        {

            // create and initialize a Label
            var label = new CCLabelTTF("Hello World!", "MarkerFelt", 22)
            {
                // position the label on the center of the screen
                Position = CCDirector.SharedDirector.WinSize.Center
            };

            // add the label as a child to this Layer
            AddChild(label);

            // setup our color for the background
            Color = new CCColor3B(Microsoft.Xna.Framework.Color.Blue);
            Opacity = 255;
        }

        public static CCScene Scene
        {
            get
            {
                // 'scene' is an autorelease object.
                var scene = new CCScene();

                // 'layer' is an autorelease object.
                var layer = new IntroLayer();

                // add layer as a child to scene
                scene.AddChild(layer);

                // return the scene
                return scene;
            }
        }
    }
}