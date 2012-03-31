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
    /// Class with a number of common functions used thoughout the project.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Efficient power function.
        /// </summary>
        /// <param name="x">Variable</param>
        /// <param name="n">Power</param>
        /// <returns>x^n</returns>
        /// <remarks>0^0 is undefined</remarks>
        public static double Pow(double x, int n)
        {
            double r = 1.0;

            if (n < 0)
            {
                n = -n;
                x = 1 / x;
            }

            while (n > 0)
            {
                if ((n & 1) == 1)
                    r *= x;
                x *= x;
                n >>= 1;
            }

            return r;
        }

        /// <summary>
        /// Function to get the distance between two Vector3 objects.
        /// </summary>
        /// <param name="v1">Vector 1</param>
        /// <param name="v2">Vector 2</param>
        /// <returns>Returns the linear 2D distance between two points</returns>
        /// <remarks>The distance is calculated by comparing x and z coordinates</remarks>
        public static float LinearDistance2D(Vector3 v1, Vector3 v2)
        {
            float xOffset = v1.X - v2.X;
            float zOffset = -v1.Z + v2.Z;
            float r = (float) Math.Sqrt(xOffset * xOffset + zOffset * zOffset);

            return r;
        }

        public static List<Vector2> AStarDebugList = new List<Vector2>();
    }
}
