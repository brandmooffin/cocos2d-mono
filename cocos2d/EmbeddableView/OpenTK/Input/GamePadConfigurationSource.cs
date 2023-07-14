using System;
using Microsoft.Xna.Framework.Input;

namespace cocos2d.EmbeddableView.OpenTK.Input
{
    internal struct GamePadConfigurationSource
    {
        private int? map_button;
        private int? map_axis;
        private JoystickHat? map_hat;
        private HatPosition? map_hat_position;

        /// <summary>
        /// Creates a new gamepad configuration source from an axis or a button
        /// </summary>
        /// <param name="isAxis">Whether this source is an axis or a button</param>
        /// <param name="index">The index of this source</param>
        public GamePadConfigurationSource(bool isAxis, int index)
            : this()
        {
            if (isAxis)
            {
                Type = ConfigurationType.Axis;
                Axis = index;
            }
            else
            {
                Type = ConfigurationType.Button;
                Button = index;
            }
        }

        /// <summary>
        /// Creates a new gamepad configuration source from a hat
        /// </summary>
        /// <param name="hat">The hat</param>
        /// <param name="pos">The starting hat position</param>
        public GamePadConfigurationSource(JoystickHat hat, HatPosition pos)
            : this()
        {
            Type = ConfigurationType.Hat;
            Hat = hat;
            map_hat_position = pos;
        }

        public ConfigurationType Type { get; private set; }

        /// <summary>
        /// Represents a gamepad axis
        /// </summary>
        public int Axis
        {
            get { return map_axis.Value; }
            private set { map_axis = value; }
        }

        /// <summary>
        /// Represents a gamepad button
        /// </summary>
        public int Button
        {
            get { return map_button.Value; }
            private set { map_button = value; }
        }

        /// <summary>
        /// Represents a gamepad hat
        /// </summary>
        public JoystickHat Hat
        {
            get { return map_hat.Value; }
            private set { map_hat = value; }
        }

        /// <summary>
        /// Represents the position of a gamepad hat
        /// </summary>
        public HatPosition HatPosition
        {
            get { return map_hat_position.Value; }
            private set { map_hat_position = value; }
        }
    }
}

