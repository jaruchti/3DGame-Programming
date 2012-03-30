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
    /// Class which implements the relevant logic to display a message to the player
    /// when they acquire a bonus.
    /// </summary>
    public class BonusScreen : GameTextScreen
    {
        /// <summary>Number of seconds the screen should display before disappearing</summary>
        public const int SecondsOnScreen = 2;

        private float seconds = 0.0f;       // number seconds the bonus has been displayed
        private Vector2 bonusPosition;      // position of the text on the screen  
        private String strBonus = "Bonus";  // text to display to the screen
        private bool isDisplayed = false;   // boolean to determine whether or not to display the text to the screen

        /// <summary>
        /// Property to set the isDisplayed variable.  When the Display property is set to true, the 
        /// screen is drawn and countdown for the number of seconds to display the screen begins
        /// </summary>
        public bool Display
        {
            get { return isDisplayed; }
            set { isDisplayed = value; }
        }

        /// <summary>
        /// Load the content required for the screen
        /// </summary>
        /// <param name="device">Graphics card (to initialize spritebatch)</param>
        /// <param name="content">Content pipeline (for fonts)</param>
        public override void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            base.LoadContent(ref device, content);
            SetPosition();
        }

        /// <summary>
        /// Set the position of the elements of the BonusScreen based on the characteristics
        /// of the viewport
        /// </summary>
        public override void SetPosition()
        {
            textSize = largeFont.MeasureString(strBonus);

            // position bonus at 20% from top of screen
            bonusPosition = new Vector2(
                (int)(GameConstants.ViewportWidth / 2) - textSize.X / 2,
                (int)(0.2f * GameConstants.ViewportHeight));
        }


        /// <summary>
        /// Update the bonus screen based on the number of seconds that have been elapsed
        /// </summary>
        /// <param name="elapsedSeconds"></param>
        /// <remarks>
        /// If the text has been shown for long enough, transition isDisplayed
        /// to false to avoid displaying the bonus message any longer
        /// </remarks>
        public void Update(float elapsedSeconds)
        {
            if (isDisplayed == true)
            {
                seconds += elapsedSeconds;

                // check if the display has been shown for long enough
                if (seconds > SecondsOnScreen)
                {
                    seconds = 0.0f;
                    isDisplayed = false;
                }
            }
        }

        /// <summary>
        /// Display the text to the screen to alert the player of the bonus
        /// </summary>
        public void Draw()
        {
            if (isDisplayed) // check if we should still display before drawing
            {
                spriteBatch.Begin();

                spriteBatch.DrawString(largeFont, strBonus, bonusPosition, Color.Orange);

                spriteBatch.End();
            }
        }

        /// <summary>
        /// Reset the bonus screen to the state when gameplay first began
        /// </summary>
        public void Reset()
        {
            seconds = 0.0f;
            isDisplayed = false;
        }
    }
}
