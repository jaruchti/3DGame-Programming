/*
 * 3D Game Programming Project
 * Dr. Liu
 * Zach Bates, Lauren Buss, Corey Darr, Jason Ruchti, Jared Tittle
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3DGameProject
{
    /// <summary>
    /// Base class for the GameObjects in the game.
    /// </summary>
    /// <remarks>
    /// Manages Models, Position, and BoundingSpheres
    /// Slightly modified from the Microsoft Fuel Cell example.
    /// </remarks>
    public class GameObject
    {
        /// <summary>
        /// Property to allow the client access to the GameObject's model.
        /// </summary>
        public Model Model { get; set; }

        /// <summary>
        /// Property to allow the client access to the GameObject's position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Property to allow the client access to the GameObject's BoundingSphere
        /// </summary>
        public BoundingSphere BoundingSphere { get; set; }

        /// <summary>
        /// Create a new GameObject
        /// </summary>
        public GameObject()
        {
            Model = null;
            Position = Vector3.Zero;
            BoundingSphere = new BoundingSphere();
        }

        /// <summary>
        /// Update the game object's position and the position of the bounding sphere.
        /// </summary>
        /// <param name="newPos">The position to move to.</param>
        public void UpdatePositionAndBoundingSphere(Vector3 newPos)
        {
            Position = newPos;

            BoundingSphere updatedSphere = BoundingSphere;
            updatedSphere.Center.X = Position.X;
            updatedSphere.Center.Z = Position.Z;
            BoundingSphere = updatedSphere;
        }

        /// <summary>
        /// Calculate the bounding sphere for the game object's model.
        /// </summary>
        /// <returns>Bounding sphere for this game object</returns>
        /// <remarks>Usually too large of a fit; scale with a constant.</remarks>
        protected BoundingSphere CalculateBoundingSphere()
        {
            BoundingSphere mergedSphere = new BoundingSphere(); // merge bounding sphere's for individual components
            BoundingSphere[] boundingSpheres;   // bounding spheres for each mesh
            int index = 0;
            int meshCount = Model.Meshes.Count;

            boundingSpheres = new BoundingSphere[meshCount];
            foreach (ModelMesh mesh in Model.Meshes)
            {
                boundingSpheres[index++] = mesh.BoundingSphere;
            }

            mergedSphere = boundingSpheres[0];
            if ((Model.Meshes.Count) > 1)
            {
                index = 1;
                do
                {
                    mergedSphere = BoundingSphere.CreateMerged(mergedSphere,
                        boundingSpheres[index]);
                    index++;
                } while (index < Model.Meshes.Count);
            }
            mergedSphere.Center.Y = 0;
            return mergedSphere;
        }

        /// <summary>
        /// Draw the bounding sphere for a given game object
        /// </summary>
        /// <param name="gameCamera">Used to get the view and projection matrices</param>
        /// <param name="boundingSphereModel">Model to use for the bounding sphere</param>
        /// <remarks>Used for debugging</remarks>
        internal void DrawBoundingSphere(Camera gameCamera, GameObject boundingSphereModel)
        {
            Matrix scaleMatrix = Matrix.CreateScale(BoundingSphere.Radius);
            Matrix translateMatrix = Matrix.CreateTranslation(BoundingSphere.Center); // translate drawing to correct position
            Matrix worldMatrix = scaleMatrix * translateMatrix;

            foreach (ModelMesh mesh in boundingSphereModel.Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = worldMatrix;
                    effect.View = gameCamera.ViewMatrix;
                    effect.Projection = gameCamera.ProjectionMatrix;
                }
                mesh.Draw();
            }
        }
    }
}
