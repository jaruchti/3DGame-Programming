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
    /// Class which implements the logic to display relevant text to the screen when
    /// the game is over (e.g. Game Over, player rank, instructions on what to do next).
    /// </summary>
    /// <remarks>This is a singleton in the game</remarks>
    public class GameOverScreen : GameTextScreen
    {
        /// <summary>Score needed for title "AmatuerAbductee"</summary>
        public const int AmatuerAbductee = 25;
        /// <summary>Score needed for title "MediocreAbductee"</summary>
        public const int MediocreAbductee = 100;
        /// <summary>Score needed for title "Abductee"</summary>
        public const int Abductee = 200;
        /// <summary>Score needed for title "MasterAbductee"</summary>
        public const int MasterAbductee = 500;

        Vector2 gameOverPosition;       // position to display strGameOver
        Vector2 rankPosition;           // position to display strRank
        Vector2 instructionsPosition;   // position to display strInstructions

        String strGameOver = "Game Over";
        String strRank = "";
        String strInstructions = "Press Space to Continue";

        /// <summary>
        /// Create the game over screen and give instructions based on the platform the player
        /// is using
        /// </summary>
        public GameOverScreen()
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
        /// Set the position of the elements of the GameOver display based on the characteristics
        /// of the viewport
        /// </summary>
        public override void SetPosition()
        {
            textSize = largeFont.MeasureString(strGameOver);

            // position gameOver slightly below center of screen
            gameOverPosition = new Vector2(
                (int)(GameConstants.ViewportWidth / 2) - textSize.X / 2,
                (int)(0.6f * GameConstants.ViewportHeight));

            // position rank directly below strGameOver
            textSize = mediumFont.MeasureString(strRank);
            rankPosition = new Vector2(
                (int)(GameConstants.ViewportWidth / 2) - textSize.X / 2,
                gameOverPosition.Y + largeFont.LineSpacing);

            // position instructions at bottom of screen just above health meter
            textSize = mediumFont.MeasureString(strInstructions);
            instructionsPosition = new Vector2(
                (int)(GameConstants.ViewportWidth / 2) - textSize.X / 2,
                GameConstants.ViewportHeight - 2 * largeFont.LineSpacing);
        }

        /// <summary>
        /// Update the player's rank based on the score.
        /// </summary>
        /// <param name="score"></param>
        public void Update(float score)
        {
            if (score <= AmatuerAbductee)
                strRank = "Amatuer Abductee";
            else if (score <= MediocreAbductee)
                strRank = "Mediocre Abductee";
            else if (score <= Abductee)
                strRank = "Abductee";
            else
                strRank = "Master Abductee";

            // position rank directly below strGameOver
            textSize = mediumFont.MeasureString(strRank);
            rankPosition = new Vector2(
                (int) (GameConstants.ViewportWidth / 2) - textSize.X / 2, 
                gameOverPosition.Y + largeFont.LineSpacing);
        }

        /// <summary>
        /// Draw Game Over, the player's rank, and instructions to the screen.
        /// </summary>
        public void Draw()
        {
            spriteBatch.Begin();

            spriteBatch.DrawString(largeFont, strGameOver, gameOverPosition, Color.Orange);
            spriteBatch.DrawString(mediumFont, strRank, rankPosition, Color.LightGray);
            spriteBatch.DrawString(mediumFont, strInstructions, instructionsPosition, Color.LightGray);

            spriteBatch.End();
        }
    }
}
