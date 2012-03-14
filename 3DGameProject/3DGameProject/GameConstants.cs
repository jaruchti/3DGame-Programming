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
    /// Defines a number of constants that are used throughout the game.
    /// </summary>
    public static class GameConstants
    {
        /// <summary>
        /// Enumeration which describes the current state the game is in.
        /// </summary>
        /// <remarks>
        /// There should only be a single instance of GameState at one time.
        /// Use carefully, with great power comes great responbibility.
        /// </remarks>
        public enum GameState { Title, Intro, Ready, Playing, End };

        /// <summary>
        /// Enemeration which defines the type of collision that has occured.
        /// </summary>
        public enum CollisionType { None, Building, Fuel };

        /// <summary>
        /// Direction the light comes from on the map.
        /// </summary>
        public static readonly Vector3 LightDirection = new Vector3(3/6.164414f, -2/6.164414f, 5/6.164414f);

        /// <summary>Width of the viewport (constant)</summary>
        public static int ViewportWidth;
        /// <summary>Height of the viewport (constant)</summary>
        public static int ViewportHeight;
    }
}
