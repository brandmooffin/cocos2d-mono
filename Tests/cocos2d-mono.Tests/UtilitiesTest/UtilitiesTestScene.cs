using Cocos2D;

namespace tests
{
    public class UtilitiesTestScene : TestScene
    {
        private static int sceneIdx = -1;
        private static int MAX_LAYER = 3;

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
                case 0: return new ObjectPoolTest();
                case 1: return new CollisionTest();
                case 2: return new CooldownTimerTest();
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
