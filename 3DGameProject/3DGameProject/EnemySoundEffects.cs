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
    /// <remarks>
    /// This is a singleton in the game to manage sound resources across multiple enemies.
    /// </remarks>
    public class EnemySoundEffects
    {
        private SoundEffect lockingBeepEffect;
        private SoundEffect lockedOnBeepEffect;

        private SoundEffectInstance lockingBeep;
        private SoundEffectInstance lockedOnBeep;

        /// <summary>
        /// Load the sound effects for the enemy
        /// </summary>
        /// <param name="content">Content pipeline (for SoundEffects)</param>
        public void LoadContent(ContentManager content)
        {
            lockingBeepEffect = content.Load<SoundEffect>("Audio/LockingBeep");
            lockedOnBeepEffect = content.Load<SoundEffect>("Audio/LockOnBeep");

            lockingBeep = lockingBeepEffect.CreateInstance();
            lockedOnBeep = lockedOnBeepEffect.CreateInstance();

            lockedOnBeep.Volume = 0.20f; // to avoid it being obnoxious
        }

        /// <summary>
        /// Play the sound of the enemy locking onto the player
        /// </summary>
        /// <remarks>
        /// If the locked on beep is already playing, the locking beep is not played
        /// since the locked on beep has higher priority.
        /// </remarks>
        public void PlayLockingBeep()
        {
            if (lockingBeep.State != SoundState.Playing && lockedOnBeep.State != SoundState.Playing)
            {
                StopAllSounds();
                lockingBeep.Play();
            }
        }

        /// <summary>
        /// Play the sound of the enemy locked onto the player
        /// </summary>
        public void PlayLockedOnBeep()
        {
            if (lockedOnBeep.State != SoundState.Playing)
            {
                StopAllSounds();
                lockedOnBeep.Play();
            }
        }

        /// <summary>
        /// Stops all Enemy sound effects
        /// </summary>
        public void StopAllSounds()
        {
            lockingBeep.Stop();
            lockedOnBeep.Stop();
        }
    }
}
