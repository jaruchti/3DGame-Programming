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
    abstract class Gauges : InGameDisplay
    {
        protected void SetUpDigitContants()
        {
            NumDisplayDigits = 2;
            DigitScale = 0.4f;
            DigitYOffset = 445;
            DigitWidth = 35;
        }
    }
}
