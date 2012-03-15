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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _3DGameProject
{
    /// <summary>
    /// Class which defines a player in the game.
    /// </summary>
    /// <remarks>This is a singleton in the game</remarks>
    public class Player : GameObject
    {
        /// <summary>Maximim velocity for the player</summary>
        public const float MaxVelocity = 1.25f / 30.0f;
        /// <summary>Turn speed for the player</summary>
        public const float TurnSpeed = 0.7f;
        /// <summary>Braking velocity for the player</summary>
        public const float Brake = -0.04f / 60f;
        /// <summary>Acceleration for the player</summary>
        public const float Accel = 0.015f / 60f;
        /// <summary>Reversing velocity for the player</summary>
        public const float Rev = -0.01f / 60f;
        /// <summary>Friction acting on the player</summary>
        public const float Friction = 0.002f / 60f;
        /// <summary>Maximum fuel amount for the player</summary>
        public const float MaxFuel = 99 + 59.0f / 60.0f;
        /// <summary>Starting position for the player</summary>
        public static readonly Vector3 PlayerStartPos = new Vector3(15.5f, 0.0f, -9.5f);
        /// <summary>Starting direction for the player</summary>
        public const float PlayerStartDirection = 0.0f;
        /// <summary>Ratio to scale bounding sphere for better fit</summary>
        public const float PlayerBoundingSphereRatio = 0.53f;

        private PlayerSoundEffects soundEffects; // sound effects for the player class
        private Effect effect;  // effect to be used when drawing the car model

        private float velocity; // player velocity
        private float fuel;     // player fuel remaining
        private float score;    // player score

        private Spedometer sped;        // to display speed to screen
        private FuelGauge fuelGauge;    // to display fuel usage to screening

        /// <summary>
        /// Property to allow the client to set and retreive the forward direction of the player.
        /// </summary>
        /// <remarks>This is the direction the car is moving</remarks>
        public float ForwardDirection { get; set; }

        /// <summary>
        /// Property to allow the client to get the player's score
        /// </summary>
        public float Score {
            get { return score; }
        }

        Texture2D[] textures;   // textures for the car model

        /// <summary>
        /// Create a new player object.
        /// </summary>
        public Player()
        {
            // Place player
            Position = PlayerStartPos;
            UpdatePositionAndBoundingSphere(Position);

            // Setup player fields
            ForwardDirection = PlayerStartDirection;
            velocity = 0.0f;
            score = 0.0f;
            fuel = MaxFuel;

            // Create displays
            sped = new Spedometer();
            fuelGauge = new FuelGauge();

            // Get sound effects
            soundEffects = new PlayerSoundEffects();
        }

        /// <summary>
        /// Load the content required to represent the player on screen.
        /// </summary>
        /// <param name="device">Graphics card</param>
        /// <param name="content">Content pipeline</param>
        public void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            // Load effect/model
            effect = content.Load<Effect>("Effects/careffect");
            Model = content.Load<Model>("Models/car");

            // Retrieve textures from model
            textures = new Texture2D[7];
            int i = 0;
            foreach (ModelMesh mesh in Model.Meshes)
                foreach (BasicEffect currentEffect in mesh.Effects)
                    textures[i++] = currentEffect.Texture;

            // Apply effect to model meshes
            foreach (ModelMesh mesh in Model.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect.Clone();

            // Load displays for gauges
            sped.LoadContent(ref device, content);
            fuelGauge.LoadContent(ref device, content);

            // Load sound effects
            soundEffects.LoadContent(content);

            // Setup bounding sphere for player
            BoundingSphere = CalculateBoundingSphere();
            BoundingSphere scaledSphere = BoundingSphere;
            scaledSphere.Radius = BoundingSphere.Radius * PlayerBoundingSphereRatio;
            BoundingSphere = scaledSphere;
        }

        /// <summary>
        /// Update the player's attributes (called 60 times sec).
        /// </summary>
        /// <param name="keyboardState">Information about user input</param>
        /// <param name="gameTime">Information about time elapsed since last update</param>
        /// <param name="map">The map the player is within</param>
        /// <param name="gameState">The state of the game</param>
        /// <remarks>If the player runs out of fuel, the gamestate will be changed to end</remarks>
        public void Update(KeyboardState keyboardState, GameTime gameTime, ref Map map, ref GameConstants.GameState gameState)
        {
            Vector3 oldPosition = Position; // the position we were before the update
            Vector3 movement;

            float turnAmount = DetermineTurnAmount(keyboardState); // determine if the user wants to turn
            UpdateVelocity(keyboardState);                         // update the velocity of the player based on user input

            // Move the player to the next position based on the current position and movement amount
            ForwardDirection += turnAmount * velocity * TurnSpeed;  // note: the final turn amount takes velocity into account
            movement = Vector3.Transform(new Vector3(0.0f, 0.0f, -1.0f), Matrix.CreateRotationY(ForwardDirection));
            movement *= velocity;
            UpdatePositionAndBoundingSphere(Position + movement);

            // Check for collision with objects in the map
            GameConstants.CollisionType collision = map.CheckCollision(this.BoundingSphere);
            if (collision == GameConstants.CollisionType.Building)
            {
                // undo the movement and set velocity to zero since we ran into building
                // can cause "bounceback effect"
                UpdatePositionAndBoundingSphere(oldPosition);

                // Play crash sound effect (major or minor crash effect based on player velocity)
                soundEffects.PlayCrash(velocity);

                velocity = 0.0f;
            }

            // Update the gauges
            sped.Update(velocity);
            UpdateFuel(gameTime, collision, ref gameState);

            // Update the player's score based on the movement that has been made
            UpdateScore(oldPosition, Position);
        }

        /// <summary>
        /// Determine if the user wants to turn
        /// </summary>
        /// <param name="keyboardState">For keyboard input</param>
        /// <returns>
        /// Float representing the amount the user wants to turn (positive for left, negative for right)
        /// </returns>
        private float DetermineTurnAmount(KeyboardState keyboardState)
        {
            float turnAmount = 0;

            if (keyboardState.IsKeyDown(Keys.A))
                turnAmount = 1;
            else if (keyboardState.IsKeyDown(Keys.D))
                turnAmount = -1;

            return turnAmount;
        }

        /// <summary>
        /// Update the velocity of the player based on user input
        /// </summary>
        /// <param name="keyboardState">For keyboard input</param>
        private void UpdateVelocity(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.W))
            {
                if (velocity < 0)
                {
                    // the player was reversing, now braking
                    velocity += -Brake;

                    // play braking sound effect based on the player's velocity
                    soundEffects.PlayBrake(velocity);
                }
                else
                {
                    // the player is accelerating
                    velocity += Accel;
                }
            }
            else if (keyboardState.IsKeyDown(Keys.S))
            {
                if (velocity > 0)
                {
                    // the player was moving forward, now braking
                    velocity += Brake;

                    // play braking sound effect based on the player's velocity
                    soundEffects.PlayBrake(velocity);
                }
                else
                {
                    // the player is reversing
                    velocity += Rev;
                }
            }

            // Add friction into the mix
            if (velocity < 0)
                velocity += Friction;
            else
                velocity -= Friction;

            // keep velocity within valid range
            if (velocity > MaxVelocity)
                velocity = MaxVelocity;
            else if (velocity < -MaxVelocity)
                velocity = -MaxVelocity;

            // Play the steady drone of the engine (the volume is proportional to the velocity of the car)
            PlayEngineNoise();
        }

        /// <summary>
        /// Update the amount of fuel the player has and the player's fuel gauges.
        /// </summary>
        /// <param name="gameTime">Lets us determine the amount of time since last update</param>
        /// <param name="collision">Lets us determine if the car collided with a fuel object</param>
        /// <param name="gameState">Current state of the game</param>
        /// <remarks>The gamestate is changed to end if the player runs out of fuel</remarks>
        private void UpdateFuel(GameTime gameTime, GameConstants.CollisionType collision, ref GameConstants.GameState gameState)
        {
            fuel -= (float) gameTime.ElapsedGameTime.TotalSeconds;  // decrement fuel usage based on time since last update


            if (fuel < 0)
            {
                // out of fuel, transition to ending
                fuel = 0;
                gameState = GameConstants.GameState.End;
            }
            else if (collision == GameConstants.CollisionType.Fuel)
            {
                // picked up fuel while driving
                fuel = MaxFuel;
            }

            fuelGauge.Update(fuel);
        }

        /// <summary>
        /// Update the player's score based on the amount of movement the player has made
        /// </summary>
        /// <param name="oldPosition">Player's position before last update</param>
        /// <param name="newPosition">Player's new position after last update</param>
        private void UpdateScore(Vector3 oldPosition, Vector3 newPosition)
        {
            double xMovement = oldPosition.X - newPosition.X;
            double zMovement = oldPosition.Z - newPosition.Z;

            score += (float) Math.Sqrt(xMovement * xMovement + zMovement * zMovement);
        }

        /// <summary>
        /// Draw the player model and the player's gauges (if applicable)
        /// </summary>
        /// <param name="gameCamera">For view and projection matrices</param>
        /// <param name="gameState">State of the game</param>
        /// <remarks>The player's gauges are not drawn during the intro</remarks>
        public void Draw(Camera gameCamera, GameConstants.GameState gameState)
        {
            DrawModel(ref gameCamera);

            if (gameState != GameConstants.GameState.Intro)
            {
                sped.Draw();
                fuelGauge.Draw();
            }
        }

        /// <summary>
        /// Draw the Model representing the player
        /// </summary>
        /// <param name="gameCamera">For view/projection matrices</param>
        private void DrawModel(ref Camera gameCamera)
        {
            Matrix[] modelTransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(modelTransforms);
            Matrix worldMatrix = Matrix.Identity;
            Matrix rotationYMatrix = Matrix.CreateRotationY(ForwardDirection + MathHelper.Pi);  // rotate car based on direction of car
            Matrix translateMatrix = Matrix.CreateTranslation(Position);    // to move to position where the player is at

            worldMatrix = rotationYMatrix * translateMatrix;    // calculate world matrix from rotation/translation
            int i = 0;

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    // Setup effect and place texture on model
                    currentEffect.CurrentTechnique = currentEffect.Techniques["Simplest"];
                    currentEffect.Parameters["xWorldViewProjection"].SetValue(modelTransforms[mesh.ParentBone.Index] * worldMatrix * gameCamera.ViewMatrix * gameCamera.ProjectionMatrix);
                    currentEffect.Parameters["xTexture"].SetValue(textures[i++]);
                }
                mesh.Draw();
            }
        }

        /// <summary>
        /// Play the engine noise for the player's car.
        /// </summary>
        public void PlayEngineNoise()
        {
            soundEffects.PlayEngine(velocity);
        }

        /// <summary>
        /// Reset the player back to the state just before play begins.
        /// </summary>
        public void Reset()
        {
            Position = PlayerStartPos;
            UpdatePositionAndBoundingSphere(Position);

            ForwardDirection = PlayerStartDirection;
            velocity = 0.0f;
            score = 0.0f;
            fuel = MaxFuel;
        }


        /* 
         * ----------------------------------------------------------------------------
         * The following section is for the introduction 
         * --------------------------------------------------------------------------- 
         */


        // Rectangle with the player's starting position
        Rectangle playerStartRect = new Rectangle((int)PlayerStartPos.X, (int) -PlayerStartPos.Z, 1, 1);

        // When the car enters one of these rectangles during the introduction, the car should turn
        Rectangle[] pivots = { new Rectangle( 3,  4, 1, 1),
                               new Rectangle( 3, 14, 1, 1),
                               new Rectangle(15,  4, 1, 1),
                               new Rectangle(15, 14, 1, 1) };

        // When the car enters one of the pivot rectangles during the introduction, the car
        // should turn to face the direction given in this array 
        float[] pivotForwardDirections = {  3 * MathHelper.PiOver2,
                                                MathHelper.Pi,
                                                MathHelper.TwoPi,
                                                MathHelper.PiOver2 };

        /// <summary>
        /// Moves the player around the map during the introduction without user input.
        /// </summary>
        /// <param name="gameState">The state of the game</param>
        /// <remarks>
        /// If the player makes a complete revolution of the map, the gameState transitions
        /// to ready.
        /// </remarks>

        public const float IntroVelocity = 2.0f / 60.0f; // velocity during intro (greater than game)
        public const float IntroTurnAmount = 2.90f;      // used to determine speed of turning during into (greater than game)

        public void AutoPilot(ref GameConstants.GameState gameState)
        {
            float turnAmount = 0.0f;
            velocity = IntroVelocity;

            for (int i = 0; i < pivots.Length; i++)
            {
                if (pivots[i].Contains((int)Position.X, (int)-Position.Z))
                {
                    // This a square the player should make a turn on
                    // Update the forward direction based on turn amount
                    turnAmount = IntroTurnAmount;
                    ForwardDirection += turnAmount * velocity * TurnSpeed;

                    // Only turn as far as required by this pivot
                    if (ForwardDirection > pivotForwardDirections[i])
                        ForwardDirection = pivotForwardDirections[i];
                }
            }

            // Check if player has made a complete reveloution and move into ready gamestate if so
            if (playerStartRect.Contains((int)Position.X, (int)-Position.Z) && ForwardDirection > 0)
            {
                gameState = GameConstants.GameState.Ready;
            }

            // Move the player to the next position based on the current position and movement amount
            Vector3 movement = Vector3.Transform(new Vector3(0.0f, 0.0f, -1.0f), Matrix.CreateRotationY(ForwardDirection));
            movement *= velocity;
            UpdatePositionAndBoundingSphere(Position + movement);

            // play the sound of the engine
            soundEffects.PlayEngine(Player.MaxVelocity);
        }
    }
}
