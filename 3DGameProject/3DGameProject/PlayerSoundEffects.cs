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
    /// Class which contains the sound effects for the player, namely the sounds
    /// the player's car makes.
    /// </summary>
    /// <remarks>This is a singleton in the game</remarks>
    class PlayerSoundEffects
    {
        private SoundEffect carCrashMinorEffect;
        private SoundEffect carCrashTotalEffect;
        private SoundEffect brakeMajorEffect;
        private SoundEffect brakeMinorEffect;
        private SoundEffect engineEffect;

        // Use SoundEffectInstance for greater control on playback
        private SoundEffectInstance carCrashMinor;
        private SoundEffectInstance carCrashTotal;
        private SoundEffectInstance brakeMajor;
        private SoundEffectInstance brakeMinor;
        private SoundEffectInstance engine;

        /// <summary>
        /// Load the sound effects for the player's car
        /// </summary>
        /// <param name="content">Content pipeline (for SoundEffects)</param>
        public void LoadContent(ContentManager content)
        {
            // Load effects
            carCrashMinorEffect = content.Load<SoundEffect>("Audio/CarCrashMinor");
            carCrashTotalEffect = content.Load<SoundEffect>("Audio/CarCrashTotal");
            brakeMajorEffect = content.Load<SoundEffect>("Audio/BrakeMajor");
            brakeMinorEffect = content.Load<SoundEffect>("Audio/BrakeMinor");
            engineEffect = content.Load<SoundEffect>("Audio/Engine");

            // CreateSoundEffectInstance
            carCrashMinor = carCrashMinorEffect.CreateInstance();
            carCrashTotal = carCrashTotalEffect.CreateInstance();
            brakeMajor = brakeMajorEffect.CreateInstance();
            brakeMinor = brakeMinorEffect.CreateInstance();
            engine = engineEffect.CreateInstance();
        }

        /// <summary>
        /// Play the sound of the player crashing
        /// </summary>
        /// <param name="velocity">Player's velocity</param>
        /// <remarks>
        /// A major or minor crash sound will be heard depending on the player's velocity
        /// </remarks>
        public void PlayCrash(float velocity)
        {
            float speed = Math.Abs(velocity);

            if (speed > (Player.MaxVelocity / 2) && !(carCrashTotal.State == SoundState.Playing))
            {
                stopAllNonEngineSounds();
                carCrashTotal.Play();
            }
            else if (speed > (Player.MaxVelocity / 10) && !(carCrashMinor.State == SoundState.Playing))
            {
                stopAllNonEngineSounds();
                carCrashMinor.Play();
            }
        }

        /// <summary>
        /// Play the sound of the player braking
        /// </summary>
        /// <param name="velocity">Player's velocity</param>
        /// <remarks>
        /// A major or minor braking sound will be heard depending on the player's velocity
        /// </remarks>
        public void PlayBrake(float velocity)
        {
            float speed = Math.Abs(velocity);

            if (speed > (3 * Player.MaxVelocity / 4) && (brakeMajor.State != SoundState.Playing))
            {
                stopAllNonEngineSounds();
                brakeMajor.Play();
            }
            else if (speed > (Player.MaxVelocity / 5) &&
                (brakeMinor.State != SoundState.Playing) &&
                (brakeMajor.State != SoundState.Playing))
            {
                stopAllNonEngineSounds();
                brakeMinor.Play();
            }
        }

        /// <summary>
        /// Play the sound of the player's engine
        /// </summary>
        /// <param name="velocity">Player's velocity</param>
        /// <remarks>
        /// The volume of the engine will be scaled based on the player's velocity
        /// </remarks>
        public void PlayEngine(float velocity)
        {
            float speed = Math.Abs(velocity);

            if (engine.State != SoundState.Playing)
            {
                engine.Play();
            }

            // Scale sound volume based on the player's velocity
            engine.Volume = 0.33f + (0.67f) * Math.Abs(velocity / Player.MaxVelocity);

            ////if (!(brakeMajor.State == SoundState.Playing) && !(brakeMinor.State == SoundState.Playing)
            ////    && !(carCrashMinor.State == SoundState.Playing) && !(carCrashTotal.State == SoundState.Playing))
            ////{
            //    if (speed > (Player.MaxVelocity / 2)
            //        && gear1.State == SoundState.Playing
            //        && gear1ToGear2.State != SoundState.Playing)
            //    {
            //        stopAllEngineSounds();
            //        gear1ToGear2.Play();
            //    }
            //    else if (speed <= Player.MaxVelocity / 2
            //        && gear2.State == SoundState.Playing
            //        && gear1ToGear2.State != SoundState.Playing)
            //    {
            //        stopAllEngineSounds();
            //        gear1ToGear2.Play();
            //    }
            //    else if (speed <= Player.MaxVelocity / 2
            //        && gear1ToGear2.State != SoundState.Playing
            //        && gear1.State != SoundState.Playing)
            //    {
            //        stopAllEngineSounds();
            //        gear1.Play();
            //    }
            //    else if (speed >= Player.MaxVelocity / 2
            //        && gear1ToGear2.State != SoundState.Playing
            //        && gear2.State != SoundState.Playing)
            //    {
            //        stopAllEngineSounds();
            //        gear2.Play();
            //    }
            ////}
        }

        /// <summary>
        /// Stops all player sounds not related to the engine.
        /// </summary>
        private void stopAllNonEngineSounds()
        {
            brakeMajor.Stop();
            brakeMinor.Stop();
            carCrashTotal.Stop();
            carCrashMinor.Stop();
        }
    }
}
