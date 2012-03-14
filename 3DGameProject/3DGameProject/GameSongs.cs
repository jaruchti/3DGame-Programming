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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _3DGameProject
{
    /// <summary>
    /// Class which contains songs to be played when the game is in a certain state
    /// </summary>
    /// <remarks>This is a singleton in the game</remarks>
    public class GameSongs
    {
        private SoundEffect titleSongEffect;
        private SoundEffect gameOverSongEffect;

        private SoundEffectInstance titleSong;
        private SoundEffectInstance gameOverSong;

        /// <summary>
        /// Allows the client to load the game songs
        /// </summary>
        /// <param name="content">Content Pipeline (for SoundEffects)</param>
        public void LoadContent(ContentManager content)
        {
            titleSongEffect = content.Load<SoundEffect>("Songs/TitleSong");
            gameOverSongEffect = content.Load<SoundEffect>("Songs/GameOverSong");

            titleSong = titleSongEffect.CreateInstance();
            gameOverSong = gameOverSongEffect.CreateInstance();
        }

        /// <summary>
        /// Allows the client to play the title song
        /// </summary>
        /// <remarks>Wraps the titleSong variable to manage access</remarks>
        public void PlayTitleSong()
        {
            if (!(titleSong.State == SoundState.Playing))
                titleSong.Play();
        }

        /// <summary>
        /// Allows the client to play the song when the player is captured
        /// </summary>
        /// <remarks>Wraps the gameOverSong variable to manage access</remarks>
        public void PlayGameOverSong()
        {
            if (!(gameOverSong.State == SoundState.Playing))
                gameOverSong.Play();
        }

        /// <summary>
        /// Allows the client to stop the title song
        /// </summary>
        /// <remarks>Wraps the titleSong variable to manage access</remarks>
        public void StopTitleSong()
        {
            titleSong.Stop();
        }

        /// <summary>
        /// Allows the client to stop the game over song
        /// </summary>
        /// <remarks>Wraps the gameOverSong variable to manage access</remarks>
        public void StopGameOverSong()
        {
            gameOverSong.Stop();
        }
    }
}
