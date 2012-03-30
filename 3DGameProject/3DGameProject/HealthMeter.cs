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
    /// Class which implements the logic for the Player's health meter
    /// </summary>
    class HealthMeter : Gauges
    {
        /// <summary>
        /// Create a new Health Meter
        /// </summary>
        /// <remarks>This is a singleton in the game</remarks>
        public HealthMeter()
        {
            // digits is inherited from InGameDisplay
            // digits will store the health of the player, and is the variable that is displayed to the screen
            digits = Player.StartingHealth; 

            textureRect = new Rectangle(727, 0, 280, 62); // position of the Health Meter in ingame texture
            SetPosition();
        }

        /// <summary>
        /// Set the position of the HealthMeter based on the characteristics
        /// of the viewport
        /// </summary>
        public override void SetPosition()
        {
            // draw Health Meter background on bottom center of screen
            displayDrawRect = new Rectangle(
                (int)(0.3f * GameConstants.ViewportWidth),
                (int)(0.885f * GameConstants.ViewportHeight),
                (int)(0.4f * GameConstants.ViewportWidth),
                (int)(0.125f * GameConstants.ViewportHeight));

            DisplayDigitXPos = (int)(0.5f * GameConstants.ViewportWidth);   // display digits in center of screen
            SetUpDigitContants();
            SetUpDigitPositions();
        }

        /// <summary>
        /// Updates the player's health when a collision occurs based on the speed the player
        /// was traveling when the collision occured
        /// </summary>
        /// <param name="velocity">Velocity of the player</param>
        /// <param name="gameState">The state of the game</param>
        /// <remarks>
        /// This method should be called when a collision occurs.
        /// If the player runs out of health, the game will transition to the end state
        /// </remarks>
        public void HitBarrier(float velocity, ref GameConstants.GameState gameState)
        {
            float speed = Math.Abs(velocity);

            if (speed > Player.MaxSpeed * Player.MajorCrashPercentMaxSpeed)
            {
                digits -= 10;
            }
            else if (speed > Player.MaxSpeed * Player.MinorCrashPercentMaxSpeed)
            {
                digits -= 5;
            }

            if (digits <= 0)
            {
                digits = 0;
                gameState = GameConstants.GameState.End;
            }
        }

        /// <summary>
        /// Update the player's heatlh when the player has been hit by a missile
        /// </summary>
        /// <param name="gameState">The current state of the game</param>
        /// <remarks>
        /// If the player's health drop's below zero, the game will transition to the 
        /// end state.
        /// </remarks>
        public void HitByMissile(ref GameConstants.GameState gameState)
        {
            digits -= 10;

            if (digits <= 0)
            {
                digits = 0;
                gameState = GameConstants.GameState.End;
            }
        }

        /// <summary>
        /// Reset the health meter to the state it was before play
        /// </summary>
        public void Reset()
        {
            digits = Player.StartingHealth;
        }
    }
}
