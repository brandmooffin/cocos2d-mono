using System;
using Microsoft.Xna.Framework.Input;

namespace cocos2d.EmbeddableView.OpenTK.Input
{
    internal struct GamePadConfigurationTarget
    {
        private Nullable<Buttons> map_button;
        private Nullable<GamePadAxes> map_axis;

        public GamePadConfigurationTarget(Buttons button)
            : this()
        {
            Type = ConfigurationType.Button;
            map_button = button;
        }

        public GamePadConfigurationTarget(GamePadAxes axis)
            : this()
        {
            Type = ConfigurationType.Axis;
            map_axis = axis;
        }

        public ConfigurationType Type { get; private set; }

        public GamePadAxes Axis
        {
            get { return map_axis.Value; }
            private set { map_axis = value; }
        }

        public Buttons Button
        {
            get { return map_button.Value; }
            private set { map_button = value; }
        }
    }
}

