using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _3DGameProject
{
    class HighScore : ScoreDisplay
    {
        TextReader tr;
        TextWriter tw;

        public HighScore()
        {
            textureRect = new Rectangle(382, 68, 339, 60);
            displayDrawRect = new Rectangle(320, 0, 180, 30);

            FirstDigitXOffset = 390;
            SetUpDigitContants();
            SetUpDigitPositions();

            Score = ReadHighScore();
        }

        public override void Update(float newHighScore)
        {
            if (newHighScore > Score)
            {
                Score = newHighScore;
                WriteHighScore();
            }
        }

        private float ReadHighScore()
        {
            float r = 0.0f;

            tr = new StreamReader("Scores/scores.txt");
            r = (float) Convert.ToDouble(tr.ReadLine());

            return r;
        }

        private void WriteHighScore()
        {
            tw = new StreamWriter("Scores/scores.txt");
            tw.WriteLine(Score);
        }
    }
}
