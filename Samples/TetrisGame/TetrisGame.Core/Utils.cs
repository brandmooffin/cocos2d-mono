using System;
using System.Collections.Generic;

namespace TetrisGame.Core
{
    class Utils
    {
        public static int GetRandomInt(int num)
        {
            return (int)Math.Floor(new Random().NextDouble() * num);
        }

        public static List<List<List<int>>> GetRandomItem(List<List<List<List<int>>>> arr)
        {
            return arr[GetRandomInt(arr.Count)];
        }
    }
}
