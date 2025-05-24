using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketGravity.Code
{
    public abstract class LevelObject
    {
        public abstract Texture2D Texture { get; }
        public abstract Rectangle Collider { get; }
        public Vector2 Position { get; protected set; }
        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract bool CheckCrushing(Rocket rocket);
        public virtual void Update(GameTime gameTime) { }

        public abstract float Number { get; }

        public void SetPosition(Vector2 position)
        {
            this.Position = position;
        }
    }
}
