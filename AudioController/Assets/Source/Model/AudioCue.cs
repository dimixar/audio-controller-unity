using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using OSAC.Model;

namespace OSAC
{
    /// <summary>
    /// Plays a whole cue of soundItems
    /// </summary>
    public class AudioCue
    {
        /// <summary>
        /// Called on every sound item that has ended playing;
        /// /// </summary>
        public Action<string> OnPlayEnded;

        /// <summary>
        /// Called when the whole cue finished playing;
        /// </summary>
        public Action OnPlayCueEnded;
        public AudioObject audioObject;

        public SoundItem[] Items
        {
            set { _items = value; }
            get { return _items; }
        }

        public GameObject Prefab { get; set; }

        private SoundItem[] _items;
        private int _currentItem = 0;
        private bool _isPaused = false;
        private bool _isUsable = true;

        /// <summary>
        /// Will start playing the cue.
        /// NOTE: It is called automatically if gotten from AudioController.
        /// </summary>
        public void Play()
        {
            UnityEngine.Assertions.Assert.IsTrue(_isUsable, "[AudioCue] AudioCue cannot be reused!!!");
            audioObject.OnFinishedPlaying = OnFinishedPlaying_handler;
            audioObject.isDespawnOnFinishedPlaying = false;
            audioObject.Setup(_items[_currentItem].name, _items[_currentItem].clip);
            _currentItem += 1;
            audioObject.Play();
            _isPaused = false;
        }

        /// <summary>
        /// Will pause the cue;
        /// </summary>
        public void Pause()
        {
            UnityEngine.Assertions.Assert.IsTrue(_currentItem > 0, "[AudioCue] Cannot pause when not even started.");
            audioObject.Pause();
        }

        /// <summary>
        /// Resume the cue from where it was paused.
        /// </summary>
        public void Resume()
        {
            UnityEngine.Assertions.Assert.IsTrue(_currentItem > 0, "[AudioCue] Cannot resume when not even started.");
            audioObject.Resume();
        }

        /// <summary>
        /// Will replay the cue.
        /// NOTE: Do not call it directly!!! Send it to audioController instead.
        /// </summary>
        public void Replay()
        {
            _isUsable = audioObject != null;
            Play();
        }

        public void Stop(bool shouldCallOnFinishedCue = false)
        {
            audioObject.OnFinishedPlaying = null;
            ((IPoolable)audioObject).pool.Despawn(audioObject.gameObject);
            audioObject.isDespawnOnFinishedPlaying = true;
            audioObject = null;
            _currentItem = 0;
            _isUsable = false;

            if (shouldCallOnFinishedCue == false)
                return;

            if (OnPlayCueEnded != null) {
                OnPlayCueEnded();
            }
        }

        private void OnFinishedPlaying_handler(AudioObject obj)
        {
            string itemName = _items[_currentItem - 1].name;
            if (OnPlayEnded != null) {
                OnPlayEnded(itemName);
            }

            if (_currentItem < _items.Length)
            {
                audioObject.Setup(_items[_currentItem].name, _items[_currentItem].clip);
                _currentItem += 1;
                audioObject.Play();
            }
            else
            {
                Stop(true);
            }
        }
    }
}