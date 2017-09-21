using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OSSC
{
    /// <summary>
    /// SoundCue Inteface.
    /// SoundController returns a SoundCue Interface to further control the playing SouncCue
    /// </summary>
    public interface ISoundCue
    {
        /// <summary>
        /// Called everytime a SoundItem finished playing in SoundCue.
        /// </summary>
        Action<string> OnPlayEnded { get; set; }
        /// <summary>
        /// Called everytime a SoundCue finished playing.
        /// </summary>
        Action<SoundCue> OnPlayCueEnded { get; set; }
        /// <summary>
        /// Used by the SoundCue to play all SoundItems.
        /// </summary>
        SoundObject AudioObject { get; set; }
        /// <summary>
        /// Data collected by the Soundcontroller. Has all SoundItems that needs to be played.
        /// </summary>
        SoundCueData Data { get; }
        /// <summary>
        /// Check if SoundCue is still playing.
        /// </summary>
        bool IsPlaying { get; }
        /// <summary>
        /// SoundCue Identifier
        /// </summary>
        int ID { get; }

        /// <summary>
        /// Plays the SoundCue.
        /// This method is called by the SoundController.
        /// </summary>
        /// <param name="data">The Data needed for the SoundCue to play.</param>
        void Play(SoundCueData data);
        /// <summary>
        /// Pause the SoundCue.
        /// </summary>
        void Pause();
        /// <summary>
        /// Resume the paused SoundCue.
        /// </summary>
        void Resume();
        /// <summary>
        /// Stop the SoundCue from playing.
        /// </summary>
        /// <param name="shouldCallOnFinishedCue">Select whether to Call OnEnd events or not.</param>
        void Stop(bool shouldCallOnFinishedCue = true);
    }
}
