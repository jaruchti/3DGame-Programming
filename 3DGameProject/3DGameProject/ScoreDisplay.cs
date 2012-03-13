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
    /// Abstract class used to set up contants associated with 
    /// the player's score displays (e.g. Timer and HighScore).
    /// </summary>
    abstract class ScoreDisplay : InGameDisplay
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
            DisplayDigitYPos = 2;       // display at top of screen
            DisplayDigitWidth = 20;
            DisplayDigitScale = 0.2f;
        }
    }
}
