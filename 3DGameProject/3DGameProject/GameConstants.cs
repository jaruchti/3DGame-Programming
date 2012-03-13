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
    class GameConstants
    {
        private GameConstants() { /* nothing to do here*/ }

        public enum GameState { Title, Intro, Ready, Playing, End };
        public enum CollisionType { None, Building, Fuel };

        //camera constants
        public const float NearClip = 0.2f;
        public const float FarClip = 500.0f;
        public const float ViewAngle = 45.0f;

        //player constants
        public const float MaxVelocity = 1.25f / 30.0f;
        public const float TurnSpeed = 0.7f;
        public const float Brake = -0.04f / 60f;
        public const float Accel = 0.015f / 60f;
        public const float Rev = -0.01f / 60f;
        public const float Friction = 0.002f / 60f;
        public const float MaxFuel = 99 + 59.0f / 60.0f;
        public static readonly Vector3 playerStartPos = new Vector3(15.5f, 0.0f, -9.5f);

        public static readonly Vector3 LightDirection = new Vector3(3/6.164414f, -2/6.164414f, 5/6.164414f);

        // Rank constants
        public const int AmatuerAbductee = 25;
        public const int MediocreAbductee = 100;
        public const int Abductee = 200;
        public const int MasterAbductee = 500;

        // Enemy constants
        public const int NumEnemy = 4;
        public const float EnemyVelocity = 0.8f / 60.0f;

        // Intro constants
        public const int IntroAltitude = 8;
        public const int IntroRadius = 3;
        public const float IntroVelocity = 2.0f / 60.0f;
        public const float AngularVelocity = MathHelper.Pi / 60;
        public static readonly Vector3 IntroCenter = new Vector3(9.5f, 0.0f, -9.5f);
    }
}
