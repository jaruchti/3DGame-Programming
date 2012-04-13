/*
 * 3D Game Programming Project
 * Dr. Liu
 * Zach Bates, Lauren Buss, Corey Darr, Jason Ruchti, Jared Tittle
 */

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace _3DGameProject
{
    /// <summary>
    /// Class which implements the logic to display a mini-map with the city layout, 
    /// the player's position, enemy positions, etc. the screen.
    /// </summary>
    /// <remarks>This is a singleton in the game</remarks>
    /// 
    public class MiniMap
    {
        private SpriteBatch spriteBatch;
        private Texture2D whiteRect;        // holds a white rectangle texture
        private Texture2D whiteCircle;      // holds a white circle texture

        /// <summary>percentage xOffset on display to the left side of the mini-map</summary>
        public const float xOffset = 0.8f;
        /// <summary>percentage yOffset on display to the top side of the mini-map</summary>
        public const float yOffset = 0.1f;
        /// <summary>width of each item in the mini-map as a percentage of the viewport</summary>
        public const float rectWidth = 0.01f;
        /// <summary>height of each item in the mini-map as a percentage of the viewport</summary>
        public const float rectHeight = 0.01f;

        /// <summary>
        /// Load the content required for the minimap.
        /// </summary>
        /// <param name="device">Graphics card (to initialize spritebatch)</param>
        /// <param name="content">Content pipeline (for texture)</param>
        public void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            whiteRect = content.Load<Texture2D>("Textures/whiterect");

            whiteCircle = content.Load<Texture2D>("Textures/whitecircle");
            spriteBatch = new SpriteBatch(device);
        }


        /// <summary>
        /// Draw the minimap to the screen
        /// </summary>
        /// <param name="gameTime">Gives information on elapsed time for flashing enemies</param>
        /// <param name="player">The position of player is needed for minimap</param>
        /// <param name="enemies">The position of enemies is needed for minimap</param>
        /// <param name="map">The position of the fuel barrels and building is needed for minimap</param>
        public void Draw(GameTime gameTime, Player player, Enemies enemies, Map map)
        {
            // this rectangle describes the position on screen where an object will be drawn
            Rectangle rect = new Rectangle();   
            rect.Width = (int) Math.Round((rectWidth * GameConstants.ViewportWidth));
            rect.Height = (int) Math.Round((rectHeight * GameConstants.ViewportHeight));

            // get items to display
            int[,] floorPlan = map.FloorPlan;
            Fuel[] fuelBarrels = map.FuelBarrels;
            Bonuses bonuses = map.Bonuses;

            spriteBatch.Begin();

            // draw floor plan
            for (int x = 0; x < floorPlan.GetLength(0); x++)
            {
                for (int z = 0; z < floorPlan.GetLength(1); z++)
                {
                    if (floorPlan[x, z] != 0)
                    {
                        // move the rectangle into the correct position of the screen
                        rect.X = (int)(xOffset * GameConstants.ViewportWidth) + rect.Width * z;
                        rect.Y = (int)(yOffset * GameConstants.ViewportHeight) + rect.Height * x;

                        spriteBatch.Draw(whiteRect, rect, Color.Orange);
                    }
                }
            }

            // draw fuel barrels
            for (int i = 0; i < fuelBarrels.Length; i++)
            {
                rect.X = (int) (xOffset * GameConstants.ViewportWidth) - rect.Width * (int)fuelBarrels[i].Position.Z;
                rect.Y = (int)(yOffset * GameConstants.ViewportHeight) + rect.Height * (int)fuelBarrels[i].Position.X;
                spriteBatch.Draw(whiteCircle, rect, Color.White);
            }

            // draw bonuses
            foreach (Bonus b in bonuses)
            {
                rect.X = (int)(xOffset * GameConstants.ViewportWidth) - rect.Width * (int)b.Position.Z;
                rect.Y = (int)(yOffset * GameConstants.ViewportHeight) + rect.Height * (int)b.Position.X;
                spriteBatch.Draw(whiteCircle, rect, Color.Purple);
            }

            // draw player
            rect.X = (int)(xOffset * GameConstants.ViewportWidth) - rect.Width * (int)player.Position.Z;
            rect.Y = (int)(yOffset * GameConstants.ViewportHeight) + rect.Height * (int)player.Position.X;
            spriteBatch.Draw(whiteRect, rect, Color.Green);

            // draw enemies
            foreach (Enemy e in enemies)
            {
                if (e.Chasing == true)
                {
                    // flash twice per second if chasing
                    if (gameTime.TotalGameTime.TotalMilliseconds % 500 < 250)
                    {
                        rect.X = (int)(xOffset * GameConstants.ViewportWidth) - rect.Width * (int)e.Position.Z;
                        rect.Y = (int)(yOffset * GameConstants.ViewportHeight) + rect.Height * (int)e.Position.X;
                        spriteBatch.Draw(whiteRect, rect, Color.Red);
                    }
                }
                else
                {
                    rect.X = (int)(xOffset * GameConstants.ViewportWidth) - rect.Width * (int)e.Position.Z;
                    rect.Y = (int)(yOffset * GameConstants.ViewportHeight) + rect.Height * (int)e.Position.X;
                    spriteBatch.Draw(whiteRect, rect, Color.Red);
                }
            }

            //draw enemies next positions (for debugging)
            //foreach (Enemy e in enemies)
            //{
            //    rect.X = (int)(xOffset * GameConstants.ViewportWidth) - rect.Width * (int)e.nextPosition.Z;
            //    rect.Y = (int)(yOffset * GameConstants.ViewportHeight) + rect.Height * (int)e.nextPosition.X;
            //    spriteBatch.Draw(whiteRect, rect, Color.Purple);
            //}

            //draw the enemies A* path (for debugging)
            //if (Helpers.AStarDebugList != null)
            //{
            //    for (int i = 1; i < Helpers.AStarDebugList.Count - 1; i++)
            //    {
            //        rect.X = (int)(xOffset * GameConstants.ViewportWidth) + rect.Width * (int)Helpers.AStarDebugList[i].Y;
            //        rect.Y = (int)(yOffset * GameConstants.ViewportHeight) + rect.Height * (int)Helpers.AStarDebugList[i].X;
            //        spriteBatch.Draw(whiteRect, rect, Color.Pink);
            //    }
            //}

            spriteBatch.End();
        }
    }
}
