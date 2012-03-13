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
        private float ForwardDirection { get; set; }
        private float velocity;

        private int[,] floorPlan;

        public Enemy() : base()
        {
            velocity = GameConstants.MaxVelocity;
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
