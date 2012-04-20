/*
 * 3D Game Programming Project
 * Dr. Liu
 * Zach Bates, Lauren Buss, Corey Darr, Jason Ruchti, Jared Tittle
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace _3DGameProject
{
    /// <summary>
    /// Class which represents a fuel object in the game for the player to pick up.
    /// </summary>
    public class Fuel : GameObject
    {
        /// <summary>
        /// Construct a new fuel object
        /// </summary>
        public Fuel() : base()
        {
            // nothing to do here
        }

        /// <summary>
        /// Load the fuel model and calculate the bounding sphere.
        /// </summary>
        /// <param name="content">Content pipeline (for models)</param>
        public void LoadContent(ContentManager content)
        {
            Model = content.Load<Model>("Models/motoroil");
            Position = Vector3.Down;
            BoundingSphere = CalculateBoundingSphere();

            BoundingSphere scaledSphere;
            scaledSphere = BoundingSphere;
            scaledSphere.Radius *= 0.5f;
            BoundingSphere = new BoundingSphere(scaledSphere.Center, scaledSphere.Radius);
        }

        /// <summary>
        /// Draw the fuel object to the screen.
        /// </summary>
        /// <param name="gameCamera">Used to get view and projection matrices</param>
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
