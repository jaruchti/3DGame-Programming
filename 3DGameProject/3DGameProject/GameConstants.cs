﻿using System;
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
        public enum CollisionType { None, Building, Fuel };

        //camera constants
        public const float NearClip = 0.2f;
        public const float FarClip = 500.0f;
        public const float ViewAngle = 45.0f;

        //player constants
        public const float MaxVelocity = 1.25f / 30.0f;
        public const float TurnSpeed = 0.7f;

        public static readonly Vector3 LightDirection = new Vector3(3/6.164414f, -2/6.164414f, 5/6.164414f);
    }
}
