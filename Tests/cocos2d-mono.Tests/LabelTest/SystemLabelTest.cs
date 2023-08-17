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
                Position = new CCPoint(s.Center.X, s.Center.Y + 200)
            };

            multiLineSystemLabel = new CCLabel("Test with multi\nlines and different\ncharacters\n複数行と異なる文字でテストする\nマルチでテストする\n行と異なる", "Arial", 12)
            {
                Position = new CCPoint(s.Center.X, s.Center.Y + 125)
            };

            AddChild(smallSystemLabel);
            AddChild(smallAdjustDimensionSystemLabel);
            AddChild(multiLineSystemLabel);
            AddChild(mediumSystemLabel);
            AddChild(largeSystemLabel);
            
            smallAdjustDimensionSystemLabel.Dimensions = new CCSize(smallAdjustDimensionSystemLabel.Dimensions.Width, smallAdjustDimensionSystemLabel.ContentSize.Height + 200);
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
