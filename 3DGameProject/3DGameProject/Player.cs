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

        public Player()
        {
            Position = new Vector3(15.5f, 0.1f, -9.5f);
            UpdateBoundingSphere();

            velocity = 0.0f;
            fuel = GameConstants.MAX_FUEL;

            sped = new Spedometer();
            fuelGauge = new FuelGauge();
        }

        public void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            effect = content.Load<Effect>("Effects/effects");
            Model = content.Load<Model>("Models/xwing");

            foreach (ModelMesh mesh in Model.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect.Clone();

            sped.LoadContent(ref device, content);
            fuelGauge.LoadContent(ref device, content);

            BoundingSphere = CalculateBoundingSphere();
        }

        public void Update(KeyboardState keyboardState, ref Map map)
        {
            float turnAmount = DetermineTurnAmount(keyboardState);
            UpdateVelocity(keyboardState);

            ForwardDirection += turnAmount * velocity * GameConstants.TurnSpeed;
            Vector3 movement = Vector3.Transform(new Vector3(0.0f, 0.0f, -1.0f), Matrix.CreateRotationY(ForwardDirection));
            movement *= velocity;

            Position = Position + movement;
            UpdateBoundingSphere();

            GameConstants.CollisionType collision = map.CheckCollision(this.BoundingSphere);

            if (collision == GameConstants.CollisionType.Building)
            {
                // undo the movement and set velocity to zero, can cause "bounceback effect"
                Position -= movement;
                velocity = 0.0f;

                UpdateBoundingSphere();
            }

            fuel -= 1.0f / 60f;
            if (fuel < 0)
                fuel = 0;
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

        private void UpdateBoundingSphere()
        {
            BoundingSphere updatedSphere = BoundingSphere;
            updatedSphere.Center.X = Position.X;
            updatedSphere.Center.Z = Position.Z;
            BoundingSphere = updatedSphere;
        }

        public void Draw(Camera gameCamera)
        {
            DrawModel(ref gameCamera);
            sped.Draw(velocity);
            fuelGauge.Draw(fuel);
        }

        public void DrawModel(ref Camera gameCamera)
        {
            Matrix worldMatrix = Matrix.CreateRotationY(MathHelper.Pi + ForwardDirection) * Matrix.CreateTranslation(Position);
            Matrix[] xwingTransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(xwingTransforms);

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    currentEffect.CurrentTechnique = currentEffect.Techniques["Colored"];
                    currentEffect.Parameters["xWorld"].SetValue(xwingTransforms[mesh.ParentBone.Index] * worldMatrix);
                    currentEffect.Parameters["xView"].SetValue(gameCamera.ViewMatrix);
                    currentEffect.Parameters["xProjection"].SetValue(gameCamera.ProjectionMatrix);
                    currentEffect.Parameters["xEnableLighting"].SetValue(true);
                    currentEffect.Parameters["xLightDirection"].SetValue(GameConstants.LightDirection);
                    currentEffect.Parameters["xAmbient"].SetValue(0.5f);
                }
                mesh.Draw();
            }
        }
    }
}
