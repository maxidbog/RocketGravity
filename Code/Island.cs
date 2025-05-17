using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RocketGravity.Code
{
    public class Island
    {
        //public Rectangle Collider { get; private set; }
        public Texture2D Texture { get; private set; }
        public bool IsLanded { get; set; }
        public bool IsVisited { get; set; }

        public Rectangle Collider => new Rectangle(
                (int)(position.X),
                (int)(position.Y + Texture.Height * scale * 0.2),
                (int)(Texture.Width * scale),
                (int)(Texture.Height * scale * 0.8)
            );

        public TimeSpan DetachTime { get; set; }

        //private Rectangle landingArea;
        private Vector2 position;
        private float scale;
        private int number;

        public Vector2 Position => position;
        public int Number => number;

        private Rectangle landingArea => new Rectangle(
                (int)position.X,
                (int)position.Y,
                (int)(Texture.Width * scale),
                (int)(Texture.Height * scale * 0.2f)
            );

        public Island(Texture2D texture, Vector2 position, int number, float scale = 1.0f)
        {
            Texture = texture;
            this.number = number;
            this.position = position;
            this.scale = scale;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color color = IsLanded ? Color.LightGreen : Color.White;
            spriteBatch.Draw(Texture, new Rectangle(
                (int)(position.X),
                (int)(position.Y),
                (int)(Texture.Width * scale),
                (int)(Texture.Height * scale)), color);

            // Для отладки: отображение зоны приземления
            //spriteBatch.Draw(Texture, landingArea, Color.Green * 0.8f);
            //spriteBatch.Draw(Texture, Collider, Color.Red * 0.8f);


        }

        public bool CheckLanding(Rocket rocket)
        {
            // Проверка пересечения с зоной приземления
            bool inLandingZone = rocket.Collider.Intersects(landingArea);

            // Проверка скорости (максимальная вертикальная и горизонтальная скорость)
            bool speedValid = rocket.Velocity.Length() < 100f;

            // Проверка правильной ориентации (ракета должна быть сверху)
            float rotationAngle = (float)Math.Atan2(rocket.Velocity.Y, rocket.Velocity.X) * (180f / (float)Math.PI);
            bool positionValid = Math.Abs(rocket.Rotation) <= 0.5f;

            IsLanded = inLandingZone && speedValid && positionValid;
            return IsLanded;
        }

        public bool CheckCrushing(Rocket rocket) => rocket.Collider.Intersects(Collider);

        public bool AreEqual(Island island) => this.Equals(island);
        
        public void SetPosition(Vector2 position)
        {
            this.position = position;
        }
    }
}
