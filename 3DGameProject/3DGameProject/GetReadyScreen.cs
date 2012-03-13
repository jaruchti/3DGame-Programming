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
    class GetReadyScreen : GameTextScreen
    {
        private float seconds;
        private const int SecondsOnScreen = 4;

        private Vector2 countdownPosition;
        private Vector2 instructionsPosition;

        private String strCountdown = "";
        private String strInstructions = "Get Ready";

        public override void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            base.LoadContent(ref device, content);

            textSize = mediumFont.MeasureString(strInstructions);
            instructionsPosition = new Vector2(250 - textSize.X / 2, 500 - largeFont.LineSpacing);
        }

        public void Update(float elapsedSeconds, ref GameConstants.GameState gameState)
        {
            seconds += elapsedSeconds;

            strCountdown = Convert.ToString((int)(SecondsOnScreen - seconds + 1)); 

            textSize = largeFont.MeasureString(strCountdown);
            countdownPosition = new Vector2(250 - textSize.X / 2, 300);

            if (seconds > SecondsOnScreen)
            {
                seconds = 0;
                gameState = GameConstants.GameState.Playing;
            }
        }

        public void Draw()
        {
            spriteBatch.Begin();

            spriteBatch.DrawString(largeFont, strCountdown, countdownPosition, Color.Orange);
            spriteBatch.DrawString(mediumFont, strInstructions, instructionsPosition, Color.LightGray);

            spriteBatch.End();
        }
    }
}
