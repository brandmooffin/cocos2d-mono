using System;
namespace cocos2d.EmbeddableView.OpenTK.Input
{
    internal class GamePadConfigurationItem
    {
        public GamePadConfigurationItem(GamePadConfigurationSource source, GamePadConfigurationTarget target)
        {
            Source = source;
            Target = target;
        }

        public GamePadConfigurationSource Source { get; private set; }

        public GamePadConfigurationTarget Target { get; private set; }
    }
}

