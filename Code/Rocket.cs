// Класс Rocket.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RocketGravity;
using System;
using static System.Math;

public class Rocket
{
    private Texture2D texture;
    private Vector2 position;
    public Vector2 Velocity { get; set; }
    private float rotation;
    private float thrust;
    private float fuel;
    private float maxFuel;
    private float fuelSpeed;

    public float Speed = 100f;
    public float RotationSpeed = 2.5f;
    public int Height = 203;
    public int Width = 90;
    private float Size => Width / (float)texture.Width;
    public Rectangle Collider => new Rectangle(
        (int)(Position.X - (Width * Abs(Cos(rotation)) + Height * Abs(Sin(rotation))) / 2),
        (int)(Position.Y - (Height * Abs(Cos(rotation)) + Width * Abs(Sin(rotation))) / 2),
        (int)(Width * Abs(Cos(rotation)) + Height * Abs(Sin(rotation))),
        (int)(Height * Abs(Cos(rotation)) + Width * Abs(Sin(rotation)))
    );

    public bool IsLanded;

    public Vector2 Position => position;
    public float Rotation => rotation;
    public float Fuel => fuel;
    public float MaxFuel => maxFuel;

    public Rocket(Texture2D texture, Vector2 startPosition, float maxFuel = 100f, float fuelSpeed = 10f)
    {
        this.texture = texture;
        position = startPosition;
        rotation = 0f;
        Velocity = Vector2.Zero;
        this.maxFuel = maxFuel;
        this.fuelSpeed = fuelSpeed;
    }

    public void SetPosition (Vector2 position)
    {
        this.position = position;
    }

    public void SetFuel (float fuel)
    { 
        this.fuel = fuel; 
    }

    public void AddFuel(float fuel)
    {
        this.fuel += fuel;
        if (this.fuel > maxFuel)
            this.fuel = maxFuel;
    }

    public void Update(GameTime gameTime, Vector2 gravity)
    {
        var keyboardState = Keyboard.GetState();
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (!IsLanded)
        {
            if (keyboardState.IsKeyDown(Keys.A))
            {
                rotation -= RotationSpeed * deltaTime;
                if (rotation <= 2 * -Math.PI)
                    rotation -= (float)(2 * Math.PI);
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                rotation += RotationSpeed * deltaTime;
                if (rotation >= 2 * Math.PI)
                    rotation -= (float)(2 * Math.PI);
            }

            if (keyboardState.IsKeyDown(Keys.W) && fuel > 0)
            {
                thrust = 1f;
                fuel -= fuelSpeed * deltaTime;
            }
            else
                thrust = 0f;



            Vector2 direction = new Vector2(
                (float)Math.Cos(rotation - MathHelper.PiOver2),
                (float)Math.Sin(rotation - MathHelper.PiOver2));

            Velocity += (direction * Speed * thrust + gravity) * deltaTime;

            Velocity *= 1f - (0.1f * deltaTime);

            position += Velocity * deltaTime;
        }
        else
        {
            rotation = 0f;
            Velocity = new Vector2(0,0);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
        spriteBatch.Draw(
            texture,
            position,
            null,
            Color.White,
            rotation,
            origin,
            Size,
            SpriteEffects.None,
            0f);

        //spriteBatch.Draw(texture, Collider, Color.Red * 0.5f);
    }
}