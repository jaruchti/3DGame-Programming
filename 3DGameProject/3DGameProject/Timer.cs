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
            DisplayDigits = 5;
            FirstDigitXOffset = 70;
            DigitYOffset = 2;
            DigitWidth = 20;

            textureRect = new Rectangle(382, 2, 339, 60);
            displayDrawRect = new Rectangle(0, 0, 180, 30);
            digitPositions = new Vector2[DisplayDigits];

            SetUpDigitPositions();
        }

        public void Update(float elapsedSecs)
        {
            score += elapsedSecs;
            if (score > GameConstants.MaxTime)
                score = GameConstants.MaxTime;
        }
    }
}
