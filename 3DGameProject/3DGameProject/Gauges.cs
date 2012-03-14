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
    /// the player's gauges (e.g. fuel gauge and spedometer).
    /// </summary>
    public abstract class Gauges : InGameDisplay
    {
        /// <summary>
        /// Set up the constants for the player's gauges
        /// </summary>
        protected void SetUpDigitContants()
        {
            DisplayNumDigits = 2;       // only two digits needed
            DisplayDigitScale = 0.35f;
            DisplayDigitYPos = (int) (0.905f * GameConstants.ViewportHeight); // display at bottom of screen
            DisplayDigitWidth = (int) (0.07f * GameConstants.ViewportWidth);
        }
    }
}
