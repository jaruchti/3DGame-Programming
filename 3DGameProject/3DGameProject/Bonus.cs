/*
 * 3D Game Programming Project
 * Dr. Liu
 * Zach Bates, Lauren Buss, Corey Darr, Jason Ruchti, Jared Tittle
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace _3DGameProject
{
    /// <summary>
    /// Class which represents an object for the player to pick up 
    /// while driving for a point bonus
    /// </summary>
    public class Bonus : GameObject
    {
        /// <summary>
        /// Load the bonus model and calculate the bounding sphere
        /// </summary>
        /// <param name="device">Graphics card (to initialize spritebatch)</param>
        /// <param name="content">Content pipeline (for models)</param>
        public void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            Model = content.Load<Model>("Models/motoroil");

            // Find bounding sphere and scale for best fit
            BoundingSphere = CalculateBoundingSphere();
            BoundingSphere scaledSphere = BoundingSphere;
            scaledSphere.Radius = BoundingSphere.Radius * 0.5f;
            BoundingSphere = scaledSphere;
        }

        /// <summary>
        /// Places the bonus in a random location within the map where there
        /// are no other objects currently residing
        /// </summary>
        /// <param name="m">To determine if the placement is valid</param>
        public void PlaceRandomly(Map m)
        {
            Random r = new Random();
            Vector3 newPosition = new Vector3();

            // loop until we find a new valid location
            do
            {
                newPosition.X = r.Next(0, m.FloorPlan.GetLength(0)) + 0.5f;
                newPosition.Z = -r.Next(0, m.FloorPlan.GetLength(1)) - 0.5f;
            } while (IsOccupied(newPosition, m));

            // Update the position and bounding sphere with this new location
            UpdatePositionAndBoundingSphere(newPosition);
        }

        /// <summary>
        /// Informs the client whether the newPosition parameter is valid location for a bonus in the map
        /// </summary>
        /// <param name="newPosition">Position within the map</param>
        /// <param name="m">For the location of all of the objects and buildings</param>
        /// <returns>
        /// True if newPosition is a location where a bonus can be placed, false otherwise
        /// </returns>
        private bool IsOccupied(Vector3 newPosition, Map m)
        {
            bool isOccupied = false;

            // check if a building is at new location
            if (m.FloorPlan[(int)newPosition.X, (int)-newPosition.Z] != 0)
                isOccupied = true;

            // check if a fuel cell is nearby
            foreach(Fuel fuel in m.FuelBarrels)
                if (MathHelper.Distance(newPosition.X, fuel.Position.X) < 2.0f ||
                    MathHelper.Distance(newPosition.Z, fuel.Position.Z) < 2.0f)
                    isOccupied = true;

            // check if another bonus is nearby
            foreach (Bonus b in m.Bonuses)
            {
                if (b != this)
                {
                    if (MathHelper.Distance(newPosition.X, b.Position.X) < 2.0f ||
                        MathHelper.Distance(newPosition.Z, b.Position.Z) < 2.0f)
                        isOccupied = true;
                }
            }

            return isOccupied;
        }

        /// <summary>
        /// Draw the bonus model to the screen
        /// </summary>
        /// <param name="gameCamera">For view and projection matrices</param>
        public void Draw(Camera gameCamera)
        {
            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix translateMatrix = Matrix.CreateTranslation(Position);
            Matrix worldMatrix = translateMatrix;

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = worldMatrix * transforms[mesh.ParentBone.Index];
                    effect.View = gameCamera.ViewMatrix;
                    effect.Projection = gameCamera.ProjectionMatrix;

                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }
                mesh.Draw();
            }
        }
    }
}
