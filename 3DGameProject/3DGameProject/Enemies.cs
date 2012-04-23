/*
 * 3D Game Programming Project
 * Dr. Liu
 * Zach Bates, Lauren Buss, Corey Darr, Jason Ruchti, Jared Tittle
 */

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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
        /// <summary>Height in the map at which the UFOs fly</summary>
        public const float Altitude = 0.135f;

        private Enemy[] enemies;
        private EnemyWarningScreen warningScreen;   // screen with information for the player on enemy activity
        private EnemySoundEffects soundEffects;     // sound effects for the enemy

        /// <summary>
        /// Property to allow the client of this class to access the warning screen for drawing
        /// </summary>
        public EnemyWarningScreen WarningScreen
        {
            get { return warningScreen; }
        }

        /// <summary>
        /// Create the enemies in the game and the singletons to manage resources common 
        /// to all enemies (such as the warning screen and sound effects).
        /// </summary>
        public Enemies()
        {
            enemies = new Enemy[NumEnemy];
            for (int i = 0; i < enemies.Length; i++)
                enemies[i] = new Enemy();

            warningScreen = new EnemyWarningScreen();
            soundEffects = new EnemySoundEffects();

            SetUpEnemyPositions();
        }

        /// <summary>
        /// Load content required for the enemies and the content for resources common
        /// to all enemies (such as the warning screen and sound effects).
        /// </summary>
        /// <param name="device">To load the enemy warning screen</param>
        /// <param name="content">Content pipeline (for models)</param>
        public void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            for (int i = 0; i < enemies.Length; i++)
                enemies[i].LoadContent(ref device, content);

            warningScreen.LoadContent(ref device, content);
            soundEffects.LoadContent(content);
        }

        /// <summary>
        /// Place the enemies in their initial positions at the beginning of play.
        /// </summary>
        public void SetUpEnemyPositions()
        {
            enemies[0].UpdatePositionAndBoundingSphere(new Vector3(9.5f, Altitude, -8.5f));
            enemies[1].UpdatePositionAndBoundingSphere(new Vector3(8.5f, Altitude, -9.5f));
            enemies[2].UpdatePositionAndBoundingSphere(new Vector3(9.5f, Altitude, -9.5f));
            enemies[3].UpdatePositionAndBoundingSphere(new Vector3(9.5f, Altitude, -10.5f));
            enemies[4].UpdatePositionAndBoundingSphere(new Vector3(15.5f, Altitude, -4.5f));
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
                    e.Update(this, player, floorPlan, gameTime, ref gameState);

                // Update the warning screen if we haven't transitioned to the end of the game
                // during this update.  If we have transitioned to the end of the game, reset the enemy alerts
                // to the state before play began
                if (gameState != GameConstants.GameState.End)
                {
                    UpdateEnemyAlerts(player);
                }
                else
                {
                    ResetEnemyAlerts();
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
        /// Updates the alerts to the user about the enemies
        /// </summary>
        /// <param name="player">For the position of the player</param>
        /// <remarks>
        /// The enemy warning screen is used to display info to the the player if an enemy is close,
        /// if an enemy is locking on, and if an enemy is firing at the player.  Additionally, sounds are
        /// played if the enemy is locked on or the enemy is locking.
        /// </remarks>
        private void UpdateEnemyAlerts(Player player)
        {
            float distancePlayerToEnemy = 0.0f;
            bool isChasing = false;         // boolean to hold whether an enemy is chasing or not
            bool isFiring = false;          // boolean to hold whether an enemy is firing or not
            bool screenUpdated = false;     // boolean to hold whether the warning screen has been updated

            // If an enemy is very close, display "Enemy Close" in red
            foreach (Enemy e in enemies)
            {
                distancePlayerToEnemy = Helpers.LinearDistance2D(e.Position, player.Position);

                if (distancePlayerToEnemy < 2.0f)
                {
                    warningScreen.Update("Enemy Close", Color.Red, false);
                    screenUpdated = true;
                }
            }

            // If an enemy is firing at the player, display "Locked On" in red
            foreach (Enemy e in enemies)
            {
                if (e.LockedOn == true)
                {
                    if (!screenUpdated) // the screen has not been updated yet, update it
                    {
                        warningScreen.Update("Locked On", Color.Red, false);
                        screenUpdated = true;
                    }
                    isFiring = true;
                }
            }

            // If an enemy is chasing, display "Locking" in flashing red
            foreach (Enemy e in enemies)
            {
                if (e.Chasing == true)
                {
                    if (!screenUpdated) // the screen has not been updated yet, update it
                    {
                        warningScreen.Update("Locking", Color.Red, true);
                        screenUpdated = true;
                    }
                    isChasing = true;
                }
            }

            // If an enemy is fairly close, display "Warning" in yellow
            if (!screenUpdated) // the screen has not been updated yet, check to see if we should display "Warning"
            {
                foreach (Enemy e in enemies)
                {
                    distancePlayerToEnemy = Helpers.LinearDistance2D(e.Position, player.Position);

                    if (distancePlayerToEnemy < 5.0f)
                    {
                        warningScreen.Update("Warning", Color.Yellow, false);
                    }
                }
            }

            if (!isChasing && !isFiring)
                soundEffects.StopAllSounds();
            else if (isFiring)
                soundEffects.PlayLockedOnBeep();
            else if (isChasing)
                soundEffects.PlayLockingBeep();

            if (!screenUpdated)
                warningScreen.Reset();
        }

        /// <summary>
        /// Draw the enemies to the screen.
        /// </summary>
        /// <param name="device">Graphics card (to draw enemy missiles)</param>
        /// <param name="gameCamera">For view and projection matrices</param>
        /// /// <param name="gameState">For the state of the game (do not draw missiles once game over)</param>
        public void Draw(ref GraphicsDevice device, Camera gameCamera, GameConstants.GameState gameState)
        {
            foreach (Enemy e in enemies)
                e.Draw(ref device, gameCamera, gameState);
        }

        /// <summary>
        /// Reset enemies to the state just before play begins.
        /// </summary>
        public void Reset()
        {
            foreach (Enemy e in enemies)
                e.Reset();
            SetUpEnemyPositions();
            ResetEnemyAlerts();
        }

        /// <summary>
        /// Reset the enemy alerts to the user (e.g. the warning screen and sound) to the state
        /// they were at before play began.
        /// </summary>
        private void ResetEnemyAlerts()
        {
            warningScreen.Reset();
            soundEffects.StopAllSounds();
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
            if (enemies[4].Position.Y > Altitude)
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
