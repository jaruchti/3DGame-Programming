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
    class Timer : ScoreDisplay
    {
        public Timer()
        {
            textureRect = new Rectangle(382, 2, 339, 60);
            displayDrawRect = new Rectangle(0, 0, 180, 30);

            FirstDigitXOffset = 70;
            SetUpDigitContants();
            SetUpDigitPositions();
        }

        public override void Update(float elapsedSecs)
        {
            Score += elapsedSecs;
            if (Score > GameConstants.MaxTime)
                Score = GameConstants.MaxTime;
        }
    }
}
