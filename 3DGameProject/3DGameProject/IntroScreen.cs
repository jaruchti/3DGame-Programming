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
    class IntroScreen : GameTextScreen
    {
        Vector2 titlePosition;
        Vector2 instructionsPosition;

        String strTitle = "Alien Attack";
        String strInstructions = "Press Space to Continue";

        public override void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            base.LoadContent(ref device, content);

            textSize = largeFont.MeasureString(strTitle);
            titlePosition = new Vector2(250 - textSize.X / 2, 300);

            textSize = mediumFont.MeasureString(strInstructions);
            instructionsPosition = new Vector2(250 - textSize.X / 2, 500 - largeFont.LineSpacing);
        }

        public void Draw()
        {
            spriteBatch.Begin();

            spriteBatch.DrawString(largeFont, strTitle, titlePosition, Color.Orange);
            spriteBatch.DrawString(mediumFont, strInstructions, instructionsPosition, Color.LightGray);

            spriteBatch.End();
        }
    }
}
