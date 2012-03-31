/*
 * 3D Game Programming Project
 * Dr. Liu
 * Zach Bates, Lauren Buss, Corey Darr, Jason Ruchti, Jared Tittle
 */

using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace _3DGameProject
{
    /// <summary>
    /// Class which implements the logic for the High (Best) Score display in the game
    /// </summary>
    public class HighScore : ScoreDisplay
    {
        /// <summary>
        /// The default highscore when the player first begins playing and no scores
        /// have been saved yet.
        /// </summary>
        public int DefaultHighScore = 50;

        /// <summary>
        /// Create a new HighScore display.
        /// </summary>
        /// <remarks>This is a singleton in the game</remarks>
        public HighScore()
        {
            textureRect = new Rectangle(382, 68, 339, 60); // position of the HighScore background in ingame texture
            SetPosition();

            Score = ReadHighScore();
        }

        /// <summary>
        /// Set the position of the elements of the HighScore display based on the characteristics
        /// of the viewport
        /// </summary>
        public override void SetPosition()
        {
            // draw HighScore background on top right of screen
            displayDrawRect = new Rectangle(
                (int)(0.66f * GameConstants.ViewportWidth),
                0,
                (int)(0.34f * GameConstants.ViewportWidth),
                (int)(0.06f * GameConstants.ViewportHeight));

            DisplayDigitXPos = (int)(0.78f * GameConstants.ViewportWidth);
            SetUpDigitContants();
            SetUpDigitPositions();
        }

        /// <summary>
        /// Update display with new HighScore.
        /// </summary>
        /// <param name="potentialNewHighScore">Potential highscore</param>
        public void Update(float potentialNewHighScore)
        {
            if (potentialNewHighScore > Score)
            {
                Score = potentialNewHighScore;
                WriteHighScore();
            }
        }

        /// <summary>
        /// Reads in the current highscore from the highscores file or the default
        /// highscore if no highscore has yet been attained.
        /// </summary>
        /// <returns>Current best highscore</returns>
        private float ReadHighScore()
        {
            float r = DefaultHighScore;
            StreamReader sr;

            if (File.Exists("Scores/scores.txt"))
            {
                sr = new StreamReader("Scores/scores.txt");
                r = (float)Convert.ToDouble(sr.ReadLine());
                sr.Close();
            }

            return r;
        }

        /// <summary>
        /// Write new highscore to highscores file.
        /// </summary>
        private void WriteHighScore()
        {
            StreamWriter sw = new StreamWriter("Scores/scores.txt");
            sw.WriteLine(Score);
            sw.Close();
        }
    }
}
