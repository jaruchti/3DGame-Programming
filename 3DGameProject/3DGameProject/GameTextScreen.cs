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
    /// Abstract class which includes members and functionality common to all ingame
    /// text displays (e.g. GameOverScreen).
    /// </summary>
    public abstract class GameTextScreen
    {
        protected SpriteBatch spriteBatch;  // used to draw the text
        protected SpriteFont largeFont;     // large size font
        protected SpriteFont mediumFont;    // medium size font
        protected Vector2 textSize;         // used to measure sizes of text

        /// <summary>
        /// Load the content required for a GameTextScreen
        /// </summary>
        /// <param name="device">Graphics card (to initialize spritebatch)</param>
        /// <param name="content">Content pipeline (for fonts)</param>
        public virtual void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            spriteBatch = new SpriteBatch(device);

            largeFont = content.Load<SpriteFont>("Fonts/LargeFont");
            mediumFont = content.Load<SpriteFont>("Fonts/MediumFont");
        }
    }
}
