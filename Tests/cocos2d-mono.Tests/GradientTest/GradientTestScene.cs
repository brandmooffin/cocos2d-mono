using Cocos2D;

namespace tests
{
    public class GradientTestScene : TestScene
    {
        private static int sceneIdx = -1;
        private static int MAX_LAYER = 4;

        public override void runThisTest()
        {
            CCLayer pLayer = nextTestAction();
            AddChild(pLayer);
            CCDirector.SharedDirector.ReplaceScene(this);
        }

        public static CCLayer createTestLayer(int nIndex)
        {
            switch (nIndex)
            {
                case 0: return new MultiGradientVerticalTest();
                case 1: return new MultiGradientHorizontalTest();
                case 2: return new MultiGradientDynamicTest();
                case 3: return new MultiGradientSunriseTest();
            }
            return null;
        }

        protected override void NextTestCase() { nextTestAction(); }
        protected override void PreviousTestCase() { backTestAction(); }
        protected override void RestTestCase() { restartTestAction(); }

        public static CCLayer nextTestAction()
        {
            sceneIdx++;
            sceneIdx = sceneIdx % MAX_LAYER;
            return createTestLayer(sceneIdx);
        }

        public static CCLayer backTestAction()
        {
            sceneIdx--;
            if (sceneIdx < 0) sceneIdx += MAX_LAYER;
            return createTestLayer(sceneIdx);
        }

        public static CCLayer restartTestAction()
        {
            return createTestLayer(sceneIdx);
        }
    }
}
