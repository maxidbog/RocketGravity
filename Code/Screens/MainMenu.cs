using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RocketGravity.Screens;

namespace RocketGravity.Code.Screens
{
    static class MainMenu
    {
        private static Texture2D Background;
        private static SpriteFont Font;
        private static Texture2D Logo;
        //static int timeCounter = 0;
        //static Color color = Color.White;

        private static Texture2D gradient = MainGame.Gradient;

        public static Texture2D buttonTexture { get; set; }

        private static Rectangle playButton;
        private static Rectangle aboutButton;
        private static Rectangle logo;
        private static Rectangle exitButton;

        private static Color normalColor = Color.White;
        private static Color hoverColor = new Color(229, 148, 69, 230);
        private static Color transparent = new Color(0, 0, 0, 0);

        public static void Initialize()
        {
            logo = new Rectangle(
               MainGame.screenWidth - 1000,
               20,
               800, 600);

            playButton = new Rectangle(
               0,
               200,
               600, 100);

            aboutButton = new Rectangle(
                playButton.X,
                playButton.Y + playButton.Height / 2 + 50,
                600, 100);

            exitButton = new Rectangle(
               0,
               aboutButton.Y + aboutButton.Height / 2 + 150,
               600, 100);


        }

        static public void Draw(SpriteBatch _spriteBatch)
        {
            //_spriteBatch.Draw(Background, new Rectangle(0,0,2016,720), normalColor);

            _spriteBatch.Draw(Background, new Rectangle(0, 0, 3024, 1080), Color.White);

            _spriteBatch.Draw(Logo, logo, normalColor);

            _spriteBatch.Draw(MainGame.Gradient, new Rectangle(0, 0, 600, 1080), normalColor);

            _spriteBatch.DrawString(Font, "Версия 0.0.1",
                new Vector2(MainGame.screenWidth - 400, MainGame.screenHeight - 60), Color.White);


            // кнопка Play
            Color playColor = playButton.Contains(Mouse.GetState().Position) ? hoverColor : transparent;
            _spriteBatch.Draw(buttonTexture, playButton, playColor);
            _spriteBatch.DrawString(Font, "Играть",
                new Vector2(playButton.X + 50, playButton.Y + 30), Color.White);

            // кнопка About
            Color aboutColor = aboutButton.Contains(Mouse.GetState().Position) ? hoverColor : transparent;
            _spriteBatch.Draw(buttonTexture, aboutButton, aboutColor);
            _spriteBatch.DrawString(Font, "Об игре",
                new Vector2(aboutButton.X + 50, aboutButton.Y + 30), Color.White);
            // кнопка About
            Color exitColor = exitButton.Contains(Mouse.GetState().Position) ? hoverColor : transparent;
            _spriteBatch.Draw(buttonTexture, exitButton, exitColor);
            _spriteBatch.DrawString(Font, "Выход",
                new Vector2(exitButton.X + 50, exitButton.Y + 30), Color.White);
        }

        public static void LoadContent(ContentManager content)
        {
            Font = MainGame.MainFont;
            buttonTexture = MainGame.DefaultTexture;
            buttonTexture.SetData(new[] { Color.White });
            Logo = content.Load<Texture2D>("Logo");
            Background = MainGame.Background;
        }

        static public void Update(MouseState mouseState)
        {
            if (playButton.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed)
            {
                SelectLevel.Initialize();
                MainGame.ChangeState(GameState.SelectLevel);
            }

            if (aboutButton.Contains(mouseState.Position))
            {
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    MainGame.ChangeState(GameState.About);
                }
            }

            if (exitButton.Contains(mouseState.Position))
            {
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    MainGame.ChangeState(GameState.Exit);
                }
            }
        }

    }
}
