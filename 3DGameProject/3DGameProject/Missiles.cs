/*
 * 3D Game Programming Project
 * Dr. Liu
 * Zach Bates, Lauren Buss, Corey Darr, Jason Ruchti, Jared Tittle
 */

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace _3DGameProject
{
    /// <summary>
    /// Class which contains information on the missiles an enemy has fired torward
    /// the player
    /// </summary>
    public class Missiles
    {
        private double timeOfLastShot;      // time the last missile was fired
        private SpriteBatch spriteBatch;    // to draw the missiles
        private Texture2D missileTexture;   // texture for the missiles
        private Effect effect;              // effect to use when drawing the missile (point sprites)
        
        private SoundEffect explosionEffect;// sound effect to use when a missile strikes the player

        private List<Missile> missileList;  // list of the missiles that have been fired

        /// <summary>
        /// Create a new missiles object
        /// </summary>
        /// <remarks>
        /// Each enemy should have a single instance of this class
        /// </remarks>
        public Missiles()
        {
            timeOfLastShot = 0;
            missileList = new List<Missile>();
        }

        /// <summary>
        /// Load the content required to represent the missiles
        /// </summary>
        /// <param name="device">Graphics card (to initialize sprite batch)</param>
        /// <param name="content">Content pipeline (for effects, sounds, and missile texture)</param>
        public void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            spriteBatch = new SpriteBatch(device);

            missileTexture = content.Load<Texture2D>("Textures/missile");
            effect = content.Load<Effect>("Effects/effects");
            explosionEffect = content.Load<SoundEffect>("Audio/Explosion");
        }
        
        /// <summary>
        /// Allows an enemy to fire a missile at the player
        /// </summary>
        /// <param name="position">Position to fire a missile from</param>
        /// <param name="target">Position missile is being fired at</param>
        /// <param name="gameTime">To determine the time since the last missile was fired</param>
        /// <remarks>
        /// A missile can only be fired once every quarter second
        /// </remarks>
        public void FireAt(Vector3 position, Vector3 target, GameTime gameTime)
        {
            if ((gameTime.TotalGameTime.TotalMilliseconds - timeOfLastShot) > 250)
            {
                // atleast a half second has passed since the last missile was fired

                timeOfLastShot = gameTime.TotalGameTime.TotalMilliseconds;
                missileList.Add(new Missile(position, target)); // fire a missile
            }
        }

        /// <summary>
        /// Update the positions of the missile and, if applicable, updates the player's
        /// health if the player has been hit by a missile
        /// </summary>
        /// <param name="player">To update the player's health, if applicable</param>
        /// <param name="floorPlan">To determine if a missile is no longer in the map</param>
        /// <param name="gameState">The current state of the game</param>
        /// <remarks>
        /// If the player has lost all health, the game will transition to the end state
        /// </remarks>
        public void Update(Player player, int[,] floorPlan, ref GameConstants.GameState gameState)
        {
            // update each missile
            for (int i = 0; i < missileList.Count; i++)
            {
                missileList[i].Update();

                if (missileList[i].Position.Y < 0.0f || 
                    floorPlan[(int)missileList[i].Position.X, (int)-missileList[i].Position.Z] != 0)
                {
                    // this missile is off of the map (either ran into a building or under the map)
                    missileList.Remove(missileList[i]);
                    continue;
                }

                if (player.BoundingSphere.Contains(missileList[i].Position) != ContainmentType.Disjoint)
                {
                    // the player was hit
                    missileList.Remove(missileList[i]);
                    player.HitByMissile(ref gameState);
                    explosionEffect.Play();
                }
            }
        }

        /// <summary>
        /// Draw the missiles to the screen
        /// </summary>
        /// <param name="device">Graphics card</param>
        /// <param name="gameCamera">For view/projection matrices and camera position</param>
        public void Draw(ref GraphicsDevice device, Camera gameCamera)
        {
            if (missileList.Count > 0)  // there are missiles to draw
            {
                VertexPositionTexture[] missileVertices = new VertexPositionTexture[missileList.Count * 6];
                int i = 0;
                
                // setup the vertices for the missiles
                // to draw a single missile, six vertices are required
                foreach (Missile currentMissile in missileList)
                {
                    Vector3 center = currentMissile.Position;

                    // first triangle
                    missileVertices[i++] = new VertexPositionTexture(center, new Vector2(1, 1));
                    missileVertices[i++] = new VertexPositionTexture(center, new Vector2(0, 0));
                    missileVertices[i++] = new VertexPositionTexture(center, new Vector2(1, 0));

                    // second triangle
                    missileVertices[i++] = new VertexPositionTexture(center, new Vector2(1, 1));
                    missileVertices[i++] = new VertexPositionTexture(center, new Vector2(0, 1));
                    missileVertices[i++] = new VertexPositionTexture(center, new Vector2(0, 0));
                }

                // use point sprites and draw the missiles
                effect.CurrentTechnique = effect.Techniques["PointSprites"];
                effect.Parameters["xWorld"].SetValue(Matrix.Identity);
                effect.Parameters["xProjection"].SetValue(gameCamera.ProjectionMatrix);
                effect.Parameters["xView"].SetValue(gameCamera.ViewMatrix);
                effect.Parameters["xCamPos"].SetValue(gameCamera.CameraPosition);
                effect.Parameters["xTexture"].SetValue(missileTexture);
                effect.Parameters["xCamUp"].SetValue(Vector3.Up);
                effect.Parameters["xPointSpriteSize"].SetValue(0.1f);

                device.BlendState = BlendState.Additive;    // use additive blending to avoid black border around missiles

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawUserPrimitives(PrimitiveType.TriangleList, missileVertices, 0, missileList.Count * 2);
                }

                device.BlendState = BlendState.Opaque;     // turn off additive blending so other items display properly
            }
        }

        /// <summary>
        /// Resets the missiles class to the state is was at before play began
        /// </summary>
        public void Reset()
        {
            timeOfLastShot = 0;
            missileList.Clear();
        }
    }

    /// <summary>
    /// Helper class which represents an individual missile to be fired at the player
    /// </summary>
    public class Missile
    {
        /// <summary>Speed the missile travels at</summary>
        public const float MissileSpeed = 2 * Player.MaxSpeed;
        private Vector3 position;   // position of the missile
        private Vector3 target;     // where the missile is heading
        private Matrix rotation;    // matrix describing the direction from the current position to the target

        /// <summary>
        /// Create a new missile
        /// </summary>
        /// <param name="position">Where the missile is fired from</param>
        /// <param name="target">Where the missile is heading</param>
        public Missile(Vector3 position, Vector3 target)
        {
            this.position = position;
            this.target = target;

            // find the direction from position to target
            rotation = LookAt(position, target);
        }

        /// <summary>
        /// Allows the client to get and set the missile target
        /// </summary>
        public Vector3 Target
        {
            get { return target; }
            set
            {
                target = value;
                rotation = LookAt(position, target);
            }
        }

        /// <summary>
        /// Allows the client to get and set the missile position
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
            set { 
                position = value;
                rotation = LookAt(position, target);
            }
        }

        /// <summary>
        /// Returns a matrix describing the direction from the missile's position and its target
        /// </summary>
        /// <param name="position">Current location</param>
        /// <param name="lookat">Target</param>
        /// <returns></returns>
        private Matrix LookAt(Vector3 position, Vector3 lookat)
        {
            Matrix rotation = new Matrix();

            rotation.Forward = Vector3.Normalize(lookat - position);
            rotation.Right = Vector3.Normalize(Vector3.Cross(rotation.Forward, Vector3.Up));
            rotation.Up = Vector3.Normalize(Vector3.Cross(rotation.Right, rotation.Forward));

            return rotation;
        }

        /// <summary>
        /// Moves the missile torward the target at the appropriate speed
        /// </summary>
        public void Update()
        {
            Vector3 movement = Vector3.Transform(new Vector3(0.0f, 0.0f, -1.0f), rotation);
            movement *= MissileSpeed;
            Position = Position + movement;
        }
    }
}
