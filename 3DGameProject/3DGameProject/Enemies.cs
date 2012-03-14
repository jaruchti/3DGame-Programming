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
    /// 
    /// </summary>
    class Enemies
    {
        /// <summary>Number of enemies the player must avoid</summary>
        public const int NumEnemy = 4;
        Enemy[] enemies;

        /// <summary>
        /// 
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
            enemies[0].UpdatePositionAndBoundingSphere(new Vector3(9.5f, 0.1f, -8.5f));
            enemies[1].UpdatePositionAndBoundingSphere(new Vector3(8.5f, 0.1f, -9.5f));
            enemies[2].UpdatePositionAndBoundingSphere(new Vector3(9.5f, 0.1f, -9.5f));
            enemies[3].UpdatePositionAndBoundingSphere(new Vector3(9.5f, 0.1f, -10.5f));
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
