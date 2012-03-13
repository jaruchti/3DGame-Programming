using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace _3DGameProject
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        GraphicsDevice device;

        GameConstants.GameState gameState;
        KeyboardState currentKeyboardState;
        KeyboardState prevKeyBoardState;

        TitleScreen titleScreen;
        GameOverScreen gameOverScreen;

        Camera gameCamera;
        Player player;
        Enemies enemies;
        Map map;
        MiniMap miniMap;
        Skybox skybox;

        Timer timer;
        HighScore highScore;

        GameObject boundingSphere = new GameObject();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 500;
            graphics.PreferredBackBufferHeight = 500;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            Window.Title = "Alien Attack";

            gameState = GameConstants.GameState.Title;
            currentKeyboardState = Keyboard.GetState();
            prevKeyBoardState = currentKeyboardState;

            titleScreen = new TitleScreen();
            gameOverScreen = new GameOverScreen();

            gameCamera = new Camera();
            player = new Player();
            enemies = new Enemies();
            map = new Map();
            miniMap = new MiniMap();
            skybox = new Skybox();

            timer = new Timer();
            highScore = new HighScore();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            device = graphics.GraphicsDevice;

            titleScreen.LoadContent(ref device, Content);
            gameOverScreen.LoadContent(ref device, Content);

            player.LoadContent(ref device, Content);
            map.LoadContent(ref device, Content);
            miniMap.LoadContent(ref device, Content);
            skybox.LoadContent(Content);
            enemies.LoadContent(Content);

            timer.LoadContent(ref device, Content);
            highScore.LoadContent(ref device, Content);

            gameCamera.LoadFloorPlan(map.FloorPlan);
            enemies.LoadFloorPlan(map.FloorPlan);

            boundingSphere.Model = Content.Load<Model>("Models/sphere1uR");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Nothing to do here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            currentKeyboardState = Keyboard.GetState();

            if (gameState == GameConstants.GameState.Title)
            {
                if (currentKeyboardState.IsKeyDown(Keys.Space))
                {
                    gameState = GameConstants.GameState.Intro;
                    gameCamera.ViewTopDown(device.Viewport.AspectRatio);
                    enemies.SetUpIntroPositions();
                }
            }
            else if (gameState == GameConstants.GameState.Intro)
            {
                enemies.PlayIntro(gameTime);

                if (currentKeyboardState.IsKeyDown(Keys.Space) && prevKeyBoardState.IsKeyUp(Keys.Space))
                {
                    enemies.SetUpEnemyPositions();
                    gameState = GameConstants.GameState.Playing;
                }
            }
            else if (gameState == GameConstants.GameState.Playing)
            {
                gameCamera.Update(player.ForwardDirection, player.Position, device.Viewport.AspectRatio);
                player.Update(Keyboard.GetState(), gameTime, ref map, ref gameState);
                timer.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

                if (gameState == GameConstants.GameState.End)
                {
                    highScore.Update(timer.Score);
                    gameOverScreen.Update(timer.Score);
                }
            }
            else if (gameState == GameConstants.GameState.End)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    gameState = GameConstants.GameState.Playing;
                    Reset();
                }
            }

            prevKeyBoardState = currentKeyboardState;

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);

            if (gameState == GameConstants.GameState.Title)
            {
                titleScreen.Draw();
            }
            else if (gameState == GameConstants.GameState.Intro)
            {
                skybox.Draw(ref device, gameCamera, player);
                enemies.Draw(gameCamera);
                map.Draw(ref device, gameCamera);
            }
            else if (gameState == GameConstants.GameState.Playing || gameState == GameConstants.GameState.End)
            {
                //RasterizerState rs = new RasterizerState();
                //rs.FillMode = FillMode.Solid;

                //GraphicsDevice.RasterizerState = rs;

                skybox.Draw(ref device, gameCamera, player);
                enemies.Draw(gameCamera);
                map.Draw(ref device, gameCamera);
                miniMap.Draw(player, enemies, map);
                player.Draw(gameCamera);

                timer.Draw();
                highScore.Draw();

                //rs = new RasterizerState();
                //rs.FillMode = FillMode.WireFrame;
                //GraphicsDevice.RasterizerState = rs;
                //enemy.DrawBoundingSphere(gameCamera.ViewMatrix,
                //    gameCamera.ProjectionMatrix, boundingSphere);

                if (gameState == GameConstants.GameState.End)
                    gameOverScreen.Draw();
            }

            base.Draw(gameTime);
        }

        private void Reset()
        {
            timer.Reset();
            player.Reset();
            enemies.Reset();
        }
    }
}
