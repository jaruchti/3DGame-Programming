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
    /// Class which implements the logic for the player's Spedometer.
    /// </summary>
    class Spedometer : Gauges
    {
        /// <summary>
        /// Create a new Spedometer
        /// </summary>
        /// <remarks>This is a singleton in the game</remarks>
        public Spedometer()
        {
            textureRect = new Rectangle(0, 0, 243, 102);  // position of the Spedometer in ingame texture

            // draw Spedometer background on bottom right of screen
            displayDrawRect = new Rectangle(
                (int) (0.750f * GameConstants.ViewportWidth), 
                (int) (0.885f * GameConstants.ViewportHeight), 
                (int) (0.250f * GameConstants.ViewportWidth), 
                (int) (0.125f * GameConstants.ViewportHeight));

            // display digits on right side of screen
            DisplayDigitXPos = (int) (0.87f * GameConstants.ViewportWidth); 
 
            SetUpDigitContants();
            SetUpDigitPositions();
        }

        /// <summary>
        /// Update the mph shown on the spedometer based on the player's new velocity.
        /// </summary>
        /// <param name="playerVelocity">Player's velocity</param>
        public override void Update(float playerVelocity)
        {
            playerVelocity = Math.Abs(playerVelocity);  // show speed, not velocity

            digits = (int)Math.Floor(playerVelocity / GameConstants.MaxVelocity * 100); // find % of max velocity

            if (digits == 100) // force mph between 0 and 99
                digits--;
        }
    }
}
