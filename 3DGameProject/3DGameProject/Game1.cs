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

        // state managers
        GameConstants.GameState currentGameState; 
        GameConstants.GameState prevGameState;
        KeyboardState currentKeyboardState;
        KeyboardState prevKeyBoardState;
        GamePadState currentGamePadState;
        GamePadState prevGamePadState;

        // screens
        TitleScreen titleScreen;
        InstructionScreen instructionScreen;
        HighScoreScreen highScoreScreen;
        CreditsScreen creditsScreen;
        IntroScreen introScreen;
        GetReadyScreen readyScreen;
        GameOverScreen gameOverScreen;

        // game objects
        Camera gameCamera;
        Player player;
        Enemies enemies;
        Map map;
        MiniMap miniMap;
        Skybox skybox;

        // displays in-game
        Score score;
        HighScore highScore;

        // sound effects
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
            // set up the display window
            graphics.PreferredBackBufferWidth = 500;
            graphics.PreferredBackBufferHeight = 500;
            graphics.ApplyChanges();

            // update the ViewportHeight and ViewportWidth constants so the screens know
            // how to draw themselves
            GameConstants.ViewportHeight = graphics.GraphicsDevice.Viewport.Height;
            GameConstants.ViewportWidth = graphics.GraphicsDevice.Viewport.Width;

            // display title in the window
            Window.Title = "Alien Attack";

            // setup the state of the game
            currentGameState = GameConstants.GameState.Title;
            currentKeyboardState = Keyboard.GetState();
            prevKeyBoardState = currentKeyboardState;

            // set up the screens
            titleScreen = new TitleScreen();
            instructionScreen = new InstructionScreen();
            highScoreScreen = new HighScoreScreen();
            creditsScreen = new CreditsScreen();
            introScreen = new IntroScreen();
            readyScreen = new GetReadyScreen();
            gameOverScreen = new GameOverScreen();

            // set up the game objects
            gameCamera = new Camera();
            player = new Player();
            enemies = new Enemies();
            map = new Map();
            miniMap = new MiniMap();
            skybox = new Skybox();

            // set up the ingame displays
            score = new Score();
            highScore = new HighScore();

            // set up the soundtrack
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

            // load up all of the screens
            titleScreen.LoadContent(ref device, Content);
            instructionScreen.LoadContent(ref device, Content);
            highScoreScreen.LoadContent(ref device, Content);
            creditsScreen.LoadContent(ref device, Content);
            introScreen.LoadContent(ref device, Content);
            readyScreen.LoadContent(ref device, Content);
            gameOverScreen.LoadContent(ref device, Content);

            // load up all of the game objects
            player.LoadContent(ref device, Content);
            map.LoadContent(ref device, Content);
            miniMap.LoadContent(ref device, Content);
            skybox.LoadContent(Content);
            enemies.LoadContent(ref device, Content);

            // load up all of the in-game displays
            score.LoadContent(ref device, Content);
            highScore.LoadContent(ref device, Content);

            // load up the game soundtrack
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

            // check to see if we should enable full screen
            if (currentKeyboardState.IsKeyDown(Keys.LeftControl) && currentKeyboardState.IsKeyDown(Keys.F) &&
                (prevKeyBoardState.IsKeyUp(Keys.LeftControl) || prevKeyBoardState.IsKeyUp(Keys.F)) &&
                currentGameState != GameConstants.GameState.Playing && currentGameState != GameConstants.GameState.Ready)
            {
                ToggleFullScreen();
            }

            if (currentGameState == GameConstants.GameState.Title) // currently at the title screen
            {
                
                if (currentKeyboardState.IsKeyDown(Keys.H) && prevKeyBoardState.IsKeyUp(Keys.H))
                {
                    // test if user wants to go to highscore screen
                    prevGameState = currentGameState;
                    currentGameState = GameConstants.GameState.HighScoreScreen;
                    highScoreScreen.Update();
                }
                else if (currentKeyboardState.IsKeyDown(Keys.I) && prevKeyBoardState.IsKeyUp(Keys.I))
                {
                    // test if user wants to go the instruction screen
                    prevGameState = currentGameState;
                    currentGameState = GameConstants.GameState.InstructionScreen;
                }
                else if (currentKeyboardState.IsKeyDown(Keys.C) && prevKeyBoardState.IsKeyUp(Keys.C))
                {
                    // test if the user wants to go to the credits screen
                    prevGameState = currentGameState;
                    currentGameState = GameConstants.GameState.CreditsScreen;
                }
                else if (currentKeyboardState.IsKeyDown(Keys.Space) || currentGamePadState.Buttons.A == ButtonState.Pressed)
                {
                    // test if the user wants to transition to playing
                    currentGameState = GameConstants.GameState.Intro;
                    enemies.SetUpIntroPositions(player.Position);
                }
            }
            else if (currentGameState == GameConstants.GameState.Intro)
            {
                // play the introduction; the camera is set to view the world as the enemy does
                player.AutoPilot(ref currentGameState); // have the player guide itself around the city
                gameCamera.Update(player.ForwardDirection, enemies.EnemyIntroPosition, map.FloorPlan, device.Viewport.AspectRatio);
                enemies.PlayIntro(player); // have an enemy 'chase' the player

                if (((currentKeyboardState.IsKeyDown(Keys.Space) && prevKeyBoardState.IsKeyUp(Keys.Space)) ||
                    (prevGamePadState.Buttons.A == ButtonState.Pressed && currentGamePadState.Buttons.A == ButtonState.Released)) ||
                    currentGameState == GameConstants.GameState.Ready)
                {
                    // test to see if the user wants to transition to the ready state
                    Reset();
                    currentGameState = GameConstants.GameState.Ready;
                }
            }
            else if (currentGameState == GameConstants.GameState.Ready)
            {
                // set the stage before play begins
                // focus the camera on the player, show the countdown, and play the engine noise
                gameCamera.Update(player.ForwardDirection, player.Position, map.FloorPlan, device.Viewport.AspectRatio);
                readyScreen.Update((float) gameTime.ElapsedGameTime.TotalSeconds, ref currentGameState);
                player.PlayEngineNoise();
            }
            else if (currentGameState == GameConstants.GameState.Playing)
            {
                // implementation of the playing logic is done with the methods called below
                // 1) the camera must be updated based on the player position and the location in the map
                // 2) the enemies their positions with each go-around
                // 3) the players must update based on user input
                // 4) the score must be updated based on time elapsed
                // 5) the bonuses must be updated based on time elapsed (this is to stop showing the bonus screen after a interval of time)
                gameCamera.Update(player.ForwardDirection, player.Position, map.FloorPlan, device.Viewport.AspectRatio);
                enemies.Update(player, map.FloorPlan, gameTime, ref currentGameState);
                player.Update(currentKeyboardState, currentGamePadState, gameTime, ref map, ref currentGameState);
                score.Update((float) gameTime.ElapsedGameTime.TotalSeconds, player);
                map.Bonuses.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

                if (currentGameState == GameConstants.GameState.End)
                {
                    // the game has transition to ending during the above method calls
                    // update the highscore and gameOverScreen
                    highScore.Update(score.Score);
                    gameOverScreen.Update(score.Score);
                }
            }
            else if (currentGameState == GameConstants.GameState.End)
            {
                enemies.Update(player, map.FloorPlan, gameTime, ref currentGameState); // move the enemy torward the player
               
                if (Keyboard.GetState().IsKeyDown(Keys.Space) || currentGamePadState.Buttons.A == ButtonState.Pressed)
                {
                    // test if the user wants to begin player again
                    currentGameState = GameConstants.GameState.Ready;
                    Reset();
                }

                if (currentKeyboardState.IsKeyDown(Keys.H) && prevKeyBoardState.IsKeyUp(Keys.H))
                {
                    // test if the user wants to display the highscore screen
                    prevGameState = currentGameState;
                    currentGameState = GameConstants.GameState.HighScoreScreen;
                    highScoreScreen.Update();
                }
                else if (currentKeyboardState.IsKeyDown(Keys.I) && prevKeyBoardState.IsKeyUp(Keys.I))
                {
                    // test if the user wants to display the instructions screen
                    prevGameState = currentGameState;
                    currentGameState = GameConstants.GameState.InstructionScreen;
                }
                else if (currentKeyboardState.IsKeyDown(Keys.C) && prevKeyBoardState.IsKeyUp(Keys.C))
                {
                    // test if the user wants to display the credits screen
                    prevGameState = currentGameState;
                    currentGameState = GameConstants.GameState.CreditsScreen;
                }
            }
            else if (currentGameState == GameConstants.GameState.HighScoreScreen)
            {
                // we are at highscore screen, test if user wants to go back to prior state
                if (currentKeyboardState.IsKeyDown(Keys.H) && prevKeyBoardState.IsKeyUp(Keys.H))
                    currentGameState = prevGameState;
            }
            else if (currentGameState == GameConstants.GameState.InstructionScreen)
            {
                // we are at instruction screen, test if user wants to go back to prior state
                if (currentKeyboardState.IsKeyDown(Keys.I) && prevKeyBoardState.IsKeyUp(Keys.I))
                    currentGameState = prevGameState;
            }
            else if (currentGameState == GameConstants.GameState.CreditsScreen)
            {
                // we are at credits screen, test if user wants to go back to prior state
                if (currentKeyboardState.IsKeyDown(Keys.C) && prevKeyBoardState.IsKeyUp(Keys.C))
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
            // clear the graphics card for effect files to work properly 
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0); 

            if (currentGameState == GameConstants.GameState.Title)
                titleScreen.Draw(); // at title screen
            else if (currentGameState == GameConstants.GameState.HighScoreScreen)
                highScoreScreen.Draw(); // at highscore screen
            else if (currentGameState == GameConstants.GameState.InstructionScreen)
                instructionScreen.Draw(); // at instruction screen
            else if (currentGameState == GameConstants.GameState.CreditsScreen)
                creditsScreen.Draw(); // at credits screen
            else
            {
                // we are not at any of the screens, so we must draw the environment
                // the environment must drawn in the correct order for proper display
                skybox.Draw(ref device, gameCamera, player);
                enemies.Draw(ref device, gameCamera, currentGameState); // draw enemies before map to avoid see-through buildings
                if (currentGameState == GameConstants.GameState.Playing) 
                    // check to see if playing before drawing bonuses to avoid bonuses in introduction
                    map.Bonuses.DrawBonuses(gameCamera);
                map.Draw(ref device, gameCamera);
                player.Draw(gameCamera, currentGameState);

                if (currentGameState == GameConstants.GameState.Intro)
                    introScreen.Draw(); // give instructions on how to skip intro
                else if (currentGameState == GameConstants.GameState.Playing || currentGameState == GameConstants.GameState.Ready ||
                    currentGameState == GameConstants.GameState.End)
                {
                    // we are in the 'real' game
                    // draw player in-game display panels
                    miniMap.Draw(gameTime, player, enemies, map);
                    score.Draw();
                    highScore.Draw();
                    enemies.WarningScreen.Draw(gameTime);

                    if (currentGameState == GameConstants.GameState.Playing)
                        map.Bonuses.DrawBonusScreen(); // display that a bonus has recently been picked up (if applicable)
                    if (currentGameState == GameConstants.GameState.End)
                        gameOverScreen.Draw(); // show end-game information and how to begin again
                    else if (currentGameState == GameConstants.GameState.Ready)
                        readyScreen.Draw(); // display a countdown if the user is in the ready state
                }
            }

            gameSongs.PlayBackground(currentGameState);    // play the appropriate background music

            base.Draw(gameTime);
        }

        /// <summary>
        /// This method is called when the end of the game is reached to reset the state of the objects
        /// to before when player began.
        /// </summary>
        private void Reset()
        {
            score.Reset();
            player.Reset();
            enemies.Reset();
            map.Bonuses.Reset();
        }

        /// <summary>
        /// This method should be called to toggle the full-screen display
        /// </summary>
        private void ToggleFullScreen()
        {
            graphics.ToggleFullScreen();

            if (!graphics.IsFullScreen) // turning off fullscreen, reset the width and height
            {
                graphics.PreferredBackBufferHeight = 500;
                graphics.PreferredBackBufferWidth = 500;
            }

            // set up the values in GameConstants so all of the objects can display properly
            GameConstants.ViewportHeight = graphics.GraphicsDevice.Viewport.Height;
            GameConstants.ViewportWidth = graphics.GraphicsDevice.Viewport.Width;

            // reset the positions of all of the screens
            titleScreen.SetPosition();
            instructionScreen.SetPosition();
            highScoreScreen.SetPosition();
            creditsScreen.SetPosition();
            introScreen.SetPosition();
            readyScreen.SetPosition();
            gameOverScreen.SetPosition();

            // reset the positions of all of the game objects
            map.Bonuses.BonusScreen.SetPosition(); 

            player.SetGuagePosition();
            score.SetPosition();
            highScore.SetPosition();
        }
    }
}
