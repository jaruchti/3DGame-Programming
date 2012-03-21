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
    /// Class which allows the client to display the title screen for the Alient Attack
    /// game. The title screen included developer information.
    /// </summary>
    /// <remarks>This is a singleton in the game</remarks>
    public class TitleScreen
    {
        private Texture2D background;
        private SpriteBatch spriteBatch;
        private Rectangle viewportRect;

        /// <summary>
        /// Load the content required for the TitleScreen
        /// </summary>
        /// <param name="device">Graphics card (to initialize spritebatch)</param>
        /// <param name="content">Content pipeline (for texture)</param>
        public void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            spriteBatch = new SpriteBatch(device);

#if !XBOX
            background = content.Load<Texture2D>("Textures/background");
#else
            background = content.Load<Texture2D>("Textures/backgroundXBOX");
#endif

            viewportRect = new Rectangle(0, 0, 
                device.Viewport.Width, 
                device.Viewport.Height);
        }

        /// <summary>
        /// Draw the title screen.
        /// </summary>
        public void Draw()
        {
            spriteBatch.Begin();

            spriteBatch.Draw(background, viewportRect, Color.White);

            spriteBatch.End();
        }
    }
}
