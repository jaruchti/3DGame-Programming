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
    class GameOverScreen
    {
        SpriteBatch spriteBatch;
        SpriteFont gameOverFont;
        SpriteFont informationFont;

        Vector2 gameOverPosition;
        Vector2 rankPosition;
        Vector2 instructionsPosition;

        Vector2 textSize;

        String strGameOver = "Game Over";
        String strRank = "";
        String strInstructions = "Press Space to Continue";

        public void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            spriteBatch = new SpriteBatch(device);
            gameOverFont = content.Load<SpriteFont>("Fonts/GameOverFont");
            informationFont = content.Load<SpriteFont>("Fonts/Information");

            textSize = gameOverFont.MeasureString(strGameOver);
            gameOverPosition = new Vector2(250 - textSize.X / 2, 300);

            rankPosition = new Vector2(0, 0);

            textSize = informationFont.MeasureString(strInstructions);
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

            textSize = informationFont.MeasureString(strRank);
            rankPosition = new Vector2(250 - textSize.X / 2, gameOverPosition.Y + gameOverFont.LineSpacing);
        }

        public void Draw()
        {
            spriteBatch.Begin();

            spriteBatch.DrawString(gameOverFont, strGameOver, gameOverPosition, Color.Orange);
            spriteBatch.DrawString(informationFont, strRank, rankPosition, Color.LightGray);
            spriteBatch.DrawString(informationFont, strInstructions, instructionsPosition, Color.LightGray);

            spriteBatch.End();
        }

        //void stuff()
        //{
        //    xOffsetText = yOffsetText = 0;
        //    Vector2 strInstructionsSize =
        //        statsFont.MeasureString(GameConstants.StrInstructions1);
        //    Vector2 strPosition;
        //    strCenter = new Vector2(strInstructionsSize.X / 2,
        //        strInstructionsSize.Y / 2);

        //    yOffsetText = (viewportSize.Y / 2 - strCenter.Y);
        //    xOffsetText = (viewportSize.X / 2 - strCenter.X);
        //    strPosition = new Vector2((int)xOffsetText, (int)yOffsetText);

        //    spriteBatch.Begin();
        //    spriteBatch.DrawString(statsFont, GameConstants.StrInstructions1,
        //        strPosition, Color.White);

        //    strInstructionsSize =
        //        statsFont.MeasureString(GameConstants.StrInstructions2);
        //    strCenter = new Vector2(strInstructionsSize.X / 2,
        //        strInstructionsSize.Y / 2);
        //    yOffsetText =
        //        (viewportSize.Y / 2 - strCenter.Y) + statsFont.LineSpacing;
        //    xOffsetText = (viewportSize.X / 2 - strCenter.X);
        //    strPosition = new Vector2((int)xOffsetText, (int)yOffsetText);

        //    spriteBatch.DrawString(statsFont, GameConstants.StrInstructions2,
        //        strPosition, Color.LightGray);
        //    spriteBatch.End();

        //    //re-enable depth buffer after sprite batch disablement

        //    //GraphicsDevice.DepthStencilState.DepthBufferEnable = true;
        //    DepthStencilState dss = new DepthStencilState();
        //    dss.DepthBufferEnable = true;
        //    GraphicsDevice.DepthStencilState = dss;

        //    //GraphicsDevice.RenderState.AlphaBlendEnable = false;
        //    //GraphicsDevice.RenderState.AlphaTestEnable = false;

        //    //GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
        //    //GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;
        //}
    }
}
