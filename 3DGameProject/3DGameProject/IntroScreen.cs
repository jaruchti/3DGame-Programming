/*
 * 3D Game Programming Project
 * Dr. Liu
 * Zach Bates, Lauren Buss, Corey Darr, Jason Ruchti, Jared Tittle
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace _3DGameProject
{
    /// <summary>
    /// Class which implements the logic to display relevant text to the screen during
    /// the introduction (e.g. game name, instructions).
    /// </summary>
    /// <remarks>This is a singleton in the game</remarks>
    public class IntroScreen : GameTextScreen
    {
        Vector2 titlePosition;          // position to display game title
        Vector2 instructionsPosition;   // position to dislay instructions

        String strTitle = "Alien Attack";
        String strInstructions;

        /// <summary>
        /// Create the intro screen and give instructions based on what platform
        /// the player is using
        /// </summary>
        public IntroScreen()
        {
            strInstructions = "Press A to Continue";
#if !XBOX
            strInstructions = "Press Space to Continue";
#endif
        }

        /// <summary>
        /// Load the content required for the screen.
        /// </summary>
        /// <param name="device">Graphics card (to initialize spritebatch)</param>
        /// <param name="content">Content pipeline (for fonts)</param>
        public override void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            base.LoadContent(ref device, content);
            SetPosition();
        }

        /// <summary>
        /// Set the position of the elements of the IntroScreen based on the characteristics
        /// of the viewport
        /// </summary>
        public override void SetPosition()
        {
            // place title slightly below center of screen
            textSize = largeFont.MeasureString(strTitle);
            titlePosition = new Vector2(
                (int)(GameConstants.ViewportWidth / 2) - textSize.X / 2,
                (int)(0.6f * GameConstants.ViewportHeight));

            // place instructions at bottom of screen
            textSize = mediumFont.MeasureString(strInstructions);
            instructionsPosition = new Vector2(
                (int)(GameConstants.ViewportWidth / 2) - textSize.X / 2,
                GameConstants.ViewportHeight - largeFont.LineSpacing);  
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
