﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketGravity.Code
{
    public class Tutorial : LevelBase
    {
        private class TutorialStep
        {
            public string Text { get; }
            public Func<bool> IsCompleted { get; set; }
            public Vector2 Position { get; }
            public bool IsActive { get; set; }

            public TutorialStep(string text, Func<bool> completionCondition, Vector2 position)
            {
                Text = text;
                IsCompleted = completionCondition;
                Position = position;
                IsActive = false;
            }
        }

        private List<TutorialStep> tutorialSteps;
        private int currentStepIndex;
        private float messageTimer;
        private const float MessageDuration = 2f;
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (currentStepIndex >= 0 && currentStepIndex < tutorialSteps.Count)
            {
                var currentStep = tutorialSteps[currentStepIndex];

                    DrawStepMessage(spriteBatch, currentStep);
            }
        }

        public override void Update(GameTime gameTime, ref GameState gameState)
        {
            base.Update(gameTime, ref gameState);

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            messageTimer -= deltaTime;

            if (currentStepIndex >= 0 && currentStepIndex < tutorialSteps.Count)
            {
                var currentStep = tutorialSteps[currentStepIndex];
                if (currentStep.IsCompleted() && currentStep.IsActive)
                {
                    currentStep.IsActive = false;
                    currentStep.IsCompleted = () => true;
                    messageTimer = MessageDuration;
                }
                if (currentStep.IsCompleted() && messageTimer <= 0)
                {
                    ActivateNextStep();
                }
            }
        }
        public override void Initialize()
        {
            IsCompleted = false;

            currentStepIndex = -1;
            Score = 0;
            levelObjects = new List<LevelObject> 
            {
                new Island(islandTexture,
                        new Vector2(200, 800),
                        0,
                        1f),
                new Island(islandTexture,
                        new Vector2(800, 600),
                        1,
                        1f),
                new Obstacle(obstacleTexture,
                        new Vector2(1500, 300),
                        2,
                        1f)
                };

            Islands[0].IsLanded = true;
            currentIsland = Islands[0];

            while (levelObjects.Count < MaxObjects)
                AddRandIsland();
            
            InitializeRocket();
            InitializeTutorialSteps();
            ActivateNextStep();
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

        private void InitializeTutorialSteps()
        {
            tutorialSteps = new List<TutorialStep>
        {
            new TutorialStep("Нажми Space чтобы взлететь",
                () => !Rocket.IsLanded,
                new Vector2(MainGame.screenWidth/2 - 600, 100)),

            new TutorialStep("Удерживай W для подъёма и траты топлива",
                () => (Rocket.Fuel < (Rocket.MaxFuel * 0.95f)) && Keyboard.GetState().IsKeyDown(Keys.W),
                new Vector2(MainGame.screenWidth/2 - 600, 100)),

            new TutorialStep("Используй A/D для поворота ракеты",
                () => Keyboard.GetState().IsKeyDown(Keys.A) || Keyboard.GetState().IsKeyDown(Keys.D),
                new Vector2(MainGame.screenWidth/2 - 600, 100)),

            new TutorialStep("Приземлись на платформу с правильной ориентацией!",
                () => Rocket.IsLanded && Score  > 0,
                new Vector2(MainGame.screenWidth/2 - 600, 100)),

            new TutorialStep("избегай препятствий!",
                () => Score > 2,
                new Vector2(MainGame.screenWidth - 600, 100)),

            new TutorialStep("Набери как можно больше очков!\n" +
                            "                          \"Esc\" для выхода",
                () => false,
                new Vector2(MainGame.screenWidth - 850, 100))
        };
        }

        private void ActivateNextStep()
        {
            if (currentStepIndex < tutorialSteps.Count - 1)
            {
                if (currentStepIndex >= 0)
                    tutorialSteps[currentStepIndex].IsActive = false;
                currentStepIndex++;
                tutorialSteps[currentStepIndex].IsActive = true;
                messageTimer = MessageDuration;
            }
        }

        private void DrawStepMessage(SpriteBatch spriteBatch, TutorialStep step)
        {
            //float alpha = MathHelper.Clamp(messageTimer * 2f, 0f, 1f);
            Color textColor = step.IsCompleted() ? Color.LightGreen : Color.White;

            spriteBatch.DrawString(
                spriteFont,
                step.Text,
                step.Position,
                textColor,
                0f,
                Vector2.Zero,
                1.2f,
                SpriteEffects.None,
                0f);
        }
    }
}
