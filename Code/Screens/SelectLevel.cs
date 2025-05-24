using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using RocketGravity.Code;

namespace RocketGravity.Screens
{
    public class SelectLevel
    {
        private static SpriteFont Font;
        private static Texture2D Background;
        private static Texture2D Texture;
        private static Texture2D TileTexture;

        private static Color NormalColor = Color.White;
        private static Color HoverColor = Color.Orange;

        private static Rectangle TutorialTile;
        private static Rectangle Level1;
        private static Rectangle Level2;
        private static Rectangle Level3;

        private static Texture2D TutorialImage;
        private static Texture2D Level1Image;
        private static Texture2D Level2Image;
        private static Texture2D Level3Image;

        public static void Initialize()
        {
            Font = MainGame.MainFont;
            Background = MainGame.Background;
            Texture = MainGame.DefaultTexture;

            TutorialTile = new Rectangle(
                100, 300,
                400, 400
                );

            Level1 = new Rectangle(
                TutorialTile.X + TutorialTile.Width + 50, 300,
                400, 400
                );

            Level2 = new Rectangle(
                Level1.X + Level1.Width + 50, 300,
                400, 400
                );

            Level3 = new Rectangle(
                Level2.X + Level2.Width + 50, 300,
                400, 400
                );
        }
        public static void LoadContent(ContentManager content)
        {
            TileTexture = content.Load<Texture2D>("Tile");
            Level1Image = content.Load<Texture2D>("MarsImage");
            TutorialImage = Level1Image;
            Level2Image = content.Load<Texture2D>("EarthImage");
            Level3Image = content.Load<Texture2D>("JupiterImage");
        }

        static public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(Background, new Rectangle(0, 0, 3024, 1080), new Color(150,150,150));

            //_spriteBatch.Draw(TileTexture, new Rectangle(200, 300, 500, 400), new Color(255, 255, 255, 200));

            //Первый уровень
            DrawTile(_spriteBatch, TutorialTile, TutorialImage, "Обучение", 1);

            DrawTile(_spriteBatch, Level1, Level1Image, "Марс", 1);

            DrawTile(_spriteBatch, Level2, Level2Image, "Земля", 3);

            DrawTile(_spriteBatch, Level3, Level3Image, "Венера (Скоро)", 5);

        }

        static public void Update(MouseState mouseState, ContentManager content)
        {
            if (TutorialTile.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed)
            {
                MainGame.LevelManager.LoadLevel(content, 0);
                MainGame.ChangeState(GameState.Level);
            }

            if (Level1.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed)
            {
                MainGame.LevelManager.LoadLevel(content, 1);
                MainGame.ChangeState(GameState.Level);
            }

            if (Level2.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed)
            {
                MainGame.LevelManager.LoadLevel(content, 2);
                MainGame.ChangeState(GameState.Level);
            }

            if (Level3.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed)
            {
                MainGame.LevelManager.LoadLevel(content, 3);
                MainGame.ChangeState(GameState.Level);
            }

            if (Input.IsSingleKeyPress(Keys.Escape))
            {
                MainGame.ChangeState(GameState.MainMenu);
            }
        }

        private static void DrawTile(SpriteBatch _spriteBatch, Rectangle rectangle, Texture2D? image, string name, int difficulty)
        {
            Color Color = rectangle.Contains(Mouse.GetState().Position) ? HoverColor : NormalColor;
            Rectangle newRectangle = rectangle.Contains(Mouse.GetState().Position) ? new Rectangle(rectangle.X - 10 , rectangle.Y - 10, rectangle.Width + 20, rectangle.Height + 20) : rectangle;
            _spriteBatch.Draw(TileTexture, newRectangle, Color);
            _spriteBatch.Draw(image ?? Texture, new Rectangle(newRectangle.X + 10, newRectangle.Y + 100, newRectangle.Width - 20, newRectangle.Height - 200), image is null ? Color.Gray : Color.White);
            var difficultyColor = Color.ForestGreen;
            if (difficulty >= 3 && difficulty < 5)
                difficultyColor = Color.YellowGreen;
            else if (difficulty >= 5)
                difficultyColor = Color.OrangeRed;
            for(int i = 0; i < 5; i++)
            {
                var currentColor = i <= difficulty - 1 ? difficultyColor : Color.Gray;
                _spriteBatch.Draw(Texture, new Rectangle(newRectangle.X + newRectangle.Width / 2 - 45 + i * 20, newRectangle.Y + newRectangle.Height - 50, 10, 20), currentColor * Color);
            }
            _spriteBatch.DrawString(Font, name,
                 new Vector2(newRectangle.X + 20, newRectangle.Y + newRectangle.Height + 5), Color);
        }
    }
}
