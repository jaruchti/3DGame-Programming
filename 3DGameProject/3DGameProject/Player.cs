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
            Position = GameConstants.playerStartPos;
            UpdatePositionAndBoundingSphere(Position);

            ForwardDirection = 0.0f;
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

        public void Update(KeyboardState keyboardState, GameTime gameTime, ref Map map, ref GameConstants.GameState gameState)
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
            UpdateFuel(gameTime, collision, ref gameState);
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

        private void UpdateFuel(GameTime gameTime, GameConstants.CollisionType collision, ref GameConstants.GameState gameState)
        {
            fuel -= (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (fuel < 0)
            {
                fuel = 0;
                gameState = GameConstants.GameState.End;
            }
            else if (collision == GameConstants.CollisionType.Fuel)
                fuel = GameConstants.MaxFuel;

            fuelGauge.Update(fuel);
        }

        public void AutoPilot(ref GameConstants.GameState gameState)
        {
            Rectangle playerStartRect = new Rectangle((int) GameConstants.playerStartPos.X, (int) -GameConstants.playerStartPos.Z, 1, 1);
            Rectangle[] pivots = { new Rectangle( 3,  4, 1, 1),
                                   new Rectangle( 3, 14, 1, 1),
                                   new Rectangle(15,  4, 1, 1),
                                   new Rectangle(15, 14, 1, 1) };

            float[] pivotForwardDirections = {  3 * MathHelper.PiOver2,
                                                MathHelper.Pi,
                                                MathHelper.TwoPi,
                                                MathHelper.PiOver2 };                             

            float turnAmount = 0.0f;
            velocity = 2.0f / 60.0f;

            for (int i = 0; i < pivots.Length; i++)
            {
                if (pivots[i].Contains((int) Position.X, (int) -Position.Z))
                {
                    turnAmount = 2.90f;
                    ForwardDirection += turnAmount * velocity * GameConstants.TurnSpeed;

                    if (ForwardDirection > pivotForwardDirections[i])
                        ForwardDirection = pivotForwardDirections[i];
                }
            }

            if (playerStartRect.Contains((int)Position.X, (int)-Position.Z) && ForwardDirection > 0)
            {
                gameState = GameConstants.GameState.Ready;
            }
            
            Vector3 movement = Vector3.Transform(new Vector3(0.0f, 0.0f, -1.0f), Matrix.CreateRotationY(ForwardDirection));
            movement *= velocity;

            UpdatePositionAndBoundingSphere(Position + movement);
        }

        public void Draw(Camera gameCamera, GameConstants.GameState gameState)
        {
            DrawModel(ref gameCamera);

            if (gameState != GameConstants.GameState.Intro)
            {
                sped.Draw();
                fuelGauge.Draw();
            }
        }

        private void DrawModel(ref Camera gameCamera)
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

        public void Reset()
        {
            Position = GameConstants.playerStartPos;
            UpdatePositionAndBoundingSphere(Position);

            ForwardDirection = 0.0f;
            velocity = 0.0f;
            fuel = GameConstants.MaxFuel;
        }
    }
}
