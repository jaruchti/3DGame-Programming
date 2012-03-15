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
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;

namespace _3DGameProject
{
    /// <summary>
    /// Class which contains the sound effects for the enemy, namely the sounds
    /// for the lock on.
    /// </summary>
    class EnemySoundEffects
    {
        private SoundEffect lockingBeepEffect;
        private SoundEffectInstance lockingBeep;

        /// <summary>
        /// Load the sound effects for the enemy
        /// </summary>
        /// <param name="content">Content pipeline (for SoundEffects)</param>
        public void LoadContent(ContentManager content)
        {
            lockingBeepEffect = content.Load<SoundEffect>("Audio/LockingBeep");
            lockingBeep = lockingBeepEffect.CreateInstance();
        }

        /// <summary>
        /// Play the sound of the enemy locking onto the player
        /// </summary>
        public void PlayLockingBeep()
        {
            if (lockingBeep.State != SoundState.Playing)
                lockingBeep.Play();
        }

        /// <summary>
        /// Stops all Enemy sound effects
        /// </summary>
        public void StopAllSounds()
        {
            lockingBeep.Stop();
        }
    }
}
