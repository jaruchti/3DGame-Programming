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
    class MiniMap
    {
        private SpriteBatch spriteBatch;
        private Texture2D whiteRect;
        private Texture2D whiteSphere;

        const int xOffset = 400;
        const int yOffset = 40;
        const int rectWidth = 5;
        const int rectHeight = 5;

        public void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            whiteRect = content.Load<Texture2D>("Textures/whiterect");

            whiteSphere = content.Load<Texture2D>("Textures/whitesphere");
            spriteBatch = new SpriteBatch(device);
        }

        public void Draw(Player player, Map map)
        {
            Rectangle rect = new Rectangle();
            rect.Width = rectWidth;
            rect.Height = rectHeight;

            int[,] floorPlan = map.FloorPlan;
            Fuel[] fuelBarrels = map.FuelBarrels;

            spriteBatch.Begin();

            // draw floor plan
            for (int x = 0; x < floorPlan.GetLength(0); x++)
            {
                for (int z = 0; z < floorPlan.GetLength(1); z++)
                {
                    if (floorPlan[x, z] != 0)
                    {
                        rect.X = xOffset + rectWidth * z;
                        rect.Y = yOffset + rectHeight * x;

                        spriteBatch.Draw(whiteRect, rect, Color.Orange);
                    }
                }
            }

            // draw fuel barrels
            for (int i = 0; i < fuelBarrels.Length; i++)
            {
                rect.X = xOffset - rectWidth * (int)fuelBarrels[i].Position.Z;
                rect.Y = yOffset + rectHeight * (int)fuelBarrels[i].Position.X;
                spriteBatch.Draw(whiteSphere, rect, Color.White);
            }

            // draw player
            rect.X = xOffset - rectWidth * (int)player.Position.Z;
            rect.Y = yOffset + rectHeight * (int)player.Position.X;
            spriteBatch.Draw(whiteRect, rect, Color.Green);

            spriteBatch.End();
        }
    }
}
