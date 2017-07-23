using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OSSC
{
    public class CueManager
    {
        #region Private fields
        private List<SoundCue> _soundCues;
        #endregion

        #region Public Methods and Properties
        public CueManager()
        {
            _soundCues = new List<SoundCue>();
        }

        public CueManager(int initialSize)
        {
            _soundCues = new List<SoundCue>(initialSize);
        }
        public SoundCue GetSoundCue()
        {
            SoundCue cue = FindFreeCue();
            cue.OnPlayKilled += OnPlayKilled_handler;
            return FindFreeCue();
        }

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