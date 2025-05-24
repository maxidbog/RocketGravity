using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RocketGravity.Code
{
    public class BlackHole : LevelObject
    {
        private Texture2D texture;
        private float scale;
        private float number;

        public float Strength { get; private set; }
        private int radius = 50;

        public override Texture2D Texture => texture;
        public override float Number => number;
        public override Rectangle Collider => new Rectangle(
            (int)Position.X - radius,
            (int)Position.Y - radius,
            (int)(radius * 2 * scale),
            (int)(radius * 2 * scale));

        public BlackHole(Texture2D texture, Vector2 position, float number,  float scale, float strength)
        {
            this.number = number;
            this.texture = texture;
            Position = position;
            this.scale = scale;
            Strength = strength;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, new Rectangle((int)(Collider.X - Collider.Width), (int)(Collider.Y - Collider.Height), Collider.Width * 3, Collider.Height * 3), Color.Black);
            spriteBatch.Draw(texture, Collider, Color.Red);
        }

        public override bool CheckCrushing(Rocket rocket) => rocket.Collider.Intersects(Collider);

        public Vector2 CalculateGravityForce(Vector2 targetPosition)
        {
            Vector2 direction = (Position - targetPosition);
            float distance = direction.Length();
            direction.Normalize();

            if (distance > 500) return Vector2.Zero;

            distance /= 100;

            float force = Strength / (distance * distance) + Strength * 0.1f;
            return direction * force;
        }
    }
}