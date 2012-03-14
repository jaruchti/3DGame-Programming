/*
 * 3D Game Programming Project
 * Dr. Liu
 * Zach Bates, Lauren Buss, Corey Darr, Jason Ruchti, Jared Tittle
 */

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
    /// <summary>
    /// Class which defines the attributes of an enemy in the game.
    /// </summary>
    class Enemy : GameObject
    {
        /// <summary>Velocity of the enemy</summary>
        public const float EnemyVelocity = 0.8f / 60.0f;
        /// <summary>Ratio to scale bounding sphere for better fit</summary>
        public const float EnemyBoundingSphereRatio = 10.0f;

        /// <summary>
        /// Property which defines the direction the enemy is moving.
        /// </summary>
        public float ForwardDirection { get; set; }
        
        /// <summary>
        /// Used to create a new enemy object.
        /// </summary>
        public Enemy() : base()
        {
            ForwardDirection = 0.0f;
        }

        /// <summary>
        /// Used to load the model to represent the enemy on screen and set up enemy's bounding sphere.
        /// </summary>
        /// <param name="content">Content pipeline</param>
        public void LoadContent(ContentManager content)
        {
            // Load model
            Model = content.Load<Model>("Models/sphere1uR");

            // Setup bounding sphere (scale for best fit)
            BoundingSphere = CalculateBoundingSphere();
            BoundingSphere scaledSphere = BoundingSphere;
            scaledSphere.Radius = BoundingSphere.Radius * EnemyBoundingSphereRatio;
            BoundingSphere = scaledSphere;
        }

        /// <summary>
        /// Draw the enemy model
        /// </summary>
        /// <param name="gameCamera">For view and projection matrices</param>
        public void Draw(Camera gameCamera)
        {
            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix translateMatrix = Matrix.CreateTranslation(Position);    // move the model to the correct position
            Matrix worldMatrix = translateMatrix;   // setup worl matrix based on translation

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    // setup effect based on worldMatrix and properties of the camera
                    effect.World =
                        worldMatrix * transforms[mesh.ParentBone.Index];
                    effect.View = gameCamera.ViewMatrix;
                    effect.Projection = gameCamera.ProjectionMatrix;

                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }
                mesh.Draw();
            }
        }


        /* 
        * ----------------------------------------------------------------------------
        * The following section is for the introduction 
        * --------------------------------------------------------------------------- 
        */

        public float AngularPosition { get; set; }
        public const float IntroVelocity = 2.0f / 60.0f;

        public void circle(Vector3 center, float radius)
        {
            // Change the angular position of the drone based on the elapsed time since last update
            // and the angular velocity
            AngularPosition += IntroVelocity;

            // Normalize angularPosition so it is between 0 and 2 pi.
            if (AngularPosition > 2 * MathHelper.Pi)
                AngularPosition = AngularPosition - 2 * MathHelper.Pi;

            // Transform the polor coordinates given by the drone's angular position and radius into
            // Cartesian coordinates, and store the values in the futurePosition vector.
            // The futurePosition vector holds the position the drone will be.
            Vector3 futurePosition = new Vector3(0, Position.Y, radius);
            futurePosition.X = radius * (float)Math.Cos(AngularPosition) + center.X;
            futurePosition.Z = radius * (float)Math.Sin(AngularPosition) + center.Z;

            // Now, update the Position and ForwardDirection after the calculations for the update
            // have completed.
            Position = futurePosition;
            ForwardDirection = -AngularPosition;
        }
    }
}
