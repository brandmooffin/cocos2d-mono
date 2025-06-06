using Cocos2D;

namespace tests.FontTest
{
    public class FontTestScene : TestScene
    {
        private static int fontIdx;

        private static readonly string[] fontList =
            {
//#if IOS || MACOS
//                "American Typewriter",
//                "Marker Felt",
//				"Chalkboard",
//#endif
                "A Damn Mess",
                "Abberancy",
                "Abduction",
                "Paint Boy",
                "Schwarzwald Regular",
                "Scissor Cuts",
            };

        public static int vAlignIdx = 0;

        public static CCVerticalTextAlignment[] verticalAlignment =
            {
                CCVerticalTextAlignment.Top,
                CCVerticalTextAlignment.Center,
                CCVerticalTextAlignment.Bottom
            };

        public override void runThisTest()
        {
            CCLayer pLayer = new FontTest();
            AddChild(pLayer);

            CCDirector.SharedDirector.ReplaceScene(this);
        }

        protected override void NextTestCase()
        {
            nextAction();
        }

        protected override void PreviousTestCase()
        {
            backAction();
        }

        protected override void RestTestCase()
        {
            restartAction();
        }

        public static string nextAction()
        {
            fontIdx++;
            if (fontIdx >= fontList.Length)
            {
                fontIdx = 0;
                vAlignIdx = (vAlignIdx + 1) % verticalAlignment.Length;
            }
            return fontList[fontIdx];
        }

        public static string backAction()
        {
            fontIdx--;
            if (fontIdx < 0)
            {
                fontIdx = fontList.Length - 1;
                vAlignIdx--;
                if (vAlignIdx < 0)
                    vAlignIdx = verticalAlignment.Length - 1;
            }

            return fontList[fontIdx];
        }

        public static string restartAction()
        {
            return fontList[fontIdx];
        }
    }


    public class FontTest : CCLayer
    {
        private const int kTagLabel1 = 1;
        private const int kTagLabel2 = 2;
        private const int kTagLabel3 = 3;
        private const int kTagLabel4 = 4;

        public FontTest()
        {
            CCSize s = CCDirector.SharedDirector.WinSize;
            CCMenuItemImage item1 = new CCMenuItemImage(TestResource.s_pPathB1, TestResource.s_pPathB2, backCallback);
            CCMenuItemImage item2 = new CCMenuItemImage(TestResource.s_pPathR1, TestResource.s_pPathR2, restartCallback);
            CCMenuItemImage item3 = new CCMenuItemImage(TestResource.s_pPathF1, TestResource.s_pPathF2, nextCallback);

            CCMenu menu = new CCMenu(item1, item2, item3);
            menu.Position = CCPoint.Zero;
            item1.Position = new CCPoint(s.Width / 2 - item2.ContentSize.Width * 2, item2.ContentSize.Height / 2);
            item2.Position = new CCPoint(s.Width / 2, item2.ContentSize.Height / 2);
            item3.Position = new CCPoint(s.Width / 2 + item2.ContentSize.Width * 2, item2.ContentSize.Height / 2);

            item1.Scale = 0.5f;
            item2.Scale = 0.5f;
            item3.Scale = 0.5f;

            AddChild(menu, 11);

            var blockSize = new CCSize(s.Width / 3, 200);

            CCLayerColor leftColor = new CCLayerColor(new CCColor4B(100, 100, 100, 255), blockSize.Width,
                                                      blockSize.Height);
            CCLayerColor centerColor = new CCLayerColor(new CCColor4B(200, 100, 100, 255), blockSize.Width,
                                                        blockSize.Height);
            CCLayerColor rightColor = new CCLayerColor(new CCColor4B(100, 100, 200, 255), blockSize.Width,
                                                       blockSize.Height);

            leftColor.IgnoreAnchorPointForPosition = false;
            centerColor.IgnoreAnchorPointForPosition = false;
            rightColor.IgnoreAnchorPointForPosition = false;

            leftColor.AnchorPoint = new CCPoint(0, 0.5f);
            centerColor.AnchorPoint = new CCPoint(0, 0.5f);
            rightColor.AnchorPoint = new CCPoint(0, 0.5f);

            leftColor.Position = new CCPoint(0, s.Height / 2);
            ;
            centerColor.Position = new CCPoint(blockSize.Width, s.Height / 2);
            rightColor.Position = new CCPoint(blockSize.Width * 2, s.Height / 2);

            AddChild(leftColor, -1);
            AddChild(rightColor, -1);
            AddChild(centerColor, -1);

            showFont(FontTestScene.restartAction());
        }

        public void showFont(string pFont)
        {
            CCSize s = CCDirector.SharedDirector.WinSize;

            var blockSize = new CCSize(s.Width / 3, 200);
            float fontSize = 26;

            RemoveChildByTag(kTagLabel1, true);
            RemoveChildByTag(kTagLabel2, true);
            RemoveChildByTag(kTagLabel3, true);
            RemoveChildByTag(kTagLabel4, true);

            CCLabelTTF top = new CCLabelTTF(pFont, "Arial", 24);
            CCLabelTTF left = new CCLabelTTF("alignment left", pFont, fontSize,
                                             blockSize, CCTextAlignment.Left,
                                             FontTestScene.verticalAlignment[FontTestScene.vAlignIdx]);
            CCLabelTTF center = new CCLabelTTF("alignment center", pFont, fontSize,
                                               blockSize, CCTextAlignment.Center,
                                               FontTestScene.verticalAlignment[FontTestScene.vAlignIdx]);
            CCLabelTTF right = new CCLabelTTF("alignment right", pFont, fontSize,
                                              blockSize, CCTextAlignment.Right,
                                              FontTestScene.verticalAlignment[FontTestScene.vAlignIdx]);

            top.AnchorPoint = new CCPoint(0.5f, 1);
            left.AnchorPoint = new CCPoint(0, 0.5f);
            center.AnchorPoint = new CCPoint(0, 0.5f);
            right.AnchorPoint = new CCPoint(0, 0.5f);

            top.Position = new CCPoint(s.Width / 2, s.Height - 20);
            left.Position = new CCPoint(0, s.Height / 2);
            center.Position = new CCPoint(blockSize.Width, s.Height / 2);
            right.Position = new CCPoint(blockSize.Width * 2, s.Height / 2);

            AddChild(left, 0, kTagLabel1);
            AddChild(right, 0, kTagLabel2);
            AddChild(center, 0, kTagLabel3);
            AddChild(top, 0, kTagLabel4);
        }

        public void restartCallback(object pSender)
        {
            showFont(FontTestScene.restartAction());
        }

        public void nextCallback(object pSender)
        {
            showFont(FontTestScene.nextAction());
        }

        public void backCallback(object pSender)
        {
            showFont(FontTestScene.backAction());
        }

        public virtual string title()
        {
            return "Font test";
        }
    }
}