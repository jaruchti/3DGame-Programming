/*
 * 3D Game Programming Project
 * Dr. Liu
 * Zach Bates, Lauren Buss, Corey Darr, Jason Ruchti, Jared Tittle
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        KeyboardState currentKeyboardState;
        KeyboardState prevKeyBoardState;
        GamePadState currentGamePadState;
        GamePadState prevGamePadState;

        TitleScreen titleScreen;
        HighScoreScreen highScoreScreen;
        IntroScreen introScreen;
        GetReadyScreen readyScreen;
        GameOverScreen gameOverScreen;

        Camera gameCamera;
        Player player;
        Enemies enemies;
        Map map;
        MiniMap miniMap;
        Skybox skybox;

        Score score;
        HighScore highScore;

        GameSongs gameSongs;

        /// <summary>
        /// Constructor for the game object
        /// </summary>
        /// <remarks>Used by Program.cs</remarks>
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
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            GameConstants.ViewportHeight = graphics.GraphicsDevice.Viewport.Height;
            GameConstants.ViewportWidth = graphics.GraphicsDevice.Viewport.Width;

            Window.Title = "Alien Attack";

            currentGameState = GameConstants.GameState.Title;
            currentKeyboardState = Keyboard.GetState();
            prevKeyBoardState = currentKeyboardState;

            titleScreen = new TitleScreen();
            highScoreScreen = new HighScoreScreen();
            introScreen = new IntroScreen();
            readyScreen = new GetReadyScreen();
            gameOverScreen = new GameOverScreen();

            gameCamera = new Camera();
            player = new Player();
            enemies = new Enemies();
            map = new Map();
            miniMap = new MiniMap();
            skybox = new Skybox();

            score = new Score();
            highScore = new HighScore();

            gameSongs = new GameSongs();

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
            highScoreScreen.LoadContent(ref device, Content);
            introScreen.LoadContent(ref device, Content);
            readyScreen.LoadContent(ref device, Content);
            gameOverScreen.LoadContent(ref device, Content);

            player.LoadContent(ref device, Content);
            map.LoadContent(ref device, Content);
            miniMap.LoadContent(ref device, Content);
            skybox.LoadContent(Content);
            enemies.LoadContent(ref device, Content);

            score.LoadContent(ref device, Content);
            highScore.LoadContent(ref device, Content);

            gameSongs.LoadContent(Content);
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
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            if (currentKeyboardState.IsKeyDown(Keys.LeftControl) && currentKeyboardState.IsKeyDown(Keys.F) &&
                (prevKeyBoardState.IsKeyUp(Keys.LeftControl) || prevKeyBoardState.IsKeyUp(Keys.F)))
            {
                ToggleFullScreen();
            }

            if (currentGameState == GameConstants.GameState.Title)
            {
                if (currentKeyboardState.IsKeyDown(Keys.H) && prevKeyBoardState.IsKeyUp(Keys.H))
                {
                    prevGameState = currentGameState;
                    currentGameState = GameConstants.GameState.HighScoreScreen;
                    highScoreScreen.Update();
                }

                if (currentKeyboardState.IsKeyDown(Keys.Space) || currentGamePadState.Buttons.A == ButtonState.Pressed)
                {
                    currentGameState = GameConstants.GameState.Intro;
                    enemies.SetUpIntroPositions(player.Position);
                }
            }
            else if (currentGameState == GameConstants.GameState.Intro)
            {
                player.AutoPilot(ref currentGameState);
                gameCamera.Update(player.ForwardDirection, enemies.EnemyIntroPosition, map.FloorPlan, device.Viewport.AspectRatio);
                enemies.PlayIntro(player);

                if (((currentKeyboardState.IsKeyDown(Keys.Space) && prevKeyBoardState.IsKeyUp(Keys.Space)) ||
                    (prevGamePadState.Buttons.A == ButtonState.Pressed && currentGamePadState.Buttons.A == ButtonState.Released)) ||
                    currentGameState == GameConstants.GameState.Ready)
                {
                    Reset();
                    currentGameState = GameConstants.GameState.Ready;
                }
            }
            else if (currentGameState == GameConstants.GameState.Ready)
            {
                gameCamera.Update(player.ForwardDirection, player.Position, map.FloorPlan, device.Viewport.AspectRatio);
                readyScreen.Update((float) gameTime.ElapsedGameTime.TotalSeconds, ref currentGameState);
                player.PlayEngineNoise();
            }
            else if (currentGameState == GameConstants.GameState.Playing)
            {
                gameCamera.Update(player.ForwardDirection, player.Position, map.FloorPlan, device.Viewport.AspectRatio);
                enemies.Update(player, map.FloorPlan, gameTime, ref currentGameState);
                player.Update(currentKeyboardState, currentGamePadState, gameTime, ref map, ref currentGameState);
                score.Update((float) gameTime.ElapsedGameTime.TotalSeconds, player);
                map.Bonuses.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

                if (currentGameState == GameConstants.GameState.End)
                {
                    highScore.Update(score.Score);
                    gameOverScreen.Update(score.Score);
                }
            }
            else if (currentGameState == GameConstants.GameState.End)
            {
                enemies.Update(player, map.FloorPlan, gameTime, ref currentGameState);

                if (Keyboard.GetState().IsKeyDown(Keys.Space) || currentGamePadState.Buttons.A == ButtonState.Pressed)
                {
                    currentGameState = GameConstants.GameState.Ready;
                    Reset();
                }

                if (currentKeyboardState.IsKeyDown(Keys.H) && prevKeyBoardState.IsKeyUp(Keys.H))
                {
                    prevGameState = currentGameState;
                    currentGameState = GameConstants.GameState.HighScoreScreen;
                    highScoreScreen.Update();
                }
            }
            else if (currentGameState == GameConstants.GameState.HighScoreScreen)
            {
                if (currentKeyboardState.IsKeyDown(Keys.H) && prevKeyBoardState.IsKeyUp(Keys.H))
                    currentGameState = prevGameState;
            }

            prevKeyBoardState = currentKeyboardState;
            prevGamePadState = currentGamePadState;

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
                titleScreen.Draw();
            else if (currentGameState == GameConstants.GameState.HighScoreScreen)
                highScoreScreen.Draw();   
            else
            {
                skybox.Draw(ref device, gameCamera, player);
                enemies.Draw(ref device, gameCamera, currentGameState);
                if (currentGameState == GameConstants.GameState.Playing)
                    map.Bonuses.DrawBonuses(gameCamera);
                map.Draw(ref device, gameCamera);
                player.Draw(gameCamera, currentGameState);

                if (currentGameState == GameConstants.GameState.Intro)
                {}//introScreen.Draw();
                else if (currentGameState == GameConstants.GameState.Playing || currentGameState == GameConstants.GameState.Ready ||
                    currentGameState == GameConstants.GameState.End)
                {
                    miniMap.Draw(gameTime, player, enemies, map);
                    score.Draw();
                    highScore.Draw();
                    enemies.WarningScreen.Draw(gameTime);

                    if (currentGameState == GameConstants.GameState.Playing)
                        map.Bonuses.DrawBonusScreen();

                    if (currentGameState == GameConstants.GameState.End)
                        gameOverScreen.Draw();
                    else if (currentGameState == GameConstants.GameState.Ready)
                        readyScreen.Draw();
                }
            }

            gameSongs.PlayBackground(currentGameState);    // play the appropriate background music

            base.Draw(gameTime);
        }

        private void Reset()
        {
            score.Reset();
            player.Reset();
            enemies.Reset();
            map.Bonuses.Reset();
        }

        private void ToggleFullScreen()
        {
            graphics.ToggleFullScreen();

            if (!graphics.IsFullScreen)
            {
                graphics.PreferredBackBufferHeight = 500;
                graphics.PreferredBackBufferWidth = 500;
            }

            GameConstants.ViewportHeight = graphics.GraphicsDevice.Viewport.Height;
            GameConstants.ViewportWidth = graphics.GraphicsDevice.Viewport.Width;

            titleScreen.SetPosition();
            highScoreScreen.SetPosition();
            introScreen.SetPosition();
            readyScreen.SetPosition();
            gameOverScreen.SetPosition();

            map.Bonuses.BonusScreen.SetPosition();

            player.SetGuagePosition();
            score.SetPosition();
            highScore.SetPosition();
        }
    }
}
