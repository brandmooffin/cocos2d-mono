using System;
using Microsoft.Xna.Framework.Input;

namespace cocos2d.EmbeddableView.OpenTK.Input
{
    /// <summary>
    /// Provides access to Joystick devices.
    /// Joystick devices provide a varying number of axes and buttons.
    /// Use <c>GetCapabilities</c> to retrieve the number of supported
    /// axes and buttons on a given device.
    /// Use <c>GetState</c> to retrieve the current state of a given device.
    /// </summary>
    /// <seealso cref="GamePad"/>
    public sealed class Joystick
    {
        private static readonly IJoystickDriver2 implementation =
            Platform.Factory.Default.CreateJoystickDriver();

        private Joystick() { }

        /// <summary>
        /// Retrieves the <see cref="JoystickCapabilities"/> of the device connected
        /// at the specified index.
        /// </summary>
        /// <returns>
        /// A <see cref="JoystickCapabilities"/> structure describing
        /// the capabilities of the device at the specified index.
        /// If no device is connected at the specified index, the <c>IsConnected</c>
        /// property of the returned structure will be false.
        /// </returns>
        /// <param name="index">The zero-based index of the device to poll.</param>
        public static JoystickCapabilities GetCapabilities(int index)
        {
            return implementation.GetCapabilities(index);
        }

        /// <summary>
        /// Retrieves the <see cref="JoystickState"/> of the device connected
        /// at the specified index.
        /// </summary>
        /// <returns>A <see cref="JoystickState"/> structure describing
        /// the current state of the device at the specified index.
        /// If no device is connected at this index, the <c>IsConnected</c>
        /// property of the returned structure will be false.
        /// </returns>
        /// <param name="index">The zero-based index of the device to poll.</param>
        public static JoystickState GetState(int index)
        {
            return implementation.GetState(index);
        }

        /// <summary>
        /// Retrieves the ID of the device connected
        /// at the specified index.
        /// </summary>
        /// <returns>
        /// A <see cref="Guid"/> representing the id of the connected device.
        /// </returns>
        /// <param name="index">The zero-based index of the device to poll.</param>
        public static Guid GetGuid(int index)
        {
            return implementation.GetGuid(index);
        }

        //public string GetName(int index)
        //{
        //    return implementation.GetName(index);
        //}
    }
}

