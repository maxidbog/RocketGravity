using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using RocketGravity.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;

namespace RocketGravity.Code
{
    public abstract class LevelBase
    {
        protected static List<LevelObject> levelObjects = new List<LevelObject>();
        protected Texture2D background;
        protected Color backgroundColor = Color.White;
        protected Texture2D rocketTexture;
        protected SpriteFont spriteFont;
        protected int Score;
        protected Texture2D DefaultTexure;

        public static Rocket Rocket { get; protected set; }
        public List<Island> Islands => levelObjects.Where(obj => obj is Island).Select(obj => obj as Island).ToList();
        public bool IsCompleted { get; protected set; }

        protected Island currentIsland;
        protected static Vector2 Centre = new Vector2(MainGame.screenWidth/2, MainGame.screenHeight/2);


        public abstract void Initialize();
        public abstract void LoadContent(ContentManager content);

        public void Update(GameTime gameTime, ref GameState gameState)
        {
            bool needToAdd = false;
            var currentTime = gameTime.TotalGameTime;
            var keyboardState = Keyboard.GetState();
            foreach (var levelObject in levelObjects)
            {
                if (levelObject.CheckCrushing(Rocket))
                {
                    Initialize();
                    break;
                }
                if (levelObject is Island island)
                {
                    if (!(island.IsLanded || island.IsVisited) && island.CheckLanding(Rocket))
                    {
                        Rocket.IsLanded = true;
                        island.IsVisited = true;
                        Rocket.SetPosition(new Vector2(island.Position.X + island.Collider.Width / 2, island.Collider.Y - Rocket.Height / 2));
                        var jumpLen = island.Number - currentIsland.Number;
                        currentIsland = island;
                        ScrollTo(currentIsland);
                        needToAdd = true;
                        var reward = jumpLen * 2 - 1;
                        Score += reward;
                        Rocket.AddFuel(25 + (reward - 1) * 15);
                    }
                    if (island.IsLanded)
                    {
                        if (keyboardState.IsKeyDown(Keys.Space))
                        {
                            island.IsLanded = false;
                            Rocket.IsLanded = false;
                            island.DetachTime = currentTime;
                            Rocket.Velocity = new Vector2(0, -150);
                        }
                    }
                    if (island.IsVisited && currentTime - island.DetachTime >= TimeSpan.FromSeconds(5))
                    {
                        island.IsVisited = false;
                    }
                }
            }

            if (needToAdd)
            {
                AddIsland();
                needToAdd = false;
            }

            if (Rocket.Position.X + 400 >= MainGame.screenWidth)
                Centre -= new Vector2(1 + (-MainGame.screenWidth + Rocket.Position.X + 400) / 20, 0);

            if (Rocket.Position.X <= 100)
                Centre += new Vector2(2, 0);

            Scroll();

            Rocket.Update(gameTime);

            //CheckLevelCompletion();
            if (keyboardState.IsKeyDown(Keys.Escape))
                gameState = GameState.MainMenu;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Rectangle(-500, 0, 3024, 1080), Color.White);
            foreach (var island in levelObjects)
            {
                island.Draw(spriteBatch);
            }

            Rocket?.Draw(spriteBatch);

            spriteBatch.DrawString(spriteFont, "Счёт: " + Score,
               new Vector2(50,30), Color.White);

            spriteBatch.Draw(DefaultTexure, new Rectangle(130, 110, 150, 50), new Color(0, 0, 0, 100));
            spriteBatch.Draw(DefaultTexure, new Rectangle(140, 120, (int)(Rocket.Fuel / Rocket.MaxFuel * 130), 30), Color.Orange);
            spriteBatch.DrawString(spriteFont, "Fuel:", new Vector2(30, 110), Color.Orange);
        }

        public static void ScrollTo(Island destinationIsland)
        {
            int scrollLen = (int)(destinationIsland.Position.X - 200);
            Centre = new Vector2(Centre.X - scrollLen, Centre.Y);
        }

        public static void Scroll()
        {
            var deltaX = (MainGame.screenWidth/2 - Centre.X) / 20;
            var deltaY = (MainGame.screenHeight / 2 - Centre.Y) / 40;

            Centre += new Vector2(deltaX, deltaY);
            foreach (var island in levelObjects)
            {
                island.SetPosition(new Vector2(island.Position.X - deltaX, island.Position.Y));
            }
            Rocket.SetPosition(new Vector2(Rocket.Position.X - deltaX, Rocket.Position.Y));

        }

        public abstract void AddIsland();

        //protected abstract void CheckLevelCompletion();
    }

    // 2. Конкретный класс уровня (Level1.cs)
    public class Level1 : LevelBase
    {
        private Texture2D islandTexture;
        private Texture2D obstacleTexture;
        private Random random = new Random();

        public override void Initialize()
        {
            IsCompleted = false;

            levelObjects.Clear();
            Score = 0;

            var random = new Random();

            // Создание островов
            levelObjects.Add(new Island(islandTexture, new Vector2(200, 800),0, 0.5f));
            Islands[0].IsLanded = true;
            Islands[0].IsVisited = true;
            currentIsland = Islands[0];

            while (levelObjects.Count < 8)
                AddIsland();

            Islands[0].IsLanded = true;
            Islands[0].IsVisited = true;
            currentIsland = Islands[0];


            // Инициализация игрока
            Rocket = new Rocket(rocketTexture, Vector2.Zero, 100, 10);
            Rocket.SetFuel(Rocket.MaxFuel);
            Rocket.SetPosition(new Vector2(levelObjects[0].Collider.X + levelObjects[0].Collider.Width / 2, levelObjects[0].Collider.Y - Rocket.Height / 2));
            Rocket.IsLanded = true;
        }

        public override void LoadContent(ContentManager content)
        {
            islandTexture = content.Load<Texture2D>("Island");
            obstacleTexture = content.Load<Texture2D>("Rock");
            rocketTexture = content.Load<Texture2D>("Rocket");
            spriteFont = content.Load<SpriteFont>("Font");
            background = MainGame.Background;
            DefaultTexure = MainGame.DefaultTexture;
        }

        public override void AddIsland()
        {
            var previous = levelObjects.LastOrDefault();
            float baseX = previous?.Position.X ?? 200;

            if (previous is Island && random.Next(100) < 30)
            {
                float newX = baseX + random.Next(400, 900);
                float newY = random.Next(400, 800);

                levelObjects.Add(new Obstacle(
               obstacleTexture,
               new Vector2(newX, newY),
               0.5f));
            }
            else
            {
                int number = Islands.LastOrDefault()?.Number + 1 ?? 0;
                float newX = baseX + random.Next(800, 1300);
                float newY = random.Next(400, 900);

                var island = new Island(
                    islandTexture,
                    new Vector2(newX, newY),
                    number,
                    0.5f);

                levelObjects.Add(island);
            }

            while (levelObjects.Count > 8 && levelObjects[0] != currentIsland)
            {
                levelObjects.RemoveAt(0);
            }


            //var previousIsle = Islands.LastOrDefault();
            //var newposition = new Vector2(previousIsle?.Collider.X + random.Next(800, 1300) ?? 200, random.Next(400, 900));
            //levelObjects.Add(new Island(islandTexture, newposition, previousIsle.Number + 1, 0.5f));
        }
    }

    // 3. Менеджер уровней
    public class LevelManager
    {
        private List<LevelBase> levels = new List<LevelBase>();
        private int currentLevelIndex = -1;

        public LevelBase CurrentLevel { get; private set; }

        public void AddLevel(LevelBase level)
        {
            levels.Add(level);
        }

        public void LoadNextLevel(ContentManager content)
        {
            if (currentLevelIndex + 1 < levels.Count)
            {
                currentLevelIndex++;
                CurrentLevel = levels[currentLevelIndex];
                CurrentLevel.LoadContent(content);
                CurrentLevel.Initialize();
            }
        }

        public void RestartCurrentLevel()
        {
            if (CurrentLevel != null)
            {
                CurrentLevel.Initialize();
            }
        }
    }
}
