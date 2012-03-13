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
    /// Abstract class which implements functionality common to all of the in-game displays
    /// (e.g. Score, Fuel Gauge, etc.).
    /// </summary>
    abstract class InGameDisplay
    {
        /// <summary>x position of the left side of the first digit in the ingame texture</summary>
        public const int TextureDigitXPos = 0;
        /// <summary>y position of the top of the digits in the ingame texture</summary>
        public const int TextureDigitYPos = 344;
        /// <summary>width of the digits in the ingame texture</summary>
        public const int TextureDigitWidth = 83;
        /// <summary>height of the digits in the ingame texture</summary>
        public const int TextureDigitHeight = 129;

        protected int DisplayNumDigits;     // number of digits to display
        protected int DisplayDigitXPos;     // x position of left side of the first digit to display
        protected int DisplayDigitYPos;     // y position of the top of the digits in the display
        protected float DisplayDigitScale;  // scale to use in displaying digits
        protected int DisplayDigitWidth;    // width between left and right side of a single digit in display

        protected float digits;             // digits to display

        protected SpriteBatch spriteBatch;  // used to draw the digits
        protected Texture2D ingameTextures; // holds the texture with the images to display

        protected Rectangle textureRect;    // holds the section of ingameTexture to display 
        protected Rectangle displayDrawRect;// holds the position in the display to draw textureRect

        protected Vector2[] digitPositions; // positions to draw the numbers in the digits variable
                                            // this field is calculated from the information provided
                                            // by the above variables
        
        /// <summary>
        /// Determine where to draw the digits on the screen and store in digitPositions array. 
        /// </summary>
        protected void SetUpDigitPositions()
        {
            digitPositions = new Vector2[DisplayNumDigits];

            for (int i = 0; i < DisplayNumDigits; i++)
            {
                digitPositions[i] = new Vector2(DisplayDigitXPos + i * DisplayDigitWidth, DisplayDigitYPos);
            }
        }

        /// <summary>
        /// Load the content required for an InGameDisplay (namely the ingame texture)
        /// </summary>
        /// <param name="device">Graphics card (to initialize spritebatch)</param>
        /// <param name="content">Content pipeline (for texture)</param>
        public void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            ingameTextures = content.Load<Texture2D>("Textures/ingame");
            spriteBatch = new SpriteBatch(device);
        }

        /// <summary>
        /// All non-abstract classes should implement an Update function to make modifications
        /// to the digits they are displaying.
        /// </summary>
        /// <param name="newDigitVal">New digit to display</param>
        abstract public void Update(float newDigitVal);

        /// <summary>
        /// Draw the InGameDisplay to the screen.
        /// </summary>
        public void Draw()
        {
            spriteBatch.Begin();

            // Draw the background texture
            spriteBatch.Draw(ingameTextures, displayDrawRect, textureRect, Color.White);

            // Draw the digits
            for (int i = 0; i < DisplayNumDigits; i++)
            {
                // Note: ((int)digits / (int)(Helpers.Pow(10, i)) % 10 is used to extract
                // a single digit from digit.  Each iteration returns a digit starting
                // with the the rightmost (hence why we display the rightmost digit first)

                spriteBatch.Draw(ingameTextures, digitPositions[DisplayNumDigits - 1 - i],
                    GetDigitRect(((int)digits / (int)(Helpers.Pow(10, i)) % 10)),
                    Color.White, 0.0f, new Vector2(0, 0),
                    DisplayDigitScale,
                    SpriteEffects.None,
                    0.0f);
            }

            spriteBatch.End();
        }

        /// <summary>
        /// Returns a rectangle containing the texture for the 
        /// digit given as parameter.
        /// </summary>
        /// <param name="digit">An integer 0-9</param>
        public static Rectangle GetDigitRect(int digit)
        {
            return new Rectangle(TextureDigitXPos + digit * TextureDigitWidth,
                TextureDigitYPos,
                TextureDigitWidth,
                TextureDigitHeight);
        }
    }
}
