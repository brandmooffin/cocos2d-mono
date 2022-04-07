using Microsoft.Xna.Framework;
using System;
using TetrisGame.Core.Enums;

namespace TetrisGame.Core.Managers
{
    internal class InputManager
    {
        public FlickDirection CurrentFlickDirection = FlickDirection.Unknown;
        public bool IsTapping = false;

        private static InputManager inputManager;

        public static InputManager Instance
        {
            get
            {
                if (inputManager == null)
                {
                    inputManager = new InputManager();
                }
                return inputManager;
            }
        }

        public void ClearInputs()
        {
            CurrentFlickDirection = FlickDirection.Unknown;
            IsTapping = false;
        }

        public void SetFlickDirection(Vector2 delta)
        {
            CurrentFlickDirection = GetFlickDirection(delta);
        }

        public FlickDirection GetFlickDirection(Vector2 delta)
        {
            float absX = Math.Abs(delta.X);
            float absY = Math.Abs(delta.Y);

            //if the absolute value of delta X is greater than the absolute value of delta Y then its horizontal
            if (absX > absY)
            {
                if (delta.X > 0)
                {
                    return FlickDirection.Right;
                }
                else
                {
                    return FlickDirection.Left;
                }
            }

            //if the absolute value of delta Y is greater than the absolute value of delta X then its vertical
            if (absX < absY)
            {
                if (delta.Y > 0)
                {
                    return FlickDirection.Down;
                }
                else
                {
                    return FlickDirection.Up;
                }
            }

            return FlickDirection.Unknown;
        }
    }
}
