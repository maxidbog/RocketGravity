using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RocketGravity.Code
{
    public class Obstacle : LevelObject
    {
        private Texture2D texture;
        private float scale;
        private float number;

        private int width = 200;
        private int height => (int)(texture.Height * width / texture.Width);

        public override Texture2D Texture => texture;
        public override float Number => number;
        public override Rectangle Collider =>  new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            (int)(width * scale),
            (int)(height * scale));

        public Obstacle(Texture2D texture, Vector2 position, float number, float scale)
        {
            this.number = number;
            this.texture = texture;
            this.Position = position;
            this.scale = scale;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Collider, Color.White);
        }

        public override bool CheckCrushing(Rocket rocket) => rocket.Collider.Intersects(Collider);
    }
}
