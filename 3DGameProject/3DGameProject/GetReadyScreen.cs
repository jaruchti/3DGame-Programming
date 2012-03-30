/*
 * 3D Game Programming Project
 * Dr. Liu
 * Zach Bates, Lauren Buss, Corey Darr, Jason Ruchti, Jared Tittle
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _3DGameProject
{
    /// <summary>
    /// Class which implements the logic to display relevant text to the screen while
    /// the player is about to begin playing the game (e.g. countdown, instructions).
    /// </summary>
    /// <remarks>This is a singleton in the game</remarks>
    public class GetReadyScreen : GameTextScreen
    {
        /// <summary>Time to display the GetReadyScreen</summary>
        public const int SecondsOnScreen = 4; 
        private float seconds;                  // seconds elapsed since the first display

        private Vector2 countdownPosition;      // position of the countdown
        private Vector2 instructionsPosition;   // position of the instructions

        private String strCountdown = "";
        private String strInstructions = "Get Ready";

        /// <summary>
        /// Load the content required for the screen.
        /// </summary>
        /// <param name="device">Graphics card (to initialize spritebatch)</param>
        /// <param name="content">Content pipeline (for fonts)</param>
        public override void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            base.LoadContent(ref device, content);
            SetPosition();
        }

        /// <summary>
        /// Set the position of the elements of the GameReadyScreen based on the characteristics
        /// of the viewport
        /// </summary>
        public override void SetPosition()
        {

            // Place instructions at bottom of screen just above health meter
            textSize = mediumFont.MeasureString(strInstructions);
            instructionsPosition = new Vector2(
                (int)(GameConstants.ViewportWidth / 2) - textSize.X / 2,
                GameConstants.ViewportHeight - 2 * largeFont.LineSpacing);
        }

        /// <summary>
        /// Update the GetReadyScreen with the seconds elapsed since the last call to this method,
        /// and if the GetReadyScreen has been displayed long enough, transition to playing state.
        /// </summary>
        /// <param name="elapsedSeconds">Seconds elapsed since the last call to this method.</param>
        /// <param name="gameState">Current gamestate</param>
        /// <remarks>This method can change the global gamestate to playing</remarks>
        public void Update(float elapsedSeconds, ref GameConstants.GameState gameState)
        {
            seconds += elapsedSeconds;

            strCountdown = Convert.ToString((int)(SecondsOnScreen - seconds + 1)); 

            // Place countdown slightly below center of the screen
            textSize = largeFont.MeasureString(strCountdown);
            countdownPosition = new Vector2(
                (int)(GameConstants.ViewportWidth / 2) - textSize.X / 2, 
                (int)(0.6f * GameConstants.ViewportHeight));

            if (seconds > SecondsOnScreen)
            {
                // The countdown has been displayed long enough, transition to playing.
                seconds = 0;
                gameState = GameConstants.GameState.Playing;
            }
        }

        /// <summary>
        /// Draw countdown, instructions to the screen.
        /// </summary>
        public void Draw()
        {
            spriteBatch.Begin();

            spriteBatch.DrawString(largeFont, strCountdown, countdownPosition, Color.Orange);
            spriteBatch.DrawString(mediumFont, strInstructions, instructionsPosition, Color.LightGray);

            spriteBatch.End();
        }
    }
}
