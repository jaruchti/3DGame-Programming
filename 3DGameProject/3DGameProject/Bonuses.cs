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
    /// Wrapper class to manage the bonuses in the game
    /// </summary>
    /// <remarks>This is a singleton in the game</remarks>
    public class Bonuses
    {
        /// <summary>Number of bonuses for the player to pick up</summary>
        public const int NumBonuses = 5;
        private Bonus[] bonuses; // array with bonuses for the player to pick up
        BonusScreen bonusScreen; // screen to display to the user when they pick up a bonus

        /// <summary>
        /// Constructor to initialize the fields of the bonuses class
        /// </summary>
        public Bonuses()
        {
            bonusScreen = new BonusScreen();
            bonuses = new Bonus[NumBonuses];
        }

        /// <summary>
        /// Load the bonus models, bonus screen, and place the bonuses in random locations
        /// </summary>
        /// <param name="device">Graphics device (to initialize spritebatch)</param>
        /// <param name="content">Content pipeline (for models)</param>
        /// <param name="m">Map to place the bonuses randomly into</param>
        public void LoadContent(ref GraphicsDevice device, ContentManager content, Map m)
        {
            // load models
            for (int i = 0; i < bonuses.Length; i++)
            {
                bonuses[i] = new Bonus();
                bonuses[i].LoadContent(ref device, content);
            }

            // place the bonuses in random locations
            foreach (Bonus b in bonuses)
            b.PlaceRandomly(m);

            // Load the content to display the text "Bonus" when the player picks up a bonus
            bonusScreen.LoadContent(ref device, content);
        }

        /// <summary>
        /// Allows the client to use a foreach construct to iterate through the bonuses
        /// </summary>
        /// <returns>IEnumerator for bonus objects</returns>
        public IEnumerator<Bonus> GetEnumerator()
        {
            foreach (Bonus b in bonuses)
                yield return b;
        }

        /// <summary>
        /// This method is used to set the bonus screen to display when the player has picked
        /// up a bonus
        /// </summary>
        /// <remarks>
        /// This method should be called when the player is involved in a collision with one
        /// of the bonus objects.
        /// </remarks>
        public void BonusPickedUp()
        {
            bonusScreen.Display = true;
        }

        /// <summary>
        /// Update the bonus screen with the number of seconds since the last update
        /// </summary>
        /// <param name="elapsedSeconds">Number of seconds since the last update</param>
        /// <remarks>
        /// When the desired timespan has elapsed, the bonus screen will automatically
        /// transition to no longer display.
        /// </remarks>
        public void Update(float elapsedSeconds)
        {
            bonusScreen.Update(elapsedSeconds);
        }

        /// <summary>
        /// Draw the bonus models
        /// </summary>
        /// <param name="gameCamera">For view and projection matrices</param>
        public void DrawBonuses(Camera gameCamera)
        {
            foreach (Bonus b in bonuses)
                b.Draw(gameCamera);
        }

        /// <summary>
        /// Draw the bonus screen to the screen
        /// </summary>
        /// <remarks>
        /// The details of drawing the bonus screen are managed by another class.
        /// If the screen no longer needs to be displayed (it has been shown long enough),
        /// no action will be taken.
        /// </remarks>
        public void DrawBonusScreen()
        {
            bonusScreen.Draw();
        }

        /// <summary>
        /// Reset to the state before gameplay began - namely, reset the bonus screen to off.
        /// </summary>
        public void Reset()
        {
            bonusScreen.Reset();
        }
    }
}
