using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketGravity.Code
{
    public static class Input
    {
        private static KeyboardState _previousKeyboardState;
        private static KeyboardState _currentKeyboardState;

        public static void Update()
        {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();
        }
        /// <summary>
        /// Один раз возвращает True если клавиша была нажата.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsSingleKeyPress(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key) &&
                   _previousKeyboardState.IsKeyUp(key);
        }
    }
}
