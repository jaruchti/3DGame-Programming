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
    class Helpers
    {
        private Helpers()
        {
            // no instances can be created with private constructor
        }

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
    }
}
