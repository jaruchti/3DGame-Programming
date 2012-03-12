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
    abstract class InGameDisplay
    {
        protected int NumDisplayDigits;
        protected int FirstDigitXOffset;
        protected int DigitYOffset;
        protected float DigitScale;
        protected int DigitWidth = 83;

        protected float digits;

        protected SpriteBatch spriteBatch;
        protected Texture2D ingameTextures;

        protected Rectangle textureRect;
        protected Rectangle displayDrawRect;
        protected Vector2[] digitPositions;

        protected void SetUpDigitPositions()
        {
            digitPositions = new Vector2[NumDisplayDigits];

            for (int i = 0; i < NumDisplayDigits; i++)
            {
                digitPositions[i] = new Vector2(FirstDigitXOffset + i * DigitWidth, DigitYOffset);
            }
        }

        public void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            ingameTextures = content.Load<Texture2D>("Textures/ingame");
            spriteBatch = new SpriteBatch(device);
        }

        abstract public void Update(float newDigitVal);

        public void Draw()
        {
            spriteBatch.Begin();

            spriteBatch.Draw(ingameTextures, displayDrawRect, textureRect, Color.White);

            for (int i = 0; i < NumDisplayDigits; i++)
            {

                spriteBatch.Draw(ingameTextures, digitPositions[NumDisplayDigits - 1 - i],
                    Helpers.GetDigitRect(((int)digits / (int)(Helpers.Pow(10, i)) % 10)),
                    Color.White, 0.0f, new Vector2(0, 0),
                    DigitScale,
                    SpriteEffects.None,
                    0.0f);
            }

            spriteBatch.End();
        }
    }
}
