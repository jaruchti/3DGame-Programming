
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
    class Enemy : GameObject
    {
        public float AngularPosition { get; set; } 
        public float ForwardDirection { get; set; }

        private int[,] floorPlan;

        public Enemy() : base()
        {
            ForwardDirection = 0.0f;
        }

        public void LoadContent(ContentManager content)
        {
            Model = content.Load<Model>("Models/sphere1uR");

            BoundingSphere = CalculateBoundingSphere();
            BoundingSphere scaledSphere = BoundingSphere;
            scaledSphere.Radius = BoundingSphere.Radius * 10;
            BoundingSphere = scaledSphere;
        }

        public void LoadFloorPlan(int[,] floorPlan)
        {
            this.floorPlan = floorPlan;
        }

        public void MakeIntroMoves()
        {
            circle(GameConstants.IntroCenter, GameConstants.IntroRadius);
        }

        public void circle(Vector3 center, float radius)
        {
            // Change the angular position of the drone based on the elapsed time since last update
            // and the angular velocity
            AngularPosition += GameConstants.IntroVelocity;

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
    }
}
