using Cocos2D;

namespace tests
{
    public class ParticleEmitterLightTestScene : TestScene
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
                case 0: return new ParticleEmitterLightBasicTest();
                case 1: return new ParticleEmitterLightColorsTest();
                case 2: return new ParticleEmitterLightCustomTest();
                case 3: return new ParticleEmitterLightStopTest();
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
