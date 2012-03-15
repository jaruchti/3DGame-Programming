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
    public class Enemy : GameObject
    {
        /// <summary>Cruising speed of the enemy</summary>
        public const float EnemySpeed = 0.75f / 60.0f;
        /// <summary>Ratio to scale bounding sphere for better fit</summary>
        public const float EnemyBoundingSphereRatio = 10.0f;

        /// <summary>
        /// Property which defines the direction the enemy is moving.
        /// </summary>
        public float ForwardDirection { get; set; }

        private float speed; // current speed of the enemy craft (either EnemyVelocity or 0)        
        private Rectangle positionOfLastMove = new Rectangle(0, 0, 1, 1); // rectangle where the enemy made the last move decision
        private bool chasing = false;   // status as to whether the enemy is chasing the player
        private Rectangle nextPosition = new Rectangle(0, 0, 1, 1); // rectangle the enemy decided to head towards on the last move decision
        private EnemySoundEffects soundEffects; // sound effects for the enemy

        /// <summary>
        /// Allows the client to client to query the status of the enemy 
        /// craft in regards to whether or not it is chasing the player
        /// </summary>
        public bool Chasing
        {
            get { return chasing; }
        }

        /// <summary>
        /// Defines the position the enemy decided to head towards on the last move decision
        /// </summary>
        /// <remarks>Used to make sure no two enemy craft move towards the same position</remarks>
        public Rectangle NextPosition
        {
            get { return NextPosition; }
        }

        /// <summary>
        /// Used to create a new enemy object.
        /// </summary>
        public Enemy() : base()
        {
            speed = 0.0f;
            ForwardDirection = 45.0f;

            soundEffects = new EnemySoundEffects();
        }

        /// <summary>
        /// Used to load the model to represent the enemy on screen and set up enemy's bounding sphere.
        /// </summary>
        /// <param name="content">Content pipeline</param>
        public void LoadContent(ContentManager content)
        {
            // Load model
            Model = content.Load<Model>("Models/sphere1uR");

            // Load sound effects
            soundEffects.LoadContent(content);

            // Setup bounding sphere (scale for best fit)
            BoundingSphere = CalculateBoundingSphere();
            BoundingSphere scaledSphere = BoundingSphere;
            scaledSphere.Radius = BoundingSphere.Radius * EnemyBoundingSphereRatio;
            BoundingSphere = scaledSphere;
        }

        /// <summary>
        /// Allows the client to move the enemy craft
        /// </summary>
        /// <param name="enemies">All of the other enemies in the game</param>
        /// <param name="player">Allows queries for attributes of the player</param>
        /// <param name="floorPlan">Gives the arrangement of the building in the city</param>
        /// <param name="gameState">Gives the current state of the game</param>
        /// <remarks>If the player is caught, the gamestate will transition to end</remarks>
        public void Update(Enemy[] enemies, Player player, int[,] floorPlan, ref GameConstants.GameState gameState)
        {
            bool canMakeMovement = false;   // boolean for status if the player was able to make a move on this update
            Vector3 movement;               // the movement to be applied to the enemy

            // check if the player has been caught, that is the enemy craft and player are in the same 
            // grid unit and if so transition to game ending state
            if (((int)Position.X - (int)player.Position.X) == 0 && ((int)Position.Z - (int)player.Position.Z) == 0)
            {
                gameState = GameConstants.GameState.End;
                return;
            }


            if (ReadyForNextMove())
            {
                // mark the spots on the grid where the enemies are and where they are headed in the next move
                MarkEnemySquares(enemies, floorPlan);   

                // the player is in the center of a grid square, and can decide on the next direction to face
                // now, check if the player is in the enemy's line of sight (no buildings or enemies obstructing view
                // from enemy to player), and, if so, set the chasing property
                // to true
                chasing = CheckIfInLineOfSight(player, floorPlan);

                if (chasing)
                {
                    // we are chasing the player, set direction toward the player
                    SetDirectionTowardPlayer(player);

                    // we can make a movement since nothing is obstructing between player and this enemy
                    canMakeMovement = true;

                    // play sound effect so player knows we are chasing
                    soundEffects.PlayLockingBeep();
                }
                else
                {
                    // we are not chasing, move randomly around the grid until we spot the player next
                    canMakeMovement = MoveRandomly(floorPlan); // returns true if a move was able to be made
                }

                // unmark the spots on the grid where the enemies are and where they are headed in the next move
                UnmarkEnemySquares(enemies, floorPlan);

                if (!canMakeMovement)
                {
                    // the enemy is surrounded and cannot make a move
                    // set velocity to zero and stay put for this move
                    speed = 0.0f;
                }
                else
                {
                    // update positionOfLastMove with grid square of current position
                    positionOfLastMove = new Rectangle((int)Position.X, (int)-Position.Z, 1, 1);

                    // update nextPosition with next grid space in the direction
                    // we are headed
                    nextPosition = new Rectangle(
                        (int)(Position.X - Math.Sin(ForwardDirection)),
                        (int)(Position.Z - Math.Cos(ForwardDirection)), 1, 1);

                    speed = EnemySpeed;
                }
            }

            // Make the movement
            movement = Vector3.Transform(new Vector3(0.0f, 0.0f, -1.0f), Matrix.CreateRotationY(ForwardDirection));
            movement *= speed;
            UpdatePositionAndBoundingSphere(Position + movement);
        }

        /// <summary>
        /// Check whether is ready to make decision on where to move next
        /// </summary>
        /// <returns>True if the enemy can make a move decision, false otherwise</returns>
        /// <remarks>
        /// The enemy is ready for a new move decision if it is at the center of a
        /// new gridspace
        /// </remarks>
        private bool ReadyForNextMove()
        {
            float xPosInSquare = Position.X - (int)Position.X;
            float zPosInSquare = -Position.Z + (int)Position.Z;

            // return true if the player is in the center of a new grid space
            return (xPosInSquare > 0.4f && xPosInSquare < 0.6f) && (zPosInSquare > 0.4f && zPosInSquare < 0.6f)
                && !positionOfLastMove.Contains((int)Position.X, (int)-Position.Z);
        }

        /// <summary>
        /// Checks if the player is in the line of sight of the enemy, that is, there are no building
        /// or other enemies between the player and the enemy craft
        /// </summary>
        /// <param name="player">For the player's position</param>
        /// <param name="floorPlan">For the layout of the building</param>
        /// <returns>True if the enemy can see the player, false otherwise</returns>
        private bool CheckIfInLineOfSight(Player player, int[,] floorPlan)
        {
            int xOffsetToPlayer = (int)player.Position.X - (int)Position.X; // x displacement from player to enemy
            int zOffsetToPlayer = (int)-player.Position.Z + (int)Position.Z;// z displacement from player to enemy
            bool inLineOfSight = false; // can we see the player?

            if (zOffsetToPlayer == 0)
            {
                inLineOfSight = true;   // assume that we can see the player

                // move up the map to the player position and check for obstacles in the way
                // if there are obstacles, mark inLineOfSight false and break
                for (int i = (int)Position.X; i < (int)player.Position.X; i++)
                {
                    if (floorPlan[i, (int)-Position.Z] != 0)
                    {
                        inLineOfSight = false;
                        break;
                    }
                }

                // move down the map to the player position and check for obstacles in the way
                // if there are obstacles, mark inLineOfSight false and break
                for (int i = (int)Position.X; i > (int)player.Position.X; i--)
                {
                    if (floorPlan[i, (int)-Position.Z] != 0)
                    {
                        inLineOfSight = false;
                        break;
                    }
                }
            }
            else if (xOffsetToPlayer == 0)
            {
                inLineOfSight = true;

                // move right on the map to the player position and check for obstacles in the way
                // if there are obstacles, mark inLineOfSight false and break
                for (int i = (int)-Position.Z; i < (int)-player.Position.Z; i++)
                {
                    if (floorPlan[(int)Position.X, i] != 0)
                    {
                        inLineOfSight = false;
                        break;
                    }
                }

                // move left on the map to the player position and check for obstacles in the way
                // if there are obstacles, mark inLineOfSight false and break
                for (int i = (int)-Position.Z; i > (int)-player.Position.Z; i--)
                {
                    if (floorPlan[(int)Position.X, i] != 0)
                    {
                        inLineOfSight = false;
                        break;
                    }
                }
            }

            return inLineOfSight;
        }

        /// <summary>
        /// Sets the foward direction of the enemy to point toward the player
        /// </summary>
        /// <param name="player">For the player's position</param>
        private void SetDirectionTowardPlayer(Player player)
        {
            int xOffsetToPlayer = (int)player.Position.X - (int)Position.X; // x displacement from player to enemy
            int zOffsetToPlayer = (int)-player.Position.Z + (int)Position.Z;// z displacement from player to enemy

            if (zOffsetToPlayer != 0)
            {
                // Must move left or right

                if (zOffsetToPlayer > 0)
                {
                    // Move right
                    ForwardDirection = 0;
                }
                else
                {
                    // Move left
                    ForwardDirection = MathHelper.Pi;
                }
            }
            else
            {
                // Must move up or down

                if (xOffsetToPlayer > 0)
                {
                    // Move down, player below
                    ForwardDirection = 3 * MathHelper.PiOver2;
                }
                else
                {
                    // Move up, player above
                    ForwardDirection = MathHelper.PiOver2;
                }
            } 
        }

        /// <summary>
        /// Updates the enemy direction to a random direction where the 
        /// gridspace in that direction is open
        /// </summary>
        /// <param name="floorPlan">For the layout of the obstacles</param>
        /// <returns>True if the enemy can move, false if the enemy is surrounded by obstacles</returns>
        private bool MoveRandomly(int[,] floorPlan)
        {
            Random r = new Random();
            int i;
            bool canMakeMovement = false;   // false if player is surrounded, true otherwise
            bool[] canMoveInDirection = new bool[4] { false, false, false, false };

            // check if the gridspace to the right is open
            // the second part of the condition makes sure that we are not moving backwards
            if (floorPlan[(int)Position.X + 1, (int)-Position.Z] == 0 && 
                (Math.Abs(ForwardDirection - MathHelper.PiOver2) > 0.01f || speed == 0.0f))
                canMoveInDirection[0] = true;

            // check if the gridspace to the left is open
            // the second part of the condition makes sure that we are not moving backwards
            if (floorPlan[(int)Position.X - 1, (int)-Position.Z] == 0 && 
                (Math.Abs(ForwardDirection - 3 * MathHelper.PiOver2) > 0.01f || speed == 0.0f))
                canMoveInDirection[1] = true;

            // check if the gridspace above is open
            // the second part of the condition makes sure that we are not moving backwards
            if (floorPlan[(int)Position.X, (int)-Position.Z + 1] == 0 && 
                (Math.Abs(ForwardDirection - MathHelper.Pi) > 0.01f || speed == 0.0f))
                canMoveInDirection[2] = true;

            // check if the gridspace below is open
            // the second part of the condition makes sure that we are not moving backwards
            if (floorPlan[(int)Position.X, (int)-Position.Z - 1] == 0 && 
                (Math.Abs(ForwardDirection - 0.0f) > 0.01f || speed == 0.0f))
                canMoveInDirection[3] = true;

            // update whether or not we are surrounded
            if (canMoveInDirection[0] == true || canMoveInDirection[1] == true ||
                canMoveInDirection[2] == true || canMoveInDirection[3] == true)
                canMakeMovement = true;

            // if we are not surrounded, pick a random direction which we can move in
            if (canMakeMovement)
            {
                while (true)
                {
                    // generate random number between 0 and 3 and if we can 
                    // move in that direction, update the ForwardDirection property
                    i = r.Next(4);

                    if (canMoveInDirection[i])
                    {
                        if (i == 0)
                            ForwardDirection = 3 * MathHelper.PiOver2;
                        else if (i == 1)
                            ForwardDirection = MathHelper.PiOver2;
                        else if (i == 2)
                            ForwardDirection = 0;
                        else
                            ForwardDirection = MathHelper.Pi;

                        // we picked a direction, now break
                        break;
                    }
                }
            }

            return canMakeMovement; // return false if surrounded, true otherwise
        }
        
        const int EnemyMark = 100;
        /// <summary>
        /// Mark spaces on the grid with the positions enemies are at and 
        /// the positions of their next move
        /// </summary>
        /// <param name="enemies">All of the enemies in the game</param>
        /// <param name="floorPlan">Layout of the map</param>
        private void MarkEnemySquares(Enemy[] enemies, int[,] floorPlan)
        {
            foreach (Enemy e in enemies)
            {
                if (e != this) // don't mark your own spot!
                {
                    floorPlan[(int)e.Position.X, (int)-e.Position.Z] = EnemyMark;

                    // no need to mark building
                    if (floorPlan[(int)e.nextPosition.X, (int)-e.nextPosition.Y] == 0)
                    {
                        floorPlan[(int)e.nextPosition.X, (int)-e.nextPosition.Y] = EnemyMark;
                    }
                }
            }
        }

        /// <summary>
        /// Unmark all spaces on the grid where the enemies are at and 
        /// the positions of their next move
        /// </summary>
        /// <param name="enemies">All of the enemies in the game</param>
        /// <param name="floorPlan">Layout of the map</param>
        private void UnmarkEnemySquares(Enemy[] enemies, int[,] floorPlan)
        {
            foreach (Enemy e in enemies)
            {
                if (e != this) // don't mark your own spot!
                {
                    floorPlan[(int)e.Position.X, (int)-e.Position.Z] = 0;

                    // make sure we don't remove the mark from a building
                    if (floorPlan[(int)e.nextPosition.X, (int)-e.nextPosition.Y] == EnemyMark)
                        floorPlan[(int)e.nextPosition.X, (int)-e.nextPosition.Y] = 0;
                }
            }
        }

        /// <summary>
        /// Reset the enemy to the state before play began
        /// </summary>
        public void Reset()
        {
            positionOfLastMove = new Rectangle(0, 0, 1, 1); 
            chasing = false; 
            nextPosition = new Rectangle(0, 0, 1, 1);
            speed = 0.0f;
            ForwardDirection = 45.0f;

            soundEffects.StopAllSounds();
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