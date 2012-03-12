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
    class ScoreDisplay
    {
        protected int DisplayDigits;
        protected int FirstDigitXOffset;
        protected int DigitYOffset;
        protected int DigitWidth;

        protected float score;
        protected SpriteBatch spriteBatch;
        protected Texture2D ingameTextures;

        protected Rectangle textureRect;
        protected Rectangle displayDrawRect;
        protected Vector2[] digitPositions;

        protected void SetUpDigitPositions()
        {
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

        public float Score { 
            get { return score; }
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
