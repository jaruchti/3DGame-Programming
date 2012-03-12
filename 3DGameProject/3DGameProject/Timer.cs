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
    class Timer
    {
        private readonly int DisplayDigits;
        private readonly int FirstDigitXOffset;
        private readonly int DigitYOffset;
        private readonly int DigitWidth;

        private float score;
        private SpriteBatch spriteBatch;
        private Texture2D ingameTextures;

        private Rectangle textureRect;
        private Rectangle displayDrawRect;
        private Vector2[] digitPositions;

        public Timer()
        {
            DisplayDigits = 5;
            FirstDigitXOffset = 70;
            DigitYOffset = 2;
            DigitWidth = 20;

            textureRect = new Rectangle(382, 2, 339, 60);
            displayDrawRect = new Rectangle(0, 0, 180, 30);
            digitPositions = new Vector2[DisplayDigits];

            for (int i = 0; i < DisplayDigits; i++)
            {
                digitPositions[i] = new Vector2(FirstDigitXOffset + i * DigitWidth, DigitYOffset);
            }
        }

        public void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            ingameTextures = content.Load<Texture2D>("Textures/ingame");
            spriteBatch = new SpriteBatch(device);
        }

        public int Score { get { return (int) score; } }

        public void Update(float elapsedSecs)
        {
            score += elapsedSecs;
            if (score > GameConstants.MaxTime)
                score = GameConstants.MaxTime;
        }
        public void Draw()
        {
            spriteBatch.Begin();

            spriteBatch.Draw(ingameTextures, displayDrawRect, textureRect, Color.White);

            for (int i = 0; i < DisplayDigits; i++)
            {

                spriteBatch.Draw(ingameTextures, digitPositions[DisplayDigits - 1 - i], 
                    Helpers.GetDigitRect(((int) score / (int) (Helpers.Pow(10, i)) % 10)), 
                    Color.White, 0.0f, new Vector2(0, 0), 
                    0.2f, 
                    SpriteEffects.None, 
                    0.0f);
            }
   
            spriteBatch.End();
        }

        public void Reset()
        {
            score = 0.0f;
        }
    }
}
