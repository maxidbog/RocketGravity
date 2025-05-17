using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketGravity.Screens
{
    public class Player
    {
        private Texture2D texture;
        private Vector2 position;
        private float speed;
        private int screenWidth;
        private int screenHeight;

        public Vector2 Position => position;
        public Texture2D Texture => texture;

        public Player(Texture2D texture, Vector2 startPosition, float speed, int screenWidth, int screenHeight)
        {
            this.texture = texture;
            this.position = startPosition;
            this.speed = speed;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
        }

        public void Update(GameTime gameTime,ref GameState gameState)
        {
            var keyboardState = Keyboard.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyboardState.IsKeyDown(Keys.W))
                position.Y -= speed * deltaTime;
            if (keyboardState.IsKeyDown(Keys.S))
                position.Y += speed * deltaTime;
            if (keyboardState.IsKeyDown(Keys.A))
                position.X -= speed * deltaTime;
            if (keyboardState.IsKeyDown(Keys.D))
                position.X += speed * deltaTime;

            // в пределах экрана
            position.X = MathHelper.Clamp(position.X, 0, screenWidth - 90);
            position.Y = MathHelper.Clamp(position.Y, 0, screenHeight - 240);

            if (keyboardState.IsKeyDown(Keys.Escape))
                gameState = GameState.MainMenu;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,new Rectangle((int)position.X, (int)position.Y, 90, 240), Color.White);
        }
    }
}
