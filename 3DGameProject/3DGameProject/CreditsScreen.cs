﻿/*
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
    /// Class which allows the client to display the credits screen for the Alien Attack
    /// game.
    /// </summary>
    /// <remarks>This is a singleton in the game</remarks>
    public class CreditsScreen
    {
        private Texture2D background;
        private SpriteBatch spriteBatch;
        private Rectangle viewportRect;

        /// <summary>
        /// Load the content required for the CreditsScreen
        /// </summary>
        /// <param name="device">Graphics card (to initialize spritebatch)</param>
        /// <param name="content">Content pipeline (for texture)</param>
        public void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            spriteBatch = new SpriteBatch(device);
            background = content.Load<Texture2D>("Textures/backgroundCredits");
            SetPosition();
        }

        /// <summary>
        /// Set the position of the Credits screen so that it fills the entire viewport
        /// </summary>
        public void SetPosition()
        {
            viewportRect = new Rectangle(0, 0,
                GameConstants.ViewportWidth,
                GameConstants.ViewportHeight);
        }

        /// <summary>
        /// Draw the Credits screen.
        /// </summary>
        public void Draw()
        {
            spriteBatch.Begin();

            spriteBatch.Draw(background, viewportRect, Color.White);

            spriteBatch.End();
        }
    }
}
