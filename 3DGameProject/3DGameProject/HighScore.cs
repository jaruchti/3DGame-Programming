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
    /// Class which implements the logic for the High (Best) Score display in the game
    /// </summary>
    public class HighScore : ScoreDisplay
    {
        /// <summary>
        /// Create a new HighScore display.
        /// </summary>
        /// <remarks>This is a singleton in the game</remarks>
        public HighScore()
        {
            textureRect = new Rectangle(382, 68, 339, 60); // position of the HighScore background in ingame texture

            // draw HighScore background on top right of screen
            displayDrawRect = new Rectangle(
                (int) (0.66f * GameConstants.ViewportWidth), 
                0, 
                (int) (0.34f * GameConstants.ViewportWidth), 
                (int) (0.06f * GameConstants.ViewportHeight));   

            DisplayDigitXPos = (int) (0.78f * GameConstants.ViewportWidth);
            SetUpDigitContants();
            SetUpDigitPositions();

            Score = 50.0f;
        }

        /// <summary>
        /// Update display with new HighScore.
        /// </summary>
        /// <param name="newHighScore">Potential highscore</param>
        public void Update(float newHighScore)
        {
            if (newHighScore > Score)
            {
                Score = newHighScore;
            }
        }
    }
}
