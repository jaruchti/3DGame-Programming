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
    class GameOverScreen : GameTextScreen
    {
        Vector2 gameOverPosition;
        Vector2 rankPosition;
        Vector2 instructionsPosition;

        String strGameOver = "Game Over";
        String strRank = "";
        String strInstructions = "Press Space to Continue";

        public void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            base.LoadContent(ref device, content);

            spriteBatch = new SpriteBatch(device);
            largeFont = content.Load<SpriteFont>("Fonts/LargeFont");
            mediumFont = content.Load<SpriteFont>("Fonts/MediumFont");

            textSize = largeFont.MeasureString(strGameOver);
            gameOverPosition = new Vector2(250 - textSize.X / 2, 300);

            rankPosition = new Vector2(0, 0);

            textSize = mediumFont.MeasureString(strInstructions);
            instructionsPosition = new Vector2(250 - textSize.X / 2, 500 - informationFont.LineSpacing);
        }

        public void Update(float score)
        {
            if (score <= GameConstants.AmatuerAbductee)
                strRank = "Amatuer Abductee";
            else if (score <= GameConstants.MediocreAbductee)
                strRank = "Mediocre Abductee";
            else if (score <= GameConstants.Abductee)
                strRank = "Abductee";
            else
                strRank = "Master Abductee";

            textSize = mediumFont.MeasureString(strRank);
            rankPosition = new Vector2(250 - textSize.X / 2, gameOverPosition.Y + largeFont.LineSpacing);
        }

        public void Draw()
        {
            spriteBatch.Begin();

            spriteBatch.DrawString(largeFont, strGameOver, gameOverPosition, Color.Orange);
            spriteBatch.DrawString(mediumFont, strRank, rankPosition, Color.LightGray);
            spriteBatch.DrawString(mediumFont, strInstructions, instructionsPosition, Color.LightGray);

            spriteBatch.End();
        }
    }
}
