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
    class FuelGauge
    {
        private SpriteBatch spriteBatch;
        private Texture2D spedometer;

        private Rectangle displayTextureRect = new Rectangle(0, 103, 243, 102);
        private Rectangle displayDrawRect = new Rectangle(0, 440, 125, 60);
        private Vector2 digitFirstPosition = new Vector2(0, 445);
        private Vector2 digitSecondPosition = new Vector2(33, 445);

        public void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            spedometer = content.Load<Texture2D>("Textures/ingame");
            spriteBatch = new SpriteBatch(device);
        }

        public void Draw(float playerFuel)
        {
            int fuelDisplay = (int) Math.Floor(playerFuel);

            spriteBatch.Begin();

            spriteBatch.Draw(spedometer, displayDrawRect, displayTextureRect, Color.White);

            spriteBatch.Draw(spedometer, digitFirstPosition, Helpers.GetDigitRect(fuelDisplay / 10), Color.White, 0.0f, new Vector2(0, 0), 0.4f, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(spedometer, digitSecondPosition, Helpers.GetDigitRect(fuelDisplay % 10), Color.White, 0.0f, new Vector2(0, 0), 0.4f, SpriteEffects.None, 0.0f);
            
            spriteBatch.End();
        }
    }
}
