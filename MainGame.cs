using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RocketGravity.Code;
using RocketGravity.Code.Screens;
using RocketGravity.Screens;
//using System.Numerics;

namespace RocketGravity;

public enum GameState { Exit, MainMenu, About, SelectLevel, Playing, Level}


public class MainGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public static SpriteFont MainFont;
    public static GameState currentState = GameState.MainMenu;
    public static Texture2D rocketTexture;
    public static Texture2D Background;
    public static Texture2D Gradient;
    public static Texture2D DefaultTexture;


    public static int screenWidth;
    public static int screenHeight;

    public static LevelManager LevelManager;

    public MainGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _graphics.PreferredBackBufferWidth = screenWidth = 1920;
        _graphics.PreferredBackBufferHeight = screenHeight = 1080;
        _graphics.IsFullScreen = false;
        Window.IsBorderless = true;
        _graphics.ApplyChanges();

        LevelManager = new LevelManager();
        LevelManager.AddLevel(new Tutorial());
        LevelManager.AddLevel(new Level1());
        LevelManager.AddLevel(new Level2());

        MainMenu.Initialize();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Background = Content.Load<Texture2D>("backgroundMars");
        MainFont = Content.Load<SpriteFont>("Font");
        Gradient = Content.Load<Texture2D>("Gradient");
        DefaultTexture = new Texture2D(GraphicsDevice, 1, 1);

        MainMenu.LoadContent(Content);
        SelectLevel.LoadContent(Content);

        //LevelManager.LoadNextLevel(Content);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape) && currentState == GameState.MainMenu)
        //    Exit();

        Input.Update();
        MouseState mouseState = Mouse.GetState();

        switch(currentState)
        {
            case GameState.Exit:
                Exit();
                break;
            case GameState.MainMenu:
                MainMenu.Update(mouseState);
                break;
            case GameState.About:
                AboutScreen.Update(ref currentState, mouseState);
                break;
            case GameState.SelectLevel:
                SelectLevel.Update(mouseState, Content);
                break;
            case GameState.Level:
                LevelManager.CurrentLevel.Update(gameTime,ref currentState);
                break;
        }

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Cyan);

        _spriteBatch.Begin();

        switch (currentState)
        {
            case GameState.MainMenu:
                MainMenu.Draw(_spriteBatch);
                break;
            case GameState.About:
                AboutScreen.Draw(_spriteBatch);
                break;
            case GameState.SelectLevel:
                SelectLevel.Draw(_spriteBatch);
                break;
            case GameState.Level:
                LevelManager.CurrentLevel.Draw(_spriteBatch);
                break;

        }


        _spriteBatch.End();

        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }

    public static void ChangeState(GameState state)
    {
        currentState = state;         
    }
}
