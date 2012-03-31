/*
 * 3D Game Programming Project
 * Dr. Liu
 * Zach Bates, Lauren Buss, Corey Darr, Jason Ruchti, Jared Tittle
 */

using Microsoft.Xna.Framework;

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
            SetPosition();

            Score = 50.0f;
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
