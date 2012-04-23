/*
 * 3D Game Programming Project
 * Dr. Liu
 * Zach Bates, Lauren Buss, Corey Darr, Jason Ruchti, Jared Tittle
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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
        public const float EnemyBoundingSphereRatio = 1.0f;

        /// <summary>
        /// Property which defines the direction the enemy is moving.
        /// </summary>
        public float ForwardDirection { get; set; }

        private float speed; // current speed of the enemy craft (either EnemyVelocity or 0)        
        private Rectangle positionOfLastMove = new Rectangle(0, 0, 1, 1); // rectangle where the enemy made the last move decision
        private bool chasing = false;   // status as to whether the enemy is chasing the player
        private Rectangle nextPosition = new Rectangle(0, 0, 1, 1); // rectangle the enemy decided to head towards on the last move decision
        private Missiles missiles;    // missiles the enemy has fired at the player
        private double timeOfChase;   // total time the enemy has been chasing the player

        /// <summary>
        /// Allows the client to query the status of the enemy 
        /// craft in regards to whether or not it is chasing the player
        /// </summary>
        public bool Chasing
        {
            get { return chasing; }
        }

        /// <summary>
        /// Allows the client to query the status of the enemy 
        /// craft in regards to whether or not it is firing at the player
        /// </summary>
        public bool LockedOn
        {
            get { return (timeOfChase > 1.5f); }
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
            speed = EnemySpeed;
            ForwardDirection = 45.0f;
            timeOfChase = 0;

            missiles = new Missiles();
        }

        /// <summary>
        /// Used to load the model to represent the enemy on screen and set up enemy's bounding sphere.
        /// </summary>
        /// <param name="device">For the missile texture</param>
        /// <param name="content">Content pipeline</param>
        public void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            // Load model
            Model = content.Load<Model>("Models/UFO");

            // Load missile effects
            missiles.LoadContent(ref device, content);

            // Setup bounding sphere (scale for best fit)
            BoundingSphere = CalculateBoundingSphere();
            BoundingSphere scaledSphere = BoundingSphere;
            scaledSphere.Radius = BoundingSphere.Radius * EnemyBoundingSphereRatio;
            BoundingSphere = scaledSphere;
        }

        /// <summary>
        /// Allows the client to move the enemy craft
        /// </summary>
        /// <param name="enemies">For positions of all of the other enemies in the game</param>
        /// <param name="player">Allows queries for attributes of the player</param>
        /// <param name="floorPlan">Gives the arrangement of the building in the city</param>
        ///<param name="gameTime">Information on the time since last update for missiles</param>
        /// <param name="gameState">Gives the current state of the game</param>
        /// <remarks>
        /// If the player is caught or the player is hit by a missile and runs
        /// out of health, the gamestate will transition to end
        /// </remarks>
        public void Update(Enemies enemies, Player player, int[,] floorPlan, GameTime gameTime, 
                           ref GameConstants.GameState gameState)
        {
            bool canMakeMovement = false;   // boolean for status if the player was able to make a move on this update
            Vector3 movement;               // the movement to be applied to the enemy

            // check if the player has been caught, that is the enemy craft and player are in the same 
            // grid unit, and if so transition to game ending state
            if (((int)Position.X - (int)player.Position.X) == 0 && ((int)Position.Z - (int)player.Position.Z) == 0)
            {
                gameState = GameConstants.GameState.End;
                return;
            }

            // Update the missiles and time during chase
            missiles.Update(player, floorPlan, ref gameState);
            if (chasing)
            {
                // Update the timeOfChase with the elapsed time since the last update
                timeOfChase += gameTime.ElapsedGameTime.TotalSeconds;
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

                    // Check if we are locked on to the player, and if so, attempt to fire
                    if (this.LockedOn)
                    {
                        missiles.FireAt(this.Position, FindTargetPosition(player), gameTime);
                    }
                }
                else
                {
                    // reset the time of chase to zero since we are not chasing
                    timeOfChase = 0.0;

                    // check to see if no other enemyies are chasing and this is the closest enemy
                    // in terms of linear distance
                    // If so, find an A* path to the player if possible.
                    // This makes the game more challenging since the player will always be under pressure
                    // from some enemy
                    if (ShouldMoveTowardPlayer(player, enemies))
                    {
                        List<Vector2> aStarPath = AStar.FindPath(floorPlan, Position, player.Position);
                        //Helpers.AStarDebugList = aStarPath;

                        if (aStarPath == null)
                        {
                            // We could not find an A* path to the player, move randomly
                            canMakeMovement = MoveRandomly(floorPlan); // returns true if a move was able to be made
                        }
                        else
                        {
                            // set the enemy direction to follow the A* path
                            UpdateDirectionFromPath(aStarPath);
                            canMakeMovement = true;
                        }
                    }
                    else
                    {
                        // we are not chasing or the closest enemy
                        // move randomly around the grid until we spot the player next
                        canMakeMovement = MoveRandomly(floorPlan); // returns true if a move was able to be made
                    }
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
        /// Check whether the enemy ready to make decision on where to move next
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
            int xOffsetPToE = (int)player.Position.X - (int)Position.X; // x displacement from player to enemy
            int zOffsetPToE = (int)-player.Position.Z + (int)Position.Z;// z displacement from player to enemy

            if (zOffsetPToE != 0)
            {
                // Must move left or right
                if (zOffsetPToE > 0)
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
                if (xOffsetPToE > 0)
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
        /// Returns true if this enemy is the closest enemy to the player in terms
        /// of linear distance and no other enemy is chasing, false otherwise
        /// </summary>
        /// <param name="player">For the player's position</param>
        /// <param name="enemies">For the position of the enemies</param>
        private bool ShouldMoveTowardPlayer(Player player, Enemies enemies)
        {
            float distanceToPlayer = Helpers.LinearDistance2D(Position, player.Position); 

            foreach (Enemy e in enemies)
            {
                if (e != this)
                {
                    if (distanceToPlayer > Helpers.LinearDistance2D(e.Position, player.Position) || 
                        e.chasing == true)
                        return false;
                }
            }

            return true;
        }

        private void UpdateDirectionFromPath(List<Vector2> aStarPath)
        {
            Vector2 nextPosition = (aStarPath.Count > 1) ? aStarPath[aStarPath.Count - 2] : aStarPath[0];
            int xOffsetToNextPos = (int)nextPosition.X - (int)Position.X;
            int zOffsetToNextPos = (int)nextPosition.Y + (int)Position.Z; 
            // Note: (int)nextPosition.Y maps to a z position in the 3D coordinate system used
            // in the game

            if (zOffsetToNextPos != 0)
            {
                // Must move left or right
                if (zOffsetToNextPos > 0)
                {
                    // Move left
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
                if (xOffsetToNextPos > 0)
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
        
        public const int EnemyMark = 100;
        /// <summary>
        /// Mark spaces on the grid with the positions enemies are at and 
        /// the positions of their next move
        /// </summary>
        /// <param name="enemies">All of the enemies in the game</param>
        /// <param name="floorPlan">Layout of the map</param>
        private void MarkEnemySquares(Enemies enemies, int[,] floorPlan)
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
        private void UnmarkEnemySquares(Enemies enemies, int[,] floorPlan)
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
        /// This method implements dead reckoning.  Based on the position of the player, the velocity
        /// of the player, and the velocity of the missile, the target position
        /// where the missile should be fired to strike the enemy is calculated.
        /// </summary>
        /// <param name="player">For the player position</param>
        /// <returns>
        /// A vector representing the target position the enemy should fire
        /// a missile.
        /// </returns>
        private Vector3 FindTargetPosition(Player player)
        {
            Vector3 target = new Vector3(0.0f, player.Position.Y, 0.0f);

            // Uses an approximation to determine where to fire the misile
            // 1) Calculate the distance between user and the missile
            // 2) Calculate the time (t) for the missile to travel to the user given the speed of the missile
            // 3) Calculate the position of the player at time (t)

            float d_enemyToPlayer = Helpers.LinearDistance2D(player.Position, this.Position); // 1
            float t_missiletoplayer = d_enemyToPlayer / Missile.MissileSpeed; // 2
            target.X = player.Position.X - player.Velocity * (float)Math.Sin(player.ForwardDirection) * t_missiletoplayer; // 3
            target.Z = player.Position.Z - player.Velocity * (float)Math.Cos(player.ForwardDirection) * t_missiletoplayer; // 3
            return target;
        }

        /// <summary>
        /// Moves the enemy towards the player when the game is over or during the introduction
        /// </summary>
        /// <param name="player">For the position of the player</param>
        /// <remarks>
        /// This method is called when the game is over or during the intrdocution.  
        /// It is known that this enemy and the player are in valid positions.  Thus, no checks are
        /// made to ensure the location is valid.
        /// </remarks>
        public void MoveTowardPlayer(Player player)
        {
            Vector3 movement;
            float distancePlayerToEnemy = Helpers.LinearDistance2D(this.Position, player.Position);

            if (distancePlayerToEnemy > speed)
            {
                // we are atleast one timestep away from the player, move toward the player

                // find direction from this enemy's position to the player
                ForwardDirection = (float) Math.Atan2(Position.X - player.Position.X, -player.Position.Z + Position.Z);

                // Make the movement
                movement = Vector3.Transform(new Vector3(0.0f, 0.0f, -1.0f), Matrix.CreateRotationY(ForwardDirection));
                movement *= speed;
                UpdatePositionAndBoundingSphere(Position + movement);
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
            speed = EnemySpeed;
            ForwardDirection = 45.0f;
            timeOfChase = 0;

            missiles.Reset();
        }

        /// <summary>
        /// Draw the enemy model and missiles
        /// </summary>
        /// <param name="device">Graphics card to draw the missiles</param>
        /// <param name="gameCamera">For view and projection matrices</param>
        /// <param name="gameState">For the state of the game (do not draw missiles once game over)</param>
        public void Draw(ref GraphicsDevice device, Camera gameCamera, GameConstants.GameState gameState)
        {
            // Draw the enemy model
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

            // Draw the missiles if we are still playing
            if (gameState == GameConstants.GameState.Playing)
                missiles.Draw(ref device, gameCamera);
        }


        /* 
        * ----------------------------------------------------------------------------
        * The following section is for the introduction 
        * --------------------------------------------------------------------------- 
        */

        /// <summary>
        /// Property to allow retrieval/setting of the angular position of the enemy
        /// when it is circling a position
        /// </summary>
        /// <remarks>
        /// This property is used during the introduction to allow the enemy to circle the
        /// player
        /// </remarks>
        public float AngularPosition { get; set; }

        /// <summary>
        /// This method should be called during each update to allow the enemy to circle
        /// a given position
        /// </summary>
        /// <param name="center">Position to circle around</param>
        /// <param name="radius">Radius of the circle</param>
        public void Circle(Vector3 center, float radius)
        {
            // Change the angular position of the drone based on the elapsed time since last update
            // and the angular velocity
            AngularPosition += Player.IntroVelocity;

            // Normalize angularPosition so it is between 0 and 2 pi.
            if (AngularPosition > 2 * MathHelper.Pi)
                AngularPosition = AngularPosition - 2 * MathHelper.Pi;

            // Transform the polor coordinates given by the drone's angular position and radius into
            // Cartesian coordinates, and store the values in the futurePosition vector.
            // The futurePosition vector holds the position the drone will be.
            Vector3 futurePosition = new Vector3(0, Position.Y, radius);
            futurePosition.X = radius * (float)Math.Cos(AngularPosition) + center.X;
            futurePosition.Z = -radius * (float)Math.Sin(AngularPosition) + center.Z;

            // Now, update the Position and ForwardDirection after the calculations for the update
            // have completed.
            Position = futurePosition;
            ForwardDirection = -AngularPosition;
        }

        /// <summary>
        /// Sets the enemy speed to that of the player during the introduction
        /// </summary>
        public void SetIntroSpeed()
        {
            speed = Player.IntroVelocity;
        }
    }

    /// <summary>
    /// Class which includes the implementation of the A* path finding algorithm.
    /// </summary>
    public class AStar
    {
        /// <summary>Cost to move a single square in the grid to the left, right, up, or down</summary>
        public const int MoveCost = 10;

        public class Node
        {
            private Node parent;    // parent of this node
            private Node goal;      // target for the A* search
            private int f;          // cost estimate from the start node to the destination
            private int g;          // accumulated cost from the start to this node
            private Vector2 loc;    // location of this node in the grid

            /// <summary>
            /// Property which allows the client to access and set the value of F, 
            /// the cost estimate from the start of the search to the destination
            /// </summary>
            public int F
            {
                get { return (int)f; }
                set { f = value; }
            }

            /// <summary>
            /// Property which allows the client to access and set the value of G, the 
            /// accumulated cost from the start to this node
            /// </summary>
            public int G
            {
                get { return (int)g; }
                set { g = value; }
            }

            /// <summary>
            /// Property which allows the client to set the parent of this node
            /// </summary>
            public Node Parent
            {
                get { return parent; }
                set { parent = value; }
            }

            /// <summary>
            /// Property which allows the client to access the X position of this
            /// node in the graph.
            /// </summary>
            public int X
            {
                get { return (int)loc.X; }
                set { loc.X = value; }
            }

            /// <summary>
            /// Property which allows the client to access the Y position of this
            /// node in the graph.
            /// </summary>
            public int Y
            {
                get { return (int)loc.Y; }
                set { loc.Y = value; }
            }

            /// <summary>
            /// Constructor for a new node in the grid
            /// </summary>
            /// <param name="parent">The parent of this node</param>
            /// <param name="goal">The destination for the A* search (used to find F)</param>
            /// <param name="loc">Location of this node in the grid.  Note: The coordinates should be integers</param>
            /// <param name="g">Accumulated cost from the start to this node</param>
            public Node(Node parent, Node goal, Vector2 loc, int g)
            {
                this.parent = parent;
                this.goal = goal;
                this.loc = loc;
                this.g = g;

                if (!(goal == null))
                {
                    f = g + FindManhattenDistanceToGoal();
                }
            }

            /// <summary>
            /// Used to get the Manhatten distance (street walking distance) between
            /// this node and the goal.
            /// </summary>
            public int FindManhattenDistanceToGoal()
            {
                int xOffset = Math.Abs(X - goal.X);
                int yOffset = Math.Abs(Y - goal.Y);
                return MoveCost * (xOffset + yOffset);
            }

            /// <summary>
            /// Used to test two Nodes for equality.  Equality is defined as
            /// each Node having the same X and Y coordinates.
            /// </summary>
            /// <param name="obj">The object we are testing for equality with</param>
            /// <returns>
            /// True if this node has the same X and Y coordinates as obj, false otherwise.
            /// </returns>
            public override bool Equals(Object obj)
            {
                bool r;

                if (this == obj) // aliases
                    r = true;
                else if (obj == null || GetType() != obj.GetType()) // Check for null values and compare run-time types.
                    r = false;
                else
                {
                    Node n = (Node)obj;
                    r = ((int)loc.X == (int)n.loc.X && (int)loc.Y == (int)n.loc.Y);
                }

                return r;
            }

            /// <summary>
            /// Used to get a hashcode representation of a node that conforms with the contract
            /// for Equals.
            /// </summary>
            /// <returns>
            /// An integer hash value for this object.
            /// </returns>
            public override int GetHashCode()
            {
                int r = 17;

                // hash the x and y positions
                r = 31 * r + (int)loc.X;
                r = 31 * r + (int)loc.Y;
 
                return r;
            }
        }

        /// <summary>
        /// Find a path from the start Vector3 to the end Vector3 using an A* search.
        /// </summary>
        /// <param name="floorPlan">Floor plan gives the position of obstacles in the map</param>
        /// <param name="start">The position to begin the start of the search</param>
        /// <param name="end">The position to end the search</param>
        /// <returns>
        /// A list of points representing the path that should be taken to move from the start
        /// to the end
        /// </returns>
        public static List<Vector2> FindPath(int[,] floorPlan, Vector3 start, Vector3 end)
        {
            List<Node> lst; // temporary list to the hold the value returned by openDict
            HashSet<Node> closedSet = new HashSet<Node>();  // set of squares we have already visited
            SortedDictionary<int, List<Node>> openDict = new SortedDictionary<int, List<Node>>();
            // The openDict stores what is commonly known as the open list in the A* literature
            // The openDict maps F cost estimates to a list of nodes which have this cost
            // Since a sorted dictionary is used, the nodes with the lowest F cost can be easily obtained

            // Note: The A* search transforms the 3D space represented by X and -Z coordinates into a 2D plane
            Node endNode = new Node(null, null, new Vector2(start.X, -start.Z), 0); // create node to end the search at
            Node startNode = new Node(null, endNode, new Vector2((int) end.X, (int) -end.Z), 0); // create node to start the search at; this node has no parent
            Node currNode = null; // current node to explore

            // Add the start node to the open list
            lst = new List<Node>();
            lst.Add(startNode);
            openDict.Add(startNode.F, lst);

            while (openDict.Count > 0)
            {
                // get the Node with the lowest F score
                lst = openDict[openDict.Keys.Min()];   
                currNode = lst[0];

                // Check to see if we have reached the end node
                // If so, reconstruct the path back to the start
                if (currNode.Equals(endNode))
                {
                    // Follow the parent references back to the start
                    List<Vector2> bestPath = new List<Vector2>();

                    while (currNode != null)
                    {
                        bestPath.Insert(0, new Vector2(currNode.X, currNode.Y));
                        currNode = currNode.Parent;
                    }

                    return bestPath;
                }

                // Remove the current node from the open list and move it to the closed list
                lst = openDict[openDict.Keys.Min()];
                lst.Remove(currNode);
                openDict[openDict.Keys.Min()] = lst;
                if (openDict[openDict.Keys.Min()].Count == 0)
                    openDict.Remove(openDict.Keys.Min());
                closedSet.Add(currNode);

                foreach (Node n in FindAdjacentNodes(floorPlan, openDict, currNode, endNode)) // Find the valid adjacent squares
                {
                    if (closedSet.Contains(n)) // we have already explored this node
                        continue;

                    if (n.G == 0)
                    {
                        // This node was not in the openDict.  When finding adjacent nodes,
                        // nodes that were not already found point in the search were marked
                        // with a G value of zero.
                        // Calcuate the accumulated cost for this node and store in G
                        // before calculating the cost estimate to the destination, F
                        n.G = currNode.G + MoveCost;
                        n.F = n.G + n.FindManhattenDistanceToGoal();
                        n.Parent = currNode;
                    }
                    else if (currNode.G + MoveCost < n.G)
                    {
                        // The path through this square is better using this route

                        // Remove this node from the open dict before updating
                        lst = openDict[n.F];
                        lst.Remove(n);
                        openDict[n.F] = lst;

                        // Update the cost estimate to the destination, F
                        n.G = currNode.G + MoveCost;
                        n.F = n.G + n.FindManhattenDistanceToGoal();
                        n.Parent = currNode;
                    }

                    // Add the node with the updated cost estimate to the openDict
                    if (!openDict.ContainsKey(n.F))
                        openDict.Add(n.F, new List<Node>());

                    lst = openDict[n.F];
                    lst.Add(n);
                    openDict[n.F] = lst;
                }
            }

            // No path to the destination could be found
            return null;
        }

        /// <summary>
        /// Returns a list of all nodes adjacent to the currNode parameter that are valid locations
        /// in the floor plan.
        /// </summary>
        /// <param name="fp">Used to determine which move positions are valid and which contain obstacles</param>
        /// <param name="openDict">
        /// The open list of nodes.  If an adjacent node is in the open list, it will be 
        /// returned with the correct values
        /// </param>
        /// <param name="currNode">Find nodes adjacent to this node</param>
        /// <param name="endNode">Destination node we are traveling towards (used to initialize new nodes)</param>
        private static List<Node> FindAdjacentNodes(int[,] fp, SortedDictionary<int, List<Node>> openDict,
                                                    Node currNode, Node endNode)
        {
            List<Node> adj = new List<Node>();  // list of valid adjacent nodes to return
            Node adjNode;   // a node position adjacent to the current Node
            Node t;         // temporary node

            // check left
            adjNode = new Node(currNode, endNode, new Vector2(currNode.X - 1, currNode.Y), 0);
            if (fp[adjNode.X, adjNode.Y] == 0 || fp[adjNode.X, adjNode.Y] == Enemy.EnemyMark)
            {
                // check to see if this node is already in the open list, and if so, update the adjNode
                // to point to it so the properties of the node are preset correctly
                if ((t = FindNodeInDict(adjNode, openDict)) != null)
                    adjNode = t;
                adj.Add(adjNode);
            }

            // check right
            adjNode = new Node(currNode, endNode, new Vector2(currNode.X + 1, currNode.Y), 0);
            if (fp[adjNode.X, adjNode.Y] == 0 || fp[adjNode.X, adjNode.Y] == Enemy.EnemyMark)
            {
                // check to see if this node is already in the open list, and if so, update the adjNode
                // to point to it so the properties of the node are preset correctly
                if ((t = FindNodeInDict(adjNode, openDict)) != null)
                    adjNode = t;
                adj.Add(adjNode);
            }

            // check down
            adjNode = new Node(currNode, endNode, new Vector2(currNode.X, currNode.Y - 1), 0);
            if (fp[adjNode.X, adjNode.Y] == 0 || fp[adjNode.X, adjNode.Y] == Enemy.EnemyMark)
            {
                // check to see if this node is already in the open list, and if so, update the adjNode
                // to point to it so the properties of the node are preset correctly
                if ((t = FindNodeInDict(adjNode, openDict)) != null)
                    adjNode = t;
                adj.Add(adjNode);
            }

            // check up
            adjNode = new Node(currNode, endNode, new Vector2(currNode.X, currNode.Y + 1), 0);
            if (fp[adjNode.X, adjNode.Y] == 0 || fp[adjNode.X, adjNode.Y] == Enemy.EnemyMark)
            {
                // check to see if this node is already in the open list, and if so, update the adjNode
                // to point to it so the properties of the node are preset correctly
                if ((t = FindNodeInDict(adjNode, openDict)) != null)
                    adjNode = t;
                adj.Add(adjNode);
            }

            return adj;
        }

        /// <summary>
        /// Allows the client to find a node in the openDict.
        /// </summary>
        /// <param name="nodeToFind">The node we are trying to find</param>
        /// <param name="openDict">Dictionary of nodes to search</param>
        /// <returns>
        /// A node in openDict with the same location, if one exists, null otherwise.
        /// </returns>
        private static Node FindNodeInDict(Node nodeToFind, SortedDictionary<int, List<Node>> openDict)
        {
            Node r = null;  // hold the result if we find the node
            List<Node> lst; // temporary list to hold the list of nodes with the same F cost

            // Loop though the open dict to find the node
            foreach (int k in openDict.Keys)
            {
                lst = openDict[k];
                if ((r = lst.Find(item => item.Equals(nodeToFind))) != null)
                    break;
            }

            return r;
        }
    }
}