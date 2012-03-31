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
    /// Abstract class which implements functionality common to all of the in-game displays
    /// (e.g. Score, Fuel Gauge, etc.).
    /// </summary>
    public abstract class InGameDisplay
    {
        /// <summary>x position of the left side of the first digit in the ingame texture</summary>
        public const int TextureDigitXPos = 0;
        /// <summary>y position of the top of the digits in the ingame texture</summary>
        public const int TextureDigitYPos = 344;
        /// <summary>width of the digits in the ingame texture</summary>
        public const int TextureDigitWidth = 83;
        /// <summary>height of the digits in the ingame texture</summary>
        public const int TextureDigitHeight = 129;

        /// <summary>Number of digits to display</summary>
        protected int DisplayNumDigits;
        /// <summary>x position of left side of the first digit to display</summary>
        protected int DisplayDigitXPos;
        /// <summary>y position of the top of the digits in the display</summary>
        protected int DisplayDigitYPos;
        /// <summary>Scale to use in displaying digits</summary>
        protected float DisplayDigitScale;
        /// <summary>Width between left and right side of a single digit in display</summary>
        protected int DisplayDigitWidth;

        /// <summary>Digits to display</summary>
        protected float digits;

        private SpriteBatch spriteBatch;    // used to draw the digits
        private Texture2D ingameTextures;   // holds the texture with the images to display

        /// <summary>Holds the section of ingameTexture to display </summary>
        protected Rectangle textureRect;
        /// <summary>Holds the position in the display to draw textureRect</summary>
        protected Rectangle displayDrawRect;

        /// <summary>
        /// Positions to draw the individual digits in the digits variable
        /// </summary>
        /// <remarks>
        /// This field is calculated from the information provided by the above variables
        /// </remarks>
        protected Vector2[] digitPositions; 

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
        /// This is an abstract method that all subclasses must implement to 
        /// set the position of their elements  based on the characteristics
        /// of the viewport
        /// </summary>
        public abstract void SetPosition();

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
