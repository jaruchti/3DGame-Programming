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
    }
}
