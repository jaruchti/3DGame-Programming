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
    /// Class which implements the logic for the Timer (Score) display in the game
    /// </summary>
    class Timer : ScoreDisplay
    {
       /// <summary>Maximum score (time)</summary>
        public const int MaxTime = 99999;

        /// <summary>
        /// Create a new timer
        /// </summary>
        /// <remarks>This is a singleton in the game</remarks>
        public Timer()
        {
            textureRect = new Rectangle(382, 2, 339, 60);   // position of the Timer background in ingame texture
            displayDrawRect = new Rectangle(0, 0, 180, 30); // draw Timer background on top right of screen


            DisplayDigitXPos = 70;
            SetUpDigitContants();
            SetUpDigitPositions();
        }

        /// <summary>
        /// Update the Timer with the new seconds elapsed since the last update.
        /// </summary>
        /// <param name="elapsedSecs"></param>
        public override void Update(float elapsedSecs)
        {
            Score += elapsedSecs;

            if (Score > MaxTime)
                Score = MaxTime;
        }

        /// <summary>
        /// Reset time to zero.
        /// </summary>
        public void Reset()
        {
            digits = 0.0f;
        }
    }
}
