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

        GameConstants.GameState currentGameState;
        GameConstants.GameState prevGameState;
        TitleScreen titleScreen;
        GameOverScreen gameOverScreen;

        Camera gameCamera;
        Player player;
        Enemy[] enemies;
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

            currentGameState = GameConstants.GameState.Title;
            prevGameState = currentGameState;

            titleScreen = new TitleScreen();
            gameOverScreen = new GameOverScreen();

            gameCamera = new Camera();
            player = new Player();

            enemies = new Enemy[GameConstants.NumEnemy];
            for (int i = 0; i < enemies.Length; i++)
                enemies[i] = new Enemy();

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

            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].LoadContent(Content);
                enemies[i].LoadFloorPlan(map.FloorPlan);
            }

            SetUpEnemyPositions();

            timer.LoadContent(ref device, Content);
            highScore.LoadContent(ref device, Content);

            gameCamera.LoadFloorPlan(map.FloorPlan);

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
            prevGameState = currentGameState;

            if (currentGameState == GameConstants.GameState.Title)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    currentGameState = GameConstants.GameState.Playing;
            }
            else if (currentGameState == GameConstants.GameState.Playing)
            {
                gameCamera.Update(player.ForwardDirection, player.Position, device.Viewport.AspectRatio);
                player.Update(Keyboard.GetState(), gameTime, ref map, ref currentGameState);
                timer.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            else if (currentGameState == GameConstants.GameState.End)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    currentGameState = GameConstants.GameState.Playing;
                    Reset();
                }
            }
            
            if (currentGameState == GameConstants.GameState.End && prevGameState == GameConstants.GameState.Playing)
            {
                highScore.Update(timer.Score);
                gameOverScreen.Update(timer.Score);
            }

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);



            if (currentGameState == GameConstants.GameState.Title)
            {
                titleScreen.Draw();
            }
            else if (currentGameState == GameConstants.GameState.Playing || currentGameState == GameConstants.GameState.End)
            {
                //RasterizerState rs = new RasterizerState();
                //rs.FillMode = FillMode.Solid;

                //GraphicsDevice.RasterizerState = rs;

                skybox.Draw(ref device, gameCamera, player);

                foreach (Enemy e in enemies)
                    e.Draw(gameCamera);

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

                if (currentGameState == GameConstants.GameState.End)
                    gameOverScreen.Draw();
            }


            base.Draw(gameTime);
        }

        private void Reset()
        {
            timer.Reset();
            player.Reset();
        }

        private void SetUpEnemyPositions()
        {
            enemies[0].UpdatePositionAndBoundingSphere(new Vector3(9.5f, 0.1f, -8.5f));
            enemies[1].UpdatePositionAndBoundingSphere(new Vector3(8.5f, 0.1f, -9.5f));
            enemies[2].UpdatePositionAndBoundingSphere(new Vector3(9.5f, 0.1f, -9.5f));
            enemies[3].UpdatePositionAndBoundingSphere(new Vector3(9.5f, 0.1f, -10.5f));
        }
    }
}
