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
    class Enemies
    {
        Enemy[] enemies;

        public Enemies()
        {
            enemies = new Enemy[GameConstants.NumEnemy];
            for (int i = 0; i < enemies.Length; i++)
                enemies[i] = new Enemy();

            SetUpEnemyPositions();
        }

        public void LoadContent(ContentManager content)
        {
            for (int i = 0; i < enemies.Length; i++)
                enemies[i].LoadContent(content);
        }

        public void LoadFloorPlan(int[,] floorPlan)
        {
            for (int i = 0; i < enemies.Length; i++)
                enemies[i].LoadFloorPlan(floorPlan);
        }

        public void SetUpIntroPositions(Vector3 playerPosition)
        {
            enemies[0].Position = playerPosition + new Vector3(0.0f, 0.2f, 0.0f);
        }

        public void PlayIntro(Vector3 playerPosition)
        {
            enemies[0].circle(playerPosition, 0.1f);
        }

        public void SetUpEnemyPositions()
        {
            enemies[0].UpdatePositionAndBoundingSphere(new Vector3(9.5f, 0.1f, -8.5f));
            enemies[1].UpdatePositionAndBoundingSphere(new Vector3(8.5f, 0.1f, -9.5f));
            enemies[2].UpdatePositionAndBoundingSphere(new Vector3(9.5f, 0.1f, -9.5f));
            enemies[3].UpdatePositionAndBoundingSphere(new Vector3(9.5f, 0.1f, -10.5f));
        }

        public Enemy[] getEnemiesArray(){
            return enemies;
        }   

        public void Draw(Camera gameCamera)
        {
            foreach (Enemy e in enemies)
                e.Draw(gameCamera);
        }

        public void Reset()
        {
            SetUpEnemyPositions();
        }
    }
}
