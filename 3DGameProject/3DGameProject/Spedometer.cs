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
    class Spedometer
    {
        private SpriteBatch spriteBatch;
        private Texture2D ingameTexture;

        private Rectangle textureRect = new Rectangle(0, 0, 243, 102);
        private Rectangle displayDrawRect = new Rectangle(375, 440, 125, 60);
        private Vector2 digitFirstPosition = new Vector2(435, 445);
        private Vector2 digitSecondPosition = new Vector2(468, 445);

        public void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            ingameTexture = content.Load<Texture2D>("Textures/ingame");
            spriteBatch = new SpriteBatch(device);
        }

        public void Draw(float playerVelocity)
        {
            int percentOfMaxVelocity;

            playerVelocity = Math.Abs(playerVelocity);
            percentOfMaxVelocity = (int) Math.Floor(playerVelocity / GameConstants.MaxVelocity * 100);

            if (percentOfMaxVelocity == 100) 
                percentOfMaxVelocity--;

            spriteBatch.Begin();

            spriteBatch.Draw(ingameTexture, displayDrawRect, textureRect, Color.White);

            spriteBatch.Draw(ingameTexture, digitFirstPosition, Helpers.GetDigitRect(percentOfMaxVelocity / 10), Color.White, 0.0f, new Vector2(0, 0), 0.4f, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(ingameTexture, digitSecondPosition, Helpers.GetDigitRect(percentOfMaxVelocity % 10), Color.White, 0.0f, new Vector2(0, 0), 0.4f, SpriteEffects.None, 0.0f);
            spriteBatch.End();
        }
    }
}
