/*
 * 3D Game Programming Project
 * Dr. Liu
 * Zach Bates, Lauren Buss, Corey Darr, Jason Ruchti, Jared Tittle
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace _3DGameProject
{
    /// <summary>
    /// Class which contains songs to be played when the game is in a certain state
    /// </summary>
    /// <remarks>This is a singleton in the game</remarks>
    public class GameSongs
    {
        private Song backgroundSong;
        private Song gameOverSong;

        /// <summary>
        /// Allows the client to load the game songs
        /// </summary>
        /// <param name="content">Content Pipeline (for SoundEffects)</param>
        public void LoadContent(ContentManager content)
        {
            MediaPlayer.Volume = 0.75f;
            
            backgroundSong = content.Load<Song>("Songs/BackgroundSong");
            gameOverSong = content.Load<Song>("Songs/GameOverSong");
        }

        /// <summary>
        /// Allows the client to play the appropriate background depending on the state of the game
        /// </summary>
        /// <param name="gameState">State of the game</param>
        public void PlayBackground(GameConstants.GameState gameState)
        {
            if (gameState == GameConstants.GameState.End)
            {
                // Play the ending song
                if (MediaPlayer.State == MediaState.Playing && MediaPlayer.Queue.ActiveSong == backgroundSong)
                {
                    // Currently the main song is playing, stop it and play the game over song
                    MediaPlayer.Stop();
                    MediaPlayer.Volume = 1.0f;
                    MediaPlayer.Play(gameOverSong);
                }
            }
            else
            {
                // Play the background song

                if (MediaPlayer.State == MediaState.Playing && MediaPlayer.Queue.ActiveSong == gameOverSong)
                {
                    // currently the game over song is playing, stop it and play the background song
                    MediaPlayer.Stop();
                }

                if (MediaPlayer.State != MediaState.Playing)
                {
                    // Play the background song if it is not playing
                    MediaPlayer.Volume = 0.75f;
                    MediaPlayer.Play(backgroundSong);
                }
            }
        }

        ///// <summary>
        ///// Allows the client to play the theme song
        ///// </summary>
        ///// <remarks>Wraps the titleSong variable to manage access</remarks>
        //private void PlayThemeSong()
        //{
        //    if (!(titleSong.State == SoundState.Playing))
        //        titleSong.Play();
        //}

        ///// <summary>
        ///// Allows the client to play the song when the player is captured
        ///// </summary>
        ///// <remarks>Wraps the gameOverSong variable to manage access</remarks>
        //private void PlayGameOverSong()
        //{
        //    if (!(gameOverSong.State == SoundState.Playing))
        //        gameOverSong.Play();
        //}

        ///// <summary>
        ///// Allows the client to stop the title song
        ///// </summary>
        ///// <remarks>Wraps the titleSong variable to manage access</remarks>
        //public void StopTitleSong()
        //{
        //    titleSong.Stop();
        //}

        ///// <summary>
        ///// Allows the client to stop the game over song
        ///// </summary>
        ///// <remarks>Wraps the gameOverSong variable to manage access</remarks>
        //public void StopGameOverSong()
        //{
        //    gameOverSong.Stop();
        //}
    }
}
