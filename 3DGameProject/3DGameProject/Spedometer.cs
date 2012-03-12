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
    class Spedometer : Gauges
    {
        public Spedometer()
        {
            textureRect = new Rectangle(0, 0, 243, 102);
            displayDrawRect = new Rectangle(375, 440, 125, 60);

            FirstDigitXOffset = 435;
            SetUpDigitContants();
            SetUpDigitPositions();
        }

        public override void Update(float playerVelocity)
        {
            playerVelocity = Math.Abs(playerVelocity);
            digits = (int)Math.Floor(playerVelocity / GameConstants.MaxVelocity * 100);

            if (digits == 100)
                digits--;
        }
    }
}
