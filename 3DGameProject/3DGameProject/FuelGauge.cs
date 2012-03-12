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
    class FuelGauge : Gauges
    {
        public FuelGauge()
        {
            textureRect = new Rectangle(0, 103, 243, 102);
            displayDrawRect = new Rectangle(0, 440, 125, 60);

            FirstDigitXOffset = 0;
            SetUpDigitContants();
            SetUpDigitPositions();
        }
        public override void Update(float playerFuel)
        {
            digits = (int)Math.Floor(playerFuel);
        }
    }
}
