using System;
namespace cocos2d.EmbeddableView.OpenTK.Input
{
    /// <summary>
    /// Describes the state of a joystick hat.
    /// </summary>
    public struct JoystickHatState : IEquatable<JoystickHatState>
    {
        internal JoystickHatState(HatPosition pos)
        {
            Position = pos;
        }

        /// <summary>
        /// Gets a <see cref="HatPosition"/> value indicating
        /// the position of this hat.
        /// </summary>
        /// <value>The position.</value>
        public HatPosition Position { get; }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating
        /// whether this hat lies in the top hemicircle.
        /// </summary>
        /// <value><c>true</c> if this hat lies in the top hemicircle; otherwise, <c>false</c>.</value>
        public bool IsUp
        {
            get
            {
                return
                    Position == HatPosition.Up ||
                    Position == HatPosition.UpLeft ||
                    Position == HatPosition.UpRight;
            }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating
        /// whether this hat lies in the bottom hemicircle.
        /// </summary>
        /// <value><c>true</c> if this hat lies in the bottom hemicircle; otherwise, <c>false</c>.</value>
        public bool IsDown
        {
            get
            {
                return
                    Position == HatPosition.Down ||
                    Position == HatPosition.DownLeft ||
                    Position == HatPosition.DownRight;
            }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating
        /// whether this hat lies in the left hemicircle.
        /// </summary>
        /// <value><c>true</c> if this hat lies in the left hemicircle; otherwise, <c>false</c>.</value>
        public bool IsLeft
        {
            get
            {
                return
                    Position == HatPosition.Left ||
                    Position == HatPosition.UpLeft ||
                    Position == HatPosition.DownLeft;
            }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating
        /// whether this hat lies in the right hemicircle.
        /// </summary>
        /// <value><c>true</c> if this hat lies in the right hemicircle; otherwise, <c>false</c>.</value>
        public bool IsRight
        {
            get
            {
                return
                    Position == HatPosition.Right ||
                    Position == HatPosition.UpRight ||
                    Position == HatPosition.DownRight;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="OpenTK.Input.JoystickHatState"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="OpenTK.Input.JoystickHatState"/>.</returns>
        public override string ToString()
        {
            return String.Format(
                "{{{0}{1}{2}{3}}}",
                IsUp ? "U" : String.Empty,
                IsLeft ? "L" : String.Empty,
                IsDown ? "D" : String.Empty,
                IsRight ? "R" : String.Empty);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="OpenTK.Input.JoystickHatState"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="OpenTK.Input.JoystickHatState"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="OpenTK.Input.JoystickHatState"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="OpenTK.Input.JoystickHatState"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return
                obj is JoystickHatState &&
                Equals((JoystickHatState)obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="OpenTK.Input.JoystickHatState"/> is equal to the current <see cref="OpenTK.Input.JoystickHatState"/>.
        /// </summary>
        /// <param name="other">The <see cref="OpenTK.Input.JoystickHatState"/> to compare with the current <see cref="OpenTK.Input.JoystickHatState"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="OpenTK.Input.JoystickHatState"/> is equal to the current
        /// <see cref="OpenTK.Input.JoystickHatState"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(JoystickHatState other)
        {
            return Position == other.Position;
        }
    }
}

