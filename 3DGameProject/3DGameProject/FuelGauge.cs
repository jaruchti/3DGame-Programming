﻿/*
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
    /// Class which implements the logic for the player's Fuel Gauge.
    /// </summary>
    class FuelGauge : Gauges
    {
        /// <summary>
        /// Create a new Fuel Gauge
        /// </summary>
        /// <remarks>This is a singleton in the game</remarks>
        public FuelGauge()
        {
            textureRect = new Rectangle(0, 103, 243, 102);      // position of the Fuel Gauge in ingame texture
            displayDrawRect = new Rectangle(0, 440, 125, 60);   // draw Fuel Gauge background on bottom left of screen

            DisplayDigitXPos = 0;   // display digits on left side of screen
            SetUpDigitContants();
            SetUpDigitPositions();
        }

        /// <summary>
        /// Update the fuel gauge with the new amount of fuel.
        /// </summary>
        /// <param name="playerFuel">Fuel amount</param>
        public override void Update(float playerFuel)
        {
            digits = (int)Math.Floor(playerFuel);
        }
    }
}
