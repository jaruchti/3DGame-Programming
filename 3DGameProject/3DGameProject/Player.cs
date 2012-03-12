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
    class Player : GameObject
    {
        public float ForwardDirection { get; set; }
        private Effect effect;

        private float velocity;
        private float fuel;

        private Spedometer sped;
        private FuelGauge fuelGauge;

        Texture2D[] textures;

        public Player()
        {
            Position = new Vector3(15.5f, 0.0f, -9.5f);
            UpdatePositionAndBoundingSphere(Position);

            velocity = 0.0f;
            fuel = GameConstants.MaxFuel;

            sped = new Spedometer();
            fuelGauge = new FuelGauge();
        }

        public void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            effect = content.Load<Effect>("Effects/careffect");
            Model = content.Load<Model>("Models/car");

            textures = new Texture2D[7];
            int i = 0;
            foreach (ModelMesh mesh in Model.Meshes)
                foreach (BasicEffect currentEffect in mesh.Effects)
                    textures[i++] = currentEffect.Texture;

            foreach (ModelMesh mesh in Model.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect.Clone();

            sped.LoadContent(ref device, content);
            fuelGauge.LoadContent(ref device, content);

            BoundingSphere = CalculateBoundingSphere();
            BoundingSphere scaledSphere = BoundingSphere;
            scaledSphere.Radius = BoundingSphere.Radius * 0.53f;
            BoundingSphere = scaledSphere;
        }

        public void Update(KeyboardState keyboardState, ref Map map)
        {
            float turnAmount = DetermineTurnAmount(keyboardState);
            UpdateVelocity(keyboardState);

            ForwardDirection += turnAmount * velocity * GameConstants.TurnSpeed;
            Vector3 movement = Vector3.Transform(new Vector3(0.0f, 0.0f, -1.0f), Matrix.CreateRotationY(ForwardDirection));
            movement *= velocity;

            UpdatePositionAndBoundingSphere(Position + movement);

            GameConstants.CollisionType collision = map.CheckCollision(this.BoundingSphere);

            if (collision == GameConstants.CollisionType.Building)
            {
                // undo the movement and set velocity to zero, can cause "bounceback effect"
                UpdatePositionAndBoundingSphere(Position - movement);
                velocity = 0.0f;

            }

            sped.Update(velocity);
            UpdateFuel(collision);
        }

        private float DetermineTurnAmount(KeyboardState keyboardState)
        {
            float turnAmount = 0;

            if (keyboardState.IsKeyDown(Keys.A))
                turnAmount = 1;
            else if (keyboardState.IsKeyDown(Keys.D))
                turnAmount = -1;

            return turnAmount;
        }

        private void UpdateVelocity(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.W))
            {
                if (velocity < 0) // braking
                    velocity += -GameConstants.Brake;
                else
                    velocity += GameConstants.Accel;
            }
            else if (keyboardState.IsKeyDown(Keys.S))
            {
                if (velocity > 0) // braking
                    velocity += GameConstants.Brake;
                else // reversing
                    velocity += GameConstants.Rev;
            }

            if (velocity < 0)
                velocity += GameConstants.Friction;
            else
                velocity -= GameConstants.Friction;

            if (velocity > GameConstants.MaxVelocity)
                velocity = GameConstants.MaxVelocity;
            else if (velocity < -GameConstants.MaxVelocity)
                velocity = -GameConstants.MaxVelocity;
        }

        private void UpdateFuel(GameConstants.CollisionType collision)
        {
            fuel -= GameConstants.FuelDrawDown;

            if (fuel < 0)
                fuel = 0;
            else if (collision == GameConstants.CollisionType.Fuel)
                fuel = GameConstants.MaxFuel;

            fuelGauge.Update(fuel);
        }

        public void Draw(Camera gameCamera)
        {
            DrawModel(ref gameCamera);
            sped.Draw();
            fuelGauge.Draw();
        }

        public void DrawModel(ref Camera gameCamera)
        {
            Matrix[] modelTransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(modelTransforms);
            Matrix worldMatrix = Matrix.Identity;
            Matrix rotationYMatrix = Matrix.CreateRotationY(ForwardDirection + MathHelper.Pi);
            Matrix translateMatrix = Matrix.CreateTranslation(Position);

            worldMatrix = rotationYMatrix * translateMatrix;
            int i = 0;

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    currentEffect.CurrentTechnique = currentEffect.Techniques["Simplest"];
                    currentEffect.Parameters["xWorldViewProjection"].SetValue(modelTransforms[mesh.ParentBone.Index] * worldMatrix * gameCamera.ViewMatrix * gameCamera.ProjectionMatrix);
                    currentEffect.Parameters["xTexture"].SetValue(textures[i++]);
                }
                mesh.Draw();
            }
        }
    }
}
