﻿using DataBros.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;

namespace DataBros.States
{
    public class GameState : State
    {
        #region Fields
        private Texture2D backgroundTexture;
        private Rectangle backgroundRectangle;

        private List<Component> components;
        private List<Component> addComponents;
        private List<Component> delComponents;

        //Upgrade menu
        private bool playGame = false;
        private bool pickWater = true;

        private Texture2D upgradeMenuTexture;
        private Rectangle upgradeMenuRectangle;

        private Texture2D upgrade1;
        private Texture2D upgrade2;
        private Texture2D player1;
        private Texture2D player2;
        private Texture2D p1Aim;
        private Texture2D p2Aim;
        private Vector2 p1origin;
        private Vector2 p2origin;
        private Vector2 p1position = new Vector2(700, 900);
        private Vector2 p2position = new Vector2(100, 900);
        private Vector2 p1AimPosition = new Vector2(700, 500);
        private Vector2 p2AimPosition = new Vector2(100, 500);
        private Rectangle upgRectangle;
        private Rectangle upg2Rectangle;
        Texture2D buttonTexture;
        SpriteFont buttonFont;
        Button pickWaterButton;
        Button streamButton;
        Button lakeButton;
        Button oceanButton;
        Button FishButton;
        public Water stream;

        private int pullCount = 0;

        public Water currentWater;
        System.Timers.Timer aTimer;
        bool alreadyFishing = false;

        private KeyboardState oldState;
        private KeyboardState newState;

        #endregion

        #region Methods

        #region Constructor
        public GameState(GameWorld game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {

            GameWorld.repo.Open();
            stream = GameWorld.repo.FindWater("Stream");
            currentWater = stream;
            GameWorld.repo.Close();
            GameWorld.visualManager.cellCount = currentWater.Size;
            GameWorld.visualManager.CreateGrid();

            LoadContent();
            //Button
            
            pickWaterButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(5, 10),
                Text = "Pick Water",
            };

            pickWaterButton.Click += PickWaterButton_Click;

            var baitButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(5, 65),
                Text = "Bait",
            };

            baitButton.Click += BaitButton_Click;

            lakeButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(300, 100),
                Text = "Lake",
            };


            oceanButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(300, 200),
                Text = "Ocean",
            };


            streamButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(300, 300),
                Text = "Stream",
            };
            FishButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(300, 400),
                Text = "Fish",
            };
            

            components = new List<Component>()
            {
                pickWaterButton,
                baitButton,
            };

            //Background
            backgroundTexture = _content.Load<Texture2D>("background1");
            backgroundRectangle = new Rectangle(0, 0, backgroundTexture.Width, backgroundTexture.Height);

            //Upgrade menu
            upgradeMenuTexture = _content.Load<Texture2D>("upgmenu");
            upgradeMenuRectangle = new Rectangle(430, 90, upgradeMenuTexture.Width, upgradeMenuTexture.Height);

            upgrade1 = _content.Load<Texture2D>("upgrade1");
            upgrade2 = _content.Load<Texture2D>("upgrade2");
            upgRectangle = new Rectangle(495, 140, upgrade1.Width, upgrade1.Height);
            upg2Rectangle = new Rectangle(495, 195, upgrade2.Width, upgrade2.Height);

        }


        #endregion
        public void LoadContent()
        {
            //player1 = _content.Load<Texture2D>("pl1");
            //p1origin = new Vector2(500, 300);
            //player2 = _content.Load<Texture2D>("pl2");
            //p2origin = new Vector2(300, 300);

            //p1Aim = _content.Load<Texture2D>("p1aimsprite");
            //p2Aim = _content.Load<Texture2D>("p2aimsprite");
            GameWorld.Instance.player1.Loadcontent();
            GameWorld.Instance.player2.Loadcontent();


            buttonTexture = _content.Load<Texture2D>("button");
            buttonFont = _content.Load<SpriteFont>("Fonts/font");

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, backgroundRectangle, Color.White);

            GameWorld.visualManager.Draw(spriteBatch);

            //spriteBatch.Draw(player1, new Vector2(200, 100), new Rectangle(100,100,100,100), Color.White, 0f, p1origin, 1, SpriteEffects.None, 0);
            //spriteBatch.Draw(player2, new Vector2(500, 200), Rectangle.Empty, Color.White, 0f, p2origin, Vector2.Zero, SpriteEffects.None, 1);

            //spriteBatch.Draw(player1, p1position, Color.White);
            //spriteBatch.Draw(player2, p2position, Color.White);
            //spriteBatch.Draw(p1Aim, p1AimPosition, Color.White);
            //spriteBatch.Draw(p2Aim, p2AimPosition, Color.White);
            //spriteBatch.DrawString(buttonFont, $"ammount of pulls left: {pullCount}", new Vector2(10, 300), Color.Green);

            GameWorld.Instance.player1.Draw(spriteBatch);
            GameWorld.Instance.player2.Draw(spriteBatch);

            //Button
            foreach (var component in components)
            {
                component.Draw(gameTime, spriteBatch);
            }
            if (playGame)
            {
                spriteBatch.Draw(upgradeMenuTexture, upgradeMenuRectangle, Color.White);
                spriteBatch.Draw(upgrade1, upgRectangle, Color.White);
                spriteBatch.Draw(upgrade2, upg2Rectangle, Color.White);
                

                if (Keyboard.GetState().IsKeyDown(Keys.U))
                {
                    //Upgrade fishing pole or bait
                }
            }

            spriteBatch.End();
        }


        private void PickWaterButton_Click(object sender, EventArgs e)
        {
            if (pickWater == true)
            {
                lakeButton.Click += LakeButton_Click;
                oceanButton.Click += OceanButton_Click;
                streamButton.Click += StreamButton_Click;
                FishButton.Click += FishButton_Click;

                addComponents = new List<Component>()
            {
                    FishButton,
                streamButton,
                lakeButton,
                oceanButton,
            };
            }
            pickWater = false;

        }

        private void FishButton_Click(object sender, EventArgs e)
        {
            if (alreadyFishing == false)
            {

                aTimer = new System.Timers.Timer();
                aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                //aTimer.Interval = 5000 ;
                aTimer.Interval = 3000 * GameWorld.currentBait.BiteTime;
                 aTimer.Enabled = true;
                alreadyFishing = true;
            }
        }

        public void FishingKey()
        {
            if (alreadyFishing == false)
            {
                Random rnd = new Random();
                pullCount = rnd.Next(10, 20);

                aTimer = new System.Timers.Timer();
                aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                //aTimer.Interval = 5000 ;
                aTimer.Interval = 3000 * GameWorld.currentBait.BiteTime;
                aTimer.Enabled = true;
                alreadyFishing = true;
            }
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            Debug.WriteLine("You have Caught a fish!");

            GameWorld.repo.Open();
            var catchAble = GameWorld.repo.FindAFish(currentWater.Id);
            int max = catchAble.Count;
            Random Rnd = new Random();
            int randomNumber = Rnd.Next(0, max);

            var caught = catchAble[randomNumber];

            Debug.WriteLine($"Id {caught.Id} Name {caught.Name} Price {caught.Price} ");
            Debug.WriteLine($"Time spent {aTimer.Interval} ");


            GameWorld.repo.Close();
            //ResetWaterButtons();
            aTimer.Elapsed -= new ElapsedEventHandler(OnTimedEvent);
            aTimer.Enabled = false;
            alreadyFishing = false;

        }

        public void ResetWaterButtons()
        {
            delComponents = new List<Component>()
            {
                FishButton,
                streamButton,
                lakeButton,
                  oceanButton,
            };
            lakeButton.Click -= LakeButton_Click;
            oceanButton.Click -= OceanButton_Click;
            streamButton.Click -= StreamButton_Click;
            FishButton.Click -= FishButton_Click;


            pickWater = true;

            GameWorld.visualManager.cellCount = currentWater.Size;
            GameWorld.visualManager.CreateGrid();

        }
        private void StreamButton_Click(object sender, EventArgs e)
        {
            
            if (currentWater.Name != "Stream")
            {
                GameWorld.repo.Open();
                stream = GameWorld.repo.FindWater("Stream");
                currentWater = stream;
                Debug.WriteLine($"Id {stream.Id} Name {stream.Name} Size {stream.Size} Type {stream.Type} ");
                GameWorld.repo.Close();
                ResetWaterButtons();

            }


        }

        private void OceanButton_Click(object sender, EventArgs e)
        {
            if (currentWater.Name != "Ocean")
            {
                GameWorld.repo.Open();
                Water ocean = GameWorld.repo.FindWater("Ocean");
                currentWater = ocean;
                Debug.WriteLine($"Id {ocean.Id} Name {ocean.Name} Size {ocean.Size} Type {ocean.Type} ");
                GameWorld.repo.Close();
                ResetWaterButtons();
            }

        }

        private void LakeButton_Click(object sender, EventArgs e)
        {
            if (currentWater.Name != "Lake")
            {
                GameWorld.repo.Open();
                Water lake = GameWorld.repo.FindWater("Lake");
                currentWater = lake;
                Debug.WriteLine($"Id {lake.Id} Name {lake.Name} Size {lake.Size} Type {lake.Type} ");
                //// test
                //Fish torsk = GameWorld.repo.FindFish("Torsk");
                //Debug.WriteLine($"Id {torsk.Id} Name {torsk.Name} Price {torsk.Price}");
                ////
                GameWorld.repo.Close();
                ResetWaterButtons();
            }
        }

        private void BaitButton_Click(object sender, EventArgs e)
        {
            GameWorld.repo.Open();
            Bait nextBait = GameWorld.repo.FindBait("PowerBait");
            GameWorld.repo.Close();

            GameWorld.currentBait = nextBait;
            //paused = !paused;
        }

        public override void PostUpdate(GameTime gameTime)
        {
            //remove sprites if they are not needed
        }

        public override void Update(GameTime gameTime)
        {

            foreach (Cell cell in GameWorld.visualManager.grid)
            {
                cell.MyColor = Color.Yellow;
            }

            if (addComponents != null)
            {
                components.AddRange(addComponents);
                addComponents.Clear();
            }
            if (delComponents != null)
            {
                foreach (Component component in delComponents)
                {
                    components.Remove(component);
                }
                delComponents.Clear();
            }

            //Button
            foreach (var component in components)
            {
                component.Update(gameTime);
            }

           // oldState = newState;
           // newState = Keyboard.GetState();
           // //player 1 movement
           // if (alreadyFishing == false)
           // {
           //     if (newState.IsKeyDown(Keys.Right) && oldState.IsKeyUp(Keys.Right) && p1position.X <= 900)
           //     {
           //         p1position.X += 100;
           //         p1AimPosition.X += 100;
           //     }
           //     if (Keyboard.GetState().IsKeyDown(Keys.Left) && oldState.IsKeyUp(Keys.Left) && p1position.X >= 100)
           //     {
           //         p1position.X -= 100;
           //         p1AimPosition.X -= 100;
           //     }
           //
           //     if (Keyboard.GetState().IsKeyDown(Keys.Up) && oldState.IsKeyUp(Keys.Up) && p1AimPosition.Y >= 100)
           //     {
           //         p1AimPosition.Y -= 100;
           //     }
           //     if (Keyboard.GetState().IsKeyDown(Keys.Down) && oldState.IsKeyUp(Keys.Down) && p1AimPosition.Y <= 600)
           //     {
           //         p1AimPosition.Y += 100;
           //     }
           //     if (Keyboard.GetState().IsKeyDown(Keys.Enter) && oldState.IsKeyUp(Keys.Enter))
           //     {
           //         FishingKey();
           //     }
           // }
           //
           // if (alreadyFishing == false)
           // {
           //     //player 2 movement
           //     if (newState.IsKeyDown(Keys.D) && oldState.IsKeyUp(Keys.D) && p2position.X <= 900)
           //     {
           //         p2position.X += 100;
           //         p2AimPosition.X += 100;
           //     }
           //     if (Keyboard.GetState().IsKeyDown(Keys.A) && oldState.IsKeyUp(Keys.A) && p2position.X >= 100)
           //     {
           //         p2position.X -= 100;
           //         p2AimPosition.X -= 100;
           //     }
           //
           //     if (Keyboard.GetState().IsKeyDown(Keys.W) && oldState.IsKeyUp(Keys.W) && p2AimPosition.Y >= 100)
           //     {
           //         p2AimPosition.Y -= 100;
           //     }
           //     if (Keyboard.GetState().IsKeyDown(Keys.S) && oldState.IsKeyUp(Keys.S) && p2AimPosition.Y <= 600)
           //     {
           //         p2AimPosition.Y += 100;
           //     }
           //
           //     if (Keyboard.GetState().IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space))
           //     {
           //         FishingKey();
           //     }
           // }
            

        }


        #endregion
    }
}