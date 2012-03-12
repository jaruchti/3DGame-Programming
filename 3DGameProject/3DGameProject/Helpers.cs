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
    class Helpers
    {
        private Helpers()
        {
            // no instances can be created with private constructor
        }

        public static Rectangle GetDigitRect(int digit)
        {
            return new Rectangle(GameConstants.DigitXPos + digit * GameConstants.DigitWidth, 
                GameConstants.DigitYPos, 
                GameConstants.DigitWidth, 
                GameConstants.DigitHeight);
        }

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
