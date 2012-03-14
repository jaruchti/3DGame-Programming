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
    static class GameConstants
    {
        public enum GameState { Title, Intro, Ready, Playing, End };
        public enum CollisionType { None, Building, Fuel };

        //player constants

        public static readonly Vector3 LightDirection = new Vector3(3/6.164414f, -2/6.164414f, 5/6.164414f);

        // Enemy constants
        public const int NumEnemy = 4;
        public const float EnemyVelocity = 0.8f / 60.0f;

        // Intro constants
        public const int IntroAltitude = 8;
        public const int IntroRadius = 3;
        public const float IntroVelocity = 2.0f / 60.0f;
        public const float AngularVelocity = MathHelper.Pi / 60;
        public static readonly Vector3 IntroCenter = new Vector3(9.5f, 0.0f, -9.5f);

        public static int ViewportWidth;
        public static int ViewportHeight;
    }
}
