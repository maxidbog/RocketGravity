using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketGravity.Code
{
    public class Obstacle : LevelObject
    {
        private Texture2D texture;
        private float scale;

        public override Texture2D Texture => texture;
        public override Rectangle Collider =>  new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            (int)(texture.Width * scale),
            (int)(texture.Height * scale));

        public Obstacle(Texture2D texture, Vector2 position, float scale)
        {
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
