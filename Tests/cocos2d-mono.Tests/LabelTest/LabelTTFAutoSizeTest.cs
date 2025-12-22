using Cocos2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace tests
{
    public class LabelTTFAutoSizeTest : AtlasDemo
    {
        private CCTextAlignment m_eHorizAlign;
        private CCVerticalTextAlignment m_eVertAlign;
        private CCLabelTTF m_plabel;
        private CCLabelTTF m_label2;

        private SpriteFont customFont;

        public LabelTTFAutoSizeTest()
        {
            customFont = CCContentManager.SharedContentManager.Load<SpriteFont>("fonts/SFFedora-32");

            var blockSize = new CCSize(400, 160);
            CCSize s = CCDirector.SharedDirector.WinSize;

            CCLayerColor colorLayer = new CCLayerColor(new CCColor4B(100, 100, 100, 255), blockSize.Width, blockSize.Height);
            colorLayer.AnchorPoint = new CCPoint(0, 0);
            colorLayer.Position = new CCPoint((s.Width - blockSize.Width) / 2, (s.Height - blockSize.Height) / 2);

            AddChild(colorLayer);

            m_plabel = null;
            updateAlignment();
        }

        private void updateAlignment()
        {
            var blockSize = new CCSize(400, 160);
            CCSize s = CCDirector.SharedDirector.WinSize;

            m_label2 = new CCLabelTTF("AutoSize Font", "SFFedoraTitles", 32, new CCSize(400, 64), CCTextAlignment.Left, CCVerticalTextAlignment.Center);
            m_label2.AnchorPoint = new CCPoint(0, 0);
            m_label2.Position = new CCPoint((s.Width - blockSize.Width) / 2, (s.Height - blockSize.Height +150) / 2);

            AddChild(m_label2);

            m_plabel = new CCLabelTTF("AutoSize Custom Font", "SFFedora", 32);
            m_plabel.AnchorPoint = new CCPoint(0, 0);
            //m_plabel.VerticalAlignment = CCVerticalTextAlignment.Center;
            m_plabel.Position = new CCPoint((s.Width - blockSize.Width) / 2, (s.Height - blockSize.Height) / 2);


            AddChild(m_plabel);
        }

        public override void Draw()
        {
            base.Draw();

            CCDrawManager.spriteBatch.Begin();

            CCDrawManager.spriteBatch.DrawString(customFont, "AutoSize Custom Font", new CCPoint(m_plabel.PositionX, m_plabel.PositionY - 200), Microsoft.Xna.Framework.Color.White);

            CCDrawManager.spriteBatch.End();
        }

        public override string title()
        {
            return "Testing CCLabelTTF AutoSize";
        }

        public override string subtitle()
        {
            return "You should see a label with auto size";
        }
    }
}