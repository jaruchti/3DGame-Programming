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
    abstract class ScoreDisplay : InGameDisplay
    {
        protected void SetUpDigitContants()
        {
            NumDisplayDigits = 5;
            DigitYOffset = 2;
            DigitWidth = 20;
            DigitScale = 0.2f;
        }

        public float Score { 
            get { return digits; }
            set { digits = value; }
        }
    }
}
