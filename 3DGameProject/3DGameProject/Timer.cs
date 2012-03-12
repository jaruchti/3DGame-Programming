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
        private SpriteBatch spriteBatch;
        private Texture2D spedometer;

        private Rectangle textureRect = new Rectangle(382, 2, 339, 60);
        private Rectangle displayDrawRect = new Rectangle(0, 0, 180, 30);
        private Vector2[] digitPositions = { new Vector2(70, 2),
                                             new Vector2(90, 2),
                                             new Vector2(110, 2),
                                             new Vector2(130, 2),
                                             new Vector2(150, 2)
                                           };

        public void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            spedometer = content.Load<Texture2D>("Textures/ingame");
            spriteBatch = new SpriteBatch(device);
        }

        public void Draw(int secs)
        {
            if (secs > GameConstants.MaxTime)
                secs = GameConstants.MaxTime;

            spriteBatch.Begin();

            spriteBatch.Draw(spedometer, displayDrawRect, textureRect, Color.White);

            for (int i = 0; i < digitPositions.Length; i++)
            {

                spriteBatch.Draw(spedometer, digitPositions[digitPositions.Length - 1 - i], 
                    Helpers.GetDigitRect((secs / (int) (Helpers.Pow(10, i)) % 10)), 
                    Color.White, 0.0f, new Vector2(0, 0), 
                    0.2f, 
                    SpriteEffects.None, 
                    0.0f);
            }
   
            spriteBatch.End();
        }
    }
}
