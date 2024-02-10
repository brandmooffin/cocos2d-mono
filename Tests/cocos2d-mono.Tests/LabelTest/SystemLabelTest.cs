using Cocos2D;

namespace cocos2d_mono.Tests.LabelTest
{
    public class SystemLabelTest : AtlasDemo
    {
        CCLabel smallSystemLabel;
        CCLabel mediumSystemLabel;
        CCLabel largeSystemLabel;
        CCLabel smallAdjustDimensionSystemLabel;
        CCLabel multiLineSystemLabel;
        CCLabel japaneseSystemLabel;

        public SystemLabelTest()
        {
            CCSize s = CCDirector.SharedDirector.WinSize;


            smallSystemLabel = new CCLabel("Test with small size", "Arial", 12)
            {
                Position = new CCPoint(s.Center.X, s.Center.Y - 200)
            };


            mediumSystemLabel = new CCLabel("Test with medium size", "Arial", 32)
            {
                Position = new CCPoint(s.Center.X, s.Center.Y - 100)
            };

            largeSystemLabel = new CCLabel("Test with large size", "Arial", 96)
            {
                Position = new CCPoint(s.Center.X, s.Center.Y)
            };


            smallAdjustDimensionSystemLabel = new CCLabel("Test with adjusted dimensions", "Arial", 12)
            {
                Position = new CCPoint(s.Center.X - 300, s.Center.Y + 250)
            };

            multiLineSystemLabel = new CCLabel("Test with multi\nlines and different\ncharacters\n複数行と異なる文字でテストする\nマルチでテストする\n行と異なる", "Arial", 12)
            {
                Position = new CCPoint(s.Center.X, s.Center.Y + 125)
            };

            japaneseSystemLabel = new CCLabel("矢印の色がセルの背景色と同じの場合、それがたどる方向です。", "Arial", 28) 
            {
                Position = new CCPoint(s.Center.X + 150, s.Center.Y + 150),
                Dimensions = new CCSize(150, 0),
                LineBreakMode = CCTextLineBreakMode.SmartBreak
            };

            AddChild(smallSystemLabel);
            AddChild(smallAdjustDimensionSystemLabel);
            AddChild(multiLineSystemLabel);
            AddChild(mediumSystemLabel);
            AddChild(largeSystemLabel);
            AddChild(japaneseSystemLabel);

            smallAdjustDimensionSystemLabel.Dimensions = new CCSize(500, 0);
            string text = "Thank you for visiting the cocos2d-mono tests\nPlease help us by donating to our project\nYou can find us at cocos2d-mono.dev\n\n\nThank you!\nDon't forget to contribute to cocos2d-x\nWithout them this project would not exist.";
            smallAdjustDimensionSystemLabel.Text = text;
        }
        public override string title()
        {
            return "SystemFont";
        }

        public override string subtitle()
        {
            return "CCLabel using System Font";
        }
    }
}
