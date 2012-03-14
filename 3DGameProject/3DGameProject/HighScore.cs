/*
 * 3D Game Programming Project
 * Dr. Liu
 * Zach Bates, Lauren Buss, Corey Darr, Jason Ruchti, Jared Tittle
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
        TextReader tr;
        TextWriter tw;

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

            Score = ReadHighScore();
        }

        /// <summary>
        /// Update display with new HighScore.
        /// </summary>
        /// <param name="newHighScore">Potential highscore</param>
        public override void Update(float newHighScore)
        {
            if (newHighScore > Score)
            {
                Score = newHighScore;
                WriteHighScore();
            }
        }

        /// <summary>
        /// Reads in the current highscore from the highscores file
        /// </summary>
        /// <returns>Current best highscore</returns>
        private float ReadHighScore()
        {
            float r = 0.0f;

            tr = new StreamReader("Scores/scores.txt");
            r = (float) Convert.ToDouble(tr.ReadLine());
            tr.Close();

            return r;
        }

        /// <summary>
        /// Write new highscore to highscores file.
        /// </summary>
        private void WriteHighScore()
        {
            tw = new StreamWriter("Scores/scores.txt");
            tw.WriteLine(Score);
            tr.Close();
        }
    }
}
