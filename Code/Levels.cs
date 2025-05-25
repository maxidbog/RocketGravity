using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace RocketGravity.Code
{
    public abstract class LevelBase
    {
        protected static List<LevelObject> levelObjects = new List<LevelObject>();
        protected Texture2D background;
        protected Color backgroundColor = Color.White;
        protected Texture2D rocketTexture;
        protected SpriteFont spriteFont;
        protected int Difficulty;
        protected int Score;
        protected Texture2D DefaultTexure;
        protected Texture2D islandTexture;
        protected Texture2D obstacleTexture;
        protected Texture2D blackHoleTexture;
        protected int MaxObjects = 12;
        protected Vector2 gravity = (new Vector2(0, 1)) * 30f;

        public static Rocket Rocket { get; protected set; }
        public List<Island> Islands => levelObjects.Where(obj => obj is Island).Select(obj => obj as Island).ToList();
        public bool IsCompleted { get; protected set; }

        protected Island currentIsland;
        protected static Vector2 Centre = new Vector2(MainGame.screenWidth/2, MainGame.screenHeight/2);

        // Configuration
        private const int obstacleChance = 30;


        public virtual void Initialize()
        {
            IsCompleted = false;
            Score = 0;
            levelObjects = new List<LevelObject>
            {
                new Island(islandTexture,
                        new Vector2(200, 800),
                        0,
                        1f)
            };
            Islands[0].IsLanded = true;
            Islands[0].IsVisited = true;
            currentIsland = Islands[0];
            InitializeRocket();
            while (levelObjects.Count < MaxObjects)
                AddRandIsland();
        }
        public abstract void LoadContent(ContentManager content);

        public virtual void Update(GameTime gameTime, ref GameState gameState)
        {
            bool needToAdd = false;
            var currentTime = gameTime.TotalGameTime;
            var keyboardState = Keyboard.GetState();
            var currentGravity = gravity;
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
                        LandingUpdate(island);
                        needToAdd = true;
                    }
                    if (island.IsLanded && keyboardState.IsKeyDown(Keys.Space))
                    {
                        DetachRocket(currentTime, island);
                    }
                    if (island.IsVisited && currentTime - island.DetachTime >= TimeSpan.FromSeconds(5))
                    {
                        island.IsVisited = false;
                    }
                }
                if (levelObject is BlackHole hole)
                {
                    currentGravity += hole.CalculateGravityForce(Rocket.Position);
                }
            }

            if (needToAdd)
            {
                AddRandIsland();
            }

            if (Rocket.Position.X + 600 >= MainGame.screenWidth)
                Centre -= new Vector2(1 + (-MainGame.screenWidth + Rocket.Position.X + 600) / 20, 0);

            if (Rocket.Position.X <= 100)
                Centre += new Vector2(2, 0);

            if (Rocket.Position.Y >= MainGame.screenHeight)
                Initialize();

            Scroll();
            Rocket.Update(gameTime, currentGravity);

            if (Input.IsSingleKeyPress(Keys.Escape))
                gameState = GameState.SelectLevel;
        }

        private static void DetachRocket(TimeSpan currentTime, Island island)
        {
            island.IsLanded = false;
            Rocket.IsLanded = false;
            island.DetachTime = currentTime;
            Rocket.Velocity = new Vector2(0, -150);
        }

        private void LandingUpdate(Island island)
        {
            Rocket.IsLanded = true;
            island.IsVisited = true;
            Rocket.SetPosition(new Vector2(island.Position.X + island.Collider.Width / 2, island.Collider.Y - Rocket.Height / 2));
            var jumpLen = island.Number - currentIsland.Number;
            currentIsland = island;
            ScrollTo(currentIsland);
            var reward = (int)(jumpLen * 2) - 1;
            Score += reward < 0 ? 0 : reward;
            Rocket.AddFuel((25 + (reward - 1) * 15) * (float)Math.Pow(1.04, Difficulty));
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

        public void AddRandIsland()
        {
            var random = new Random();
            var previous = levelObjects.LastOrDefault();

            float baseX = previous.Position.X + Difficulty * 30;
            float number = previous.Number + 1;
            int obstacleRandom = random.Next(100);
            var difficultyMultiplyer = 1 + Difficulty / 10 ;

            if (((previous is not Obstacle && previous is not BlackHole) && obstacleRandom < obstacleChance + Difficulty * 5) || obstacleRandom < 10 + Difficulty * 2)
            {
                AddNewObstacle(random, baseX, number, difficultyMultiplyer);
            }
            else
            {
                AddNewIsland(random, baseX, number, difficultyMultiplyer);
            }

            while (levelObjects.Count > MaxObjects && levelObjects[0] != currentIsland)
            {
                levelObjects.RemoveAt(0);
            }
        }

        private void AddNewObstacle(Random random, float baseX, float number, int difficultyMultiplyer)
        {
            float newX = baseX + random.Next(400, 900) * difficultyMultiplyer;
            float newY = random.Next(200, 700);
            if (random.Next(100) < 40 && Difficulty > 3)
            {
                levelObjects.Add(new BlackHole(
                       blackHoleTexture,
                       new Vector2(newX, newY),
                       number,
                       1f,
                       200f));
            }
            else
            {
                number -= 0.5f;

                levelObjects.Add(new Obstacle(
                       obstacleTexture,
                       new Vector2(newX, newY),
                       number,
                       1f));
            }
        }

        private void AddNewIsland(Random random, float baseX, float number, int difficultyMultiplyer)
        {
            float newX = baseX + random.Next(800, 1300) * difficultyMultiplyer;
            float newY = random.Next(400, 800);

            var island = new Island(
                islandTexture,
                new Vector2(newX, newY),
                number,
                1f);

            levelObjects.Add(island);
        }

        public void InitializeRocket()
        {
            Rocket = new Rocket(rocketTexture, Vector2.Zero, 100, 10);
            Rocket.SetFuel(Rocket.MaxFuel);
            Rocket.SetPosition(new Vector2(Islands[0].Collider.X + Islands[0].Collider.Width / 2, Islands[0].Collider.Y - Rocket.Height / 2));
            Rocket.IsLanded = true;
        }

    }
    public class Level1 : LevelBase
    {
        public override void Initialize()
        {
            Difficulty = 1;
            base.Initialize();
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
    }

    public class Level2 : LevelBase
    {

        public override void Initialize()
        {
            Difficulty = 3;
            gravity = (new Vector2(0, 1)) * 40f;
            base.Initialize();
        }

        public override void LoadContent(ContentManager content)
        {
            islandTexture = content.Load<Texture2D>("EarthIsland");
            obstacleTexture = content.Load<Texture2D>("cliff");
            rocketTexture = content.Load<Texture2D>("Rocket");
            spriteFont = content.Load<SpriteFont>("Font");
            background = content.Load<Texture2D>("backgroundEarth");
            blackHoleTexture = content.Load<Texture2D>("blackHole");
            DefaultTexure = MainGame.DefaultTexture;
        }
    }

    public class Level3 : LevelBase
    {

        public override void Initialize()
        {
            Difficulty = 5;
            gravity = (new Vector2(0, 1)) * 50f;
            base.Initialize();
        }

        public override void LoadContent(ContentManager content)
        {
            islandTexture = content.Load<Texture2D>("JupiterIsland");
            obstacleTexture = content.Load<Texture2D>("Rock");
            rocketTexture = content.Load<Texture2D>("Rocket");
            spriteFont = content.Load<SpriteFont>("Font");
            background = content.Load<Texture2D>("backgroundJupiter");
            blackHoleTexture = content.Load<Texture2D>("blackHole");
            DefaultTexure = MainGame.DefaultTexture;
        }
    }

    // Менеджер уровней
    public class LevelManager
    {
        private List<LevelBase> levels = new List<LevelBase>();
        private int currentLevelIndex = -1;

        public LevelBase CurrentLevel { get; private set; }

        public void AddLevel(LevelBase level)
        {
            levels.Add(level);
        }

        public void LoadLevel(ContentManager content, int index)
        {
            currentLevelIndex = index;
            CurrentLevel = levels[index];
            CurrentLevel.LoadContent(content);
            CurrentLevel.Initialize();
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
