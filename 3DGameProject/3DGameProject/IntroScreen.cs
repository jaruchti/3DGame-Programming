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
    /// Class which implements the logic to display relevant text to the screen during
    /// the introduction (e.g. game name, instructions).
    /// </summary>
    /// <remarks>This is a singleton in the game</remarks>
    class IntroScreen : GameTextScreen
    {
        Vector2 titlePosition;          // position to display game title
        Vector2 instructionsPosition;   // position to dislay instructions

        String strTitle = "Alien Attack";
        String strInstructions = "Press Space to Continue";

        /// <summary>
        /// Load the content required for the screen.
        /// </summary>
        /// <param name="device">Graphics card (to initialize spritebatch)</param>
        /// <param name="content">Content pipeline (for fonts)</param>
        public override void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            base.LoadContent(ref device, content);

            // place title slightly below center of screen
            textSize = largeFont.MeasureString(strTitle);
            titlePosition = new Vector2(250 - textSize.X / 2, 300);

            // place instructions at bottom of screen
            textSize = mediumFont.MeasureString(strInstructions);
            instructionsPosition = new Vector2(250 - textSize.X / 2, 500 - largeFont.LineSpacing);  
        }

        /// <summary>
        /// Draw game title, instuctions to the screen
        /// </summary>
        public void Draw()
        {
            spriteBatch.Begin();

            spriteBatch.DrawString(largeFont, strTitle, titlePosition, Color.Orange);
            spriteBatch.DrawString(mediumFont, strInstructions, instructionsPosition, Color.LightGray);

            spriteBatch.End();
        }
    }
}
