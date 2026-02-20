using System;
using Cocos2D;

namespace tests
{
    public class EmbeddableViewTestScene : TestScene
    {
        protected override void NextTestCase()
        {
            nextEmbeddableViewAction();
        }

        protected override void PreviousTestCase()
        {
            backEmbeddableViewAction();
        }

        protected override void RestTestCase()
        {
            restartEmbeddableViewAction();
        }

        private void nextEmbeddableViewAction()
        {
            CCScene s = new EmbeddableViewTestScene();
            s.AddChild(EmbeddableViewTest.nextTestAction());
            CCDirector.SharedDirector.ReplaceScene(s);
        }

        private void backEmbeddableViewAction()
        {
            CCScene s = new EmbeddableViewTestScene();
            s.AddChild(EmbeddableViewTest.backTestAction());
            CCDirector.SharedDirector.ReplaceScene(s);
        }

        private void restartEmbeddableViewAction()
        {
            CCScene s = new EmbeddableViewTestScene();
            s.AddChild(EmbeddableViewTest.restartTestAction());
            CCDirector.SharedDirector.ReplaceScene(s);
        }

        public override void runThisTest()
        {
            CCLayer pLayer = EmbeddableViewTest.nextTestAction();
            AddChild(pLayer);
            CCDirector.SharedDirector.ReplaceScene(this);
        }
    }
}
