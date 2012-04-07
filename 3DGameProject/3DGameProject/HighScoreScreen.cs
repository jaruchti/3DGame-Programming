/*
 * 3D Game Programming Project
 * Dr. Liu
 * Zach Bates, Lauren Buss, Corey Darr, Jason Ruchti, Jared Tittle
 */

using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace _3DGameProject
{
    /// <summary>
    /// Class which implements the logic to display a the relevant text
    /// to the screen when the user selects that he or she wishes to see
    /// the highscores.
    /// </summary>
    class HighScoreScreen : GameTextScreen
    {
        /// <summary>Maximum number of scores to display on the highscore screen</summary>
        public const int MAX_SCORES_TO_DISPLAY = 5;

        private Texture2D highScoreBackground;  // background texture
        private Rectangle viewportRect;         // describes the bounds of the screen
        private SpriteFont highScoreFont;       // font to draw the scores with

        private List<int> scores = new List<int>(); // list containing the scores player has achieved
        private string strTitle = "High Scores";    // title to this screen
        private string strInstructions = "Press H to Return"; // instructions for this screen

        private Vector2 titlePos; // defines the placement of the title on the page
        private List<Vector2> scorePositions = new List<Vector2>();
        private Vector2 instructionsPos;
        // the above list defines the placement of the highscores on the page

        /// <summary>
        /// Load the content required to display the HighScoreScreen.
        /// </summary>
        /// <param name="graphics">Graphics card (to initialize spritebatch)</param>
        /// <param name="content">Content pipeline (for fonts and highscore background)</param>
        public override void LoadContent(ref GraphicsDevice graphics, ContentManager content)
        {
            base.LoadContent(ref graphics, content);
            highScoreBackground = content.Load<Texture2D>("Textures/highscoreBackground");
            highScoreFont = content.Load<SpriteFont>("Fonts/HighScoreFont");

            // get the scores information and set up the positions on the screen
            ReadScoreInfo(); 
            SetPosition();
        }

        /// <summary>
        /// Set up the positions of the background, title, instructions, 
        /// and the scores on the highscores screen
        /// </summary>
        public override void SetPosition()
        {
            // set up viewport for background
            viewportRect = new Rectangle(0, 0, GameConstants.ViewportWidth, GameConstants.ViewportHeight);

            // set up position for title
            textSize = largeFont.MeasureString(strTitle);
            titlePos = new Vector2(
                (int)(GameConstants.ViewportWidth / 2) - textSize.X / 2,
                (int)(0.1f * GameConstants.ViewportHeight));

            // set up position for scores
            scorePositions.Clear();
            for (int i = 0; i < MAX_SCORES_TO_DISPLAY && i < scores.Count; i++)
            {
                textSize = highScoreFont.MeasureString("#" + (i + 1).ToString() + ": " + scores[i].ToString().PadLeft(4));
                scorePositions.Add(new Vector2(
                    (int)(GameConstants.ViewportWidth / 2) - textSize.X / 2,
                    (int)((0.3 + 0.1 * i) * GameConstants.ViewportHeight)));
            }

            // set up position for instruction
            textSize = mediumFont.MeasureString(strInstructions);
            instructionsPos = new Vector2(
                (int)(GameConstants.ViewportWidth / 2) - textSize.X / 2,
                (int)(0.85f * GameConstants.ViewportHeight));
        }

        /// <summary>
        /// Read all of the scores that player has achieved from the scores file,
        /// and sort the scores descending.
        /// </summary>
        private void ReadScoreInfo()
        {
            StreamReader sr;
            float t;

            if (File.Exists("Scores/scores.txt"))
            {
                // file exists, open and read a line at a time from it
                sr = new StreamReader("Scores/scores.txt");

                while (!sr.EndOfStream)
                {
                    t = (float)Convert.ToDouble(sr.ReadLine());
                    scores.Add((int)t);
                }

                sr.Close();
            }
            else
            {
                // could not open file, add default highscore to the display
                scores.Clear();
                scores.Add(HighScore.DefaultHighScore);
            }

            // sort the scores in descending order based on the lambda expression
            scores.Sort((int m, int n) => {
                if (m > n)
                    return -1;
                else if (m == n)
                    return 0;
                else
                    return 1;
                });
        }

        /// <summary>
        /// Update the HighScores screen.
        /// </summary>
        /// <remarks>
        /// Any new scores are read in from a file and all of the positions 
        /// are reset.
        /// </remarks>
        public void Update()
        {
            ReadScoreInfo();
            SetPosition();
        }

        /// <summary>
        /// Draw the HighScores screen to the page.
        /// </summary>
        public void Draw()
        {
            spriteBatch.Begin();

            spriteBatch.Draw(highScoreBackground, viewportRect, Color.White); // draw background
            spriteBatch.DrawString(largeFont, strTitle, titlePos, Color.Orange); // draw HighScores title

            // Draw the scores
            for (int i = 0; i < MAX_SCORES_TO_DISPLAY && i < scores.Count; i++)
            {
                spriteBatch.DrawString(highScoreFont, "#" + (i + 1).ToString() + ": " + scores[i].ToString().PadLeft(4),
                    scorePositions[i], Color.Orange);
            }

            // draw the instructions
            spriteBatch.DrawString(mediumFont, strInstructions, instructionsPos, Color.White);

            spriteBatch.End();
        }
    }
}
