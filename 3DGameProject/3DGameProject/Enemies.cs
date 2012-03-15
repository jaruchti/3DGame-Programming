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
        Enemy[] enemies;

        /// <summary>
        /// Create the enemies in the game
        /// </summary>
        public Enemies()
        {
            enemies = new Enemy[NumEnemy];
            for (int i = 0; i < enemies.Length; i++)
                enemies[i] = new Enemy();

            SetUpEnemyPositions();
        }

        /// <summary>
        /// Load the enemies
        /// </summary>
        /// <param name="content">Content pipeline (for models)</param>
        public void LoadContent(ContentManager content)
        {
            for (int i = 0; i < enemies.Length; i++)
                enemies[i].LoadContent(content);
        }

        /// <summary>
        /// Place the enemies in their initial positions at the beginning of play.
        /// </summary>
        public void SetUpEnemyPositions()
        {
            enemies[0].UpdatePositionAndBoundingSphere(new Vector3(9.5f, 0.2f, -8.5f));
            enemies[1].UpdatePositionAndBoundingSphere(new Vector3(8.5f, 0.2f, -9.5f));
            enemies[2].UpdatePositionAndBoundingSphere(new Vector3(9.5f, 0.2f, -9.5f));
            enemies[3].UpdatePositionAndBoundingSphere(new Vector3(9.5f, 0.2f, -10.5f));
            enemies[4].UpdatePositionAndBoundingSphere(new Vector3(15.5f, 0.2f,-4.5f));
        }

        /// <summary>
        /// Allows the client to use a foreach construct to iterate through the enemies
        /// </summary>
        /// <returns>IEnumerator for the enemy object</returns>
        public IEnumerator<Enemy> GetEnumerator()
        {
            foreach (Enemy e in enemies)
                yield return e;
        }

        /// <summary>
        /// Update the positions of the enemies
        /// </summary>
        /// <param name="player">For the position of the player</param>
        /// <param name="floorPlan">For the arrangment of the obstacles</param>
        /// <param name="gameState">Current state of the game</param>
        /// <remarks>
        /// The enemies are intelligent and will chase the player when they spot the car.
        /// Also, the enemies will not run into each other when moving
        /// If the player is caught, the gameState will transition to end
        /// </remarks>
        public void Update(Player player, int[,] floorPlan, ref GameConstants.GameState gameState)
        {
            foreach (Enemy e in enemies)
                e.Update(enemies, player, floorPlan, ref gameState);
        }

        /// <summary>
        /// Draw the enemies to the screen.
        /// </summary>
        /// <param name="gameCamera">For view and projection matrices</param>
        public void Draw(Camera gameCamera)
        {
            foreach (Enemy e in enemies)
                e.Draw(gameCamera);
        }

        /// <summary>
        /// Reset enemies to the state just before play begins.
        /// </summary>
        public void Reset()
        {
            foreach (Enemy e in enemies)
                e.Reset();
            SetUpEnemyPositions();
        }

        /* 
        * ----------------------------------------------------------------------------
        * The following section is for the introduction 
        * --------------------------------------------------------------------------- 
        */

        public void SetUpIntroPositions(Vector3 playerPosition)
        {
            enemies[0].Position = playerPosition + new Vector3(0.0f, 0.2f, 0.0f);
        }

        public void PlayIntro(Vector3 playerPosition)
        {
            enemies[0].circle(playerPosition, 0.1f);
        }
    }
}
