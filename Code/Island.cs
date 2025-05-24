using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RocketGravity.Code
{
    public class Island : LevelObject
    {
        //public Rectangle Collider { get; private set; }
        private Texture2D texture;
        public bool IsLanded { get; set; }
        public bool IsVisited { get; set; }

        public override Texture2D Texture => texture;

        private int width = 300;
        private int Height => (int)(texture.Height * width / texture.Width);

        public override Rectangle Collider => new Rectangle(
                (int)(Position.X),
                (int)(Position.Y + Height * scale * 0.2),
                (int)(width * scale),
                (int)(Height * scale * 0.8)
            );

        public TimeSpan DetachTime { get; set; }

        //private Rectangle landingArea;
        //private Vector2 position;
        private float scale;
        private float number;

        //public override Vector2 Position => position;
        public override float Number => number;

        private Rectangle landingArea => new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                (int)(width * scale),
                (int)(Height * scale * 0.2f)
            );

        public Island(Texture2D texture, Vector2 position, float number, float scale = 1.0f)
        {
            this.texture = texture;
            this.number = number;
            this.Position = position;
            this.scale = scale;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Color color = IsLanded ? Color.LightGreen : Color.White;
            spriteBatch.Draw(Texture, new Rectangle(
                (int)(Position.X),
                (int)(Position.Y),
                (int)(width * scale),
                (int)(Height * scale)), color);

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

        public override bool CheckCrushing(Rocket rocket) => rocket.Collider.Intersects(Collider);

        public bool AreEqual(Island island) => this.Equals(island);
        
        //public void SetPosition(Vector2 position)
        //{
        //    this.position = position;
        //}
    }
}
