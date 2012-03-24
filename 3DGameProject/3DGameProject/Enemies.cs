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
    /// Wrapper class to manage the enemies in the game
    /// </summary>
    /// <remarks>This is a singleton in the game</remarks>
    public class Enemies
    {
        /// <summary>Number of enemies the player must avoid</summary>
        public const int NumEnemy = 5;

        private Enemy[] enemies;
        private EnemyWarningScreen warningScreen;   // screen with information for the player on enemy activity

        /// <summary>
        /// Property allow the client of this class to access the warning screen for drawing
        /// </summary>
        public EnemyWarningScreen WarningScreen
        {
            get { return warningScreen; }
        }

        /// <summary>
        /// Create the enemies in the game
        /// </summary>
        public Enemies()
        {
            enemies = new Enemy[NumEnemy];
            for (int i = 0; i < enemies.Length; i++)
                enemies[i] = new Enemy();

            warningScreen = new EnemyWarningScreen();

            SetUpEnemyPositions();
        }

        /// <summary>
        /// Load the enemies
        /// </summary>
        /// <param name="device">To load the enemy warning screen</param>
        /// <param name="content">Content pipeline (for models)</param>
        public void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            for (int i = 0; i < enemies.Length; i++)
                enemies[i].LoadContent(ref device, content);

            warningScreen.LoadContent(ref device, content);
        }

        /// <summary>
        /// Place the enemies in their initial positions at the beginning of play.
        /// </summary>
        public void SetUpEnemyPositions()
        {
            enemies[0].UpdatePositionAndBoundingSphere(new Vector3(9.5f, 0.18f, -8.5f));
            enemies[1].UpdatePositionAndBoundingSphere(new Vector3(8.5f, 0.18f, -9.5f));
            enemies[2].UpdatePositionAndBoundingSphere(new Vector3(9.5f, 0.18f, -9.5f));
            enemies[3].UpdatePositionAndBoundingSphere(new Vector3(9.5f, 0.18f, -10.5f));
            enemies[4].UpdatePositionAndBoundingSphere(new Vector3(15.5f, 0.18f,-4.5f));
        }

        /// <summary>
        /// Allows the client to use a foreach construct to iterate through the enemies
        /// </summary>
        /// <returns>IEnumerator for the enemy objects</returns>
        public IEnumerator<Enemy> GetEnumerator()
        {
            foreach (Enemy e in enemies)
                yield return e;
        }

        /// <summary>
        /// Update the positions of the enemies and updates the warning screen if enemies
        /// are close or chasing the player
        /// </summary>
        /// <param name="player">For the position of the player</param>
        /// <param name="floorPlan">For the arrangment of the obstacles</param>
        /// <param name="gameState">Current state of the game</param>
        /// <param name="gameTime">Information on the time since last update for missiles</param>
        /// <remarks>
        /// The enemies are intelligent and will chase the player when they spot the car.
        /// Also, the enemies will not run into each other when moving
        /// If the player is caught, the gameState will transition to end
        /// </remarks>
        public void Update(Player player, int[,] floorPlan, GameTime gameTime, ref GameConstants.GameState gameState)
        {
            if (gameState != GameConstants.GameState.End) // the game is not over
            {
                // Update the positions of the enemies
                foreach (Enemy e in enemies)
                    e.Update(enemies, player, floorPlan, gameTime, ref gameState);

                // Update the warning screen if we haven't transitioned to the end of the game
                // during this update
                if (gameState != GameConstants.GameState.End)
                {
                    UpdateWarningScreen(player);
                }
            }
            else
            {
                // we are in the game over state, have the enemy that caught the player move toward
                // the player
                foreach (Enemy e in enemies)
                {
                    if (((int)e.Position.X - (int)player.Position.X) == 0 &&
                        ((int)e.Position.Z - (int)player.Position.Z) == 0)
                    {
                        e.MoveTowardPlayer(player);
                    }
                }

                warningScreen.Reset();
            }
        }

        /// <summary>
        /// Updates the warning screen with a message to player if enemies are close or following
        /// </summary>
        /// <param name="player">For the position of the player</param>
        private void UpdateWarningScreen(Player player)
        {
            float distancePlayerToEnemy = 0.0f;

            // If an enemy is very close, display "Enemy Close" in red
            foreach (Enemy e in enemies)
            {
                distancePlayerToEnemy = (float)Math.Sqrt(
                    (e.Position.X - player.Position.X) * (e.Position.X - player.Position.X) +
                    (e.Position.Z - player.Position.Z) * (e.Position.Z - player.Position.Z));

                if (distancePlayerToEnemy < 2.0f)
                {
                    warningScreen.Update("Enemy Close", Color.Red, false);
                    return;
                }
            }

            // If an enemy is firing at the player, display "Locked On" in red
            foreach (Enemy e in enemies)
            {
                if (e.LockedOn == true)
                {
                    warningScreen.Update("Locked On", Color.Red, false);
                    return;
                }
            }

            // If an enemy is chasing, display "Locking" in flashing red
            foreach (Enemy e in enemies)
            {
                if (e.Chasing == true)
                {
                    warningScreen.Update("Locking", Color.Red, true);
                    return;
                }
            }

            // If an enemy is fairly close, display "Warning" in yellow
            foreach (Enemy e in enemies)
            {
                distancePlayerToEnemy = (float)Math.Sqrt(
                    (e.Position.X - player.Position.X) * (e.Position.X - player.Position.X) +
                    (e.Position.Z - player.Position.Z) * (e.Position.Z - player.Position.Z));

                if (distancePlayerToEnemy < 5.0f)
                {
                    warningScreen.Update("Warning", Color.Yellow, false);
                    return;
                }
            }

            // If we reached here, there is no need for a warning so reset the warning screen
            warningScreen.Reset();
        }

        /// <summary>
        /// Draw the enemies to the screen.
        /// </summary>
        /// <param name="device">Graphics card (to draw enemy missiles)</param>
        /// <param name="gameCamera">For view and projection matrices</param>
        public void Draw(ref GraphicsDevice device, Camera gameCamera)
        {
            foreach (Enemy e in enemies)
                e.Draw(ref device, gameCamera);
        }

        /// <summary>
        /// Reset enemies to the state just before play begins.
        /// </summary>
        public void Reset()
        {
            foreach (Enemy e in enemies)
                e.Reset();
            warningScreen.Reset();
            SetUpEnemyPositions();
        }

        /* 
        * ----------------------------------------------------------------------------
        * The following section is for the introduction 
        * --------------------------------------------------------------------------- 
        */

        /// <summary>
        /// Sets up the position of an enemy for the introduction
        /// </summary>
        /// <param name="playerPosition">Position of the player at the beginning of the introduction</param>
        public void SetUpIntroPositions(Vector3 playerPosition)
        {
            enemies[4].Position = playerPosition + new Vector3(0.0f, 0.5f, 0.1f); // fly above and slightly behind player
            enemies[4].AngularPosition = 3 * MathHelper.PiOver2; // start circle behind player
            enemies[4].SetIntroSpeed(); // move as fast a player
        }

        /// <summary>
        /// Method to play the enemy motions during the introduction
        /// </summary>
        /// <param name="player">For the position of the player</param>
        /// <remarks>
        /// The enemy will descent to the player and then circle
        /// </remarks>
        public void PlayIntro(Player player)
        {
            if (enemies[4].Position.Y > 0.18f)
            {
                // Move with the player during the beginning of the introduction while descending
                enemies[4].MoveTowardPlayer(player);
                enemies[4].Position = new Vector3(enemies[4].Position.X, enemies[4].Position.Y - 0.005f, enemies[4].Position.Z);
            }
            else
            {
                // we have descended enough, now circle the player
                enemies[4].Circle(player.Position, 0.1f);
            }
        }

        /// <summary>
        /// Property to allow the client to get the position of the enemy following the player
        /// during the introduction
        /// </summary>
        /// <remarks>
        /// This is used by the camera class to set-up the camera following the enemy
        /// </remarks>
        public Vector3 EnemyIntroPosition
        {
            get { return enemies[4].Position; }
        }
    }
}
