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
    /// Abstract class which includes members and functionality common to all ingame
    /// text displays (e.g. GameOverScreen).
    /// </summary>
    public abstract class GameTextScreen
    {
        /// <summary>Used to draw the text</summary>
        protected SpriteBatch spriteBatch;
        /// <summary>Large size font</summary>
        protected SpriteFont largeFont;
        /// <summary>Medium size font</summary>
        protected SpriteFont mediumFont;
        /// <summary>Used to measure sizes of text</summary>
        protected Vector2 textSize; 

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
