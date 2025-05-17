using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketGravity.Screens
{
    static class AboutScreen
    {
        public static SpriteFont Font = MainGame.MainFont;

        public static void Initialize(GraphicsDeviceManager _graphics)
        {
           
        }

        static public void Draw(SpriteBatch _spriteBatch)
        {
            // Отрисовка экрана "Об игре"
            string aboutText = "Это игра на MonoGame!\nРазработано Maxidbog\nESC - назад";
            Vector2 textPosition = new Vector2(
                100,
                100);
            _spriteBatch.DrawString(Font, aboutText, textPosition, Color.Black);
        }

        static public void Update(ref GameState currentState, MouseState mouseState)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                currentState = GameState.MainMenu;
            }
        }
    }
}
