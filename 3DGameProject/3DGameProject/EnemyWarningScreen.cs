/*
 * 3D Game Programming Project
 * Dr. Liu
 * Zach Bates, Lauren Buss, Corey Darr, Jason Ruchti, Jared Tittle
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace _3DGameProject
{
    /// <summary>
    /// Class which implements the logic to display a warning to the screen to inform
    /// the player of enemy positions
    /// </summary>
    /// <remarks>This is a singlton in the game</remarks>
    public class EnemyWarningScreen : GameTextScreen
    {
        private string strWarning = "";     // warning message
        private Vector2 warningPosition;    // where to put the message 
        private Color displayColor;         // what color to display the message in
        private bool flashing = false;      // should the text be flashing?

        /// <summary>
        /// Load the content for the screen
        /// </summary>
        /// <param name="device">Graphics device (to initialize spritebatch)</param>
        /// <param name="content">Content pipeline (for fonts)</param>
        public override void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            base.LoadContent(ref device, content);
        }

        /// <summary>
        /// Set the position of the elements of the EnemyWarningScreen based on the characteristics
        /// of the viewport
        /// </summary>
        public override void SetPosition()
        {
            textSize = largeFont.MeasureString(strWarning);

            // position warning slightly below center of screen
            warningPosition = new Vector2(
                (int)(GameConstants.ViewportWidth / 2) - textSize.X / 2,
                (int)(0.6f * GameConstants.ViewportHeight));
        }

        /// <summary>
        /// Update the warning string with new state information
        /// </summary>
        /// <param name="strWarning">New warning message</param>
        /// <param name="displayColor">New display color</param>
        /// <param name="flashing">Should the message flash on and off?</param>
        public void Update(string strWarning, Color displayColor, bool flashing)
        {
            this.strWarning = strWarning;
            this.displayColor = displayColor;
            this.flashing = flashing;

            SetPosition();
        }

        /// <summary>
        /// Reset the warning message to the blank string
        /// </summary>
        public void Reset()
        {
            strWarning = "";
        }

        /// <summary>
        /// Draw the warning message to the screen
        /// </summary>
        /// <param name="gameTime">Gives information on elapsed time for flashing display</param>
        public void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            if (flashing)
            {
                // flash text twice per second
                if (gameTime.TotalGameTime.TotalMilliseconds % 500 < 250)
                    spriteBatch.DrawString(largeFont, strWarning, warningPosition, displayColor);
            }
            else
                spriteBatch.DrawString(largeFont, strWarning, warningPosition, displayColor);

            spriteBatch.End();
        }
    }
}
