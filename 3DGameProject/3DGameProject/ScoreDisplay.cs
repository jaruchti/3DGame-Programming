/*
 * 3D Game Programming Project
 * Dr. Liu
 * Zach Bates, Lauren Buss, Corey Darr, Jason Ruchti, Jared Tittle
 */

namespace _3DGameProject
{
    /// <summary>
    /// Abstract class used to set up contants associated with 
    /// the player's score displays (e.g. Timer and HighScore).
    /// </summary>
    public abstract class ScoreDisplay : InGameDisplay
    {
        /// <summary>
        /// Provide wrapper for digits variable (score has more meaning
        /// in this domain).
        /// </summary>
        public float Score
        {
            get { return digits; }
            set { digits = value; }
        }

        /// <summary>
        /// Set up constants for the player's score displays.
        /// </summary>
        protected void SetUpDigitContants()
        {
            DisplayNumDigits = 5;
            DisplayDigitYPos = (int) (0.004f * GameConstants.ViewportHeight); // display at top of screen
            DisplayDigitWidth = (int) (0.04f * GameConstants.ViewportWidth);
            DisplayDigitScale = 0.2f;
        }
    }
}
