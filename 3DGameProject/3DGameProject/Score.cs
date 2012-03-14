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
    /// Class which implements the logic for the Score display in the game
    /// </summary>
    public class Score : ScoreDisplay
    {
       /// <summary>Maximum score</summary>
        public const int MaxScore = 99999;

        /// <summary>
        /// Create a new Score display
        /// </summary>
        /// <remarks>This is a singleton in the game</remarks>
        public Score()
        {
            textureRect = new Rectangle(382, 2, 339, 60);   // position of the current score background in ingame texture

            // draw Score background on top left of screen
            displayDrawRect = new Rectangle(
                0, 
                0, 
                (int) (0.36f * GameConstants.ViewportWidth), 
                (int) (0.06f * GameConstants.ViewportHeight)); 


            DisplayDigitXPos = (int) (0.14f * GameConstants.ViewportWidth);
            SetUpDigitContants();
            SetUpDigitPositions();
        }

        /// <summary>
        /// Update the Scare with the new score since the last update.
        /// </summary>
        /// <param name="newScore"></param>
        public override void Update(float newScore)
        {
            Score = newScore;

            if (Score > MaxScore)
                Score = MaxScore;
        }

        /// <summary>
        /// Reset score to zero.
        /// </summary>
        public void Reset()
        {
            digits = 0.0f;
        }
    }
}
