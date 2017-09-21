using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OSSC
{
    /// <summary>
    /// Manages all SoundCues
    /// </summary>
    public class CueManager
    {
        #region Private fields
        /// <summary>
        /// List of all SoundCues.
        /// </summary>
        private List<SoundCue> _soundCues;
        #endregion

        #region Public Methods and Properties
        /// <summary>
        /// Default Constructor
        /// </summary>
        public CueManager()
        {
            _soundCues = new List<SoundCue>();
        }

        /// <summary>
        /// Costruct CueManager with some initial SoundCues created.
        /// </summary>
        /// <param name="initialSize">Size of the SoundCue pool.</param>
        public CueManager(int initialSize)
        {
            _soundCues = new List<SoundCue>(initialSize);
        }
        
        /// <summary>
        /// Get a free SoundCue.
        /// </summary>
        /// <returns>Returns a SoundCue instance.</returns>
        public SoundCue GetSoundCue()
        {
            SoundCue cue = FindFreeCue();
            cue.OnPlayKilled += OnPlayKilled_handler;
            return cue;
        }

        /// <summary>
        /// Stops all SoundCues from playing.
        /// </summary>
        /// <param name="shouldCallOnEndCallback">Check whether to call OnEnd events or not.</param>
        public void StopAllCues(bool shouldCallOnEndCallback = true)
        {
            for (int i = 0; i < _soundCues.Count; i++)
            {
                if (_soundCues[i].IsPlaying)
                    _soundCues[i].Stop(shouldCallOnEndCallback);
            }
        }
        #endregion

        #region Private methods
        private void OnPlayKilled_handler(SoundCue cue, SoundCueProxy proxy)
        {
            //NOTE: Clear up any references to events.
            cue.OnPlayKilled = null;
            cue.OnPlayCueEnded = null;
            cue.OnPlayEnded = null;
            proxy.SoundCue = null;
        }

        /// <summary>
        /// Finds the first non-playing SoundCue.
        /// </summary>
        /// <returns>cached SoundCue item.</returns>
        private SoundCue FindFreeCue()
        {
            SoundCue cue = null;
            for (int i = 0; i < _soundCues.Count; i++)
            {
                if (_soundCues[i].IsPlaying == false)
                {
                    cue = _soundCues[i];
                    break;
                }
            }

            if (cue == null)
            {
                cue = new SoundCue(_soundCues.Count);
                _soundCues.Add(cue);
            }

            return cue;
        }
        #endregion
    }
}