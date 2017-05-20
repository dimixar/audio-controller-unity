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

        public AudioCueData data { get { return _data; } }

        private int _currentItem = 0;
        private bool _isUsable = true;
        private AudioCueData _data;

        /// <summary>
        /// Will start playing the cue.
        /// NOTE: It is called automatically if gotten from AudioController.
        /// </summary>
        public void Play(AudioCueData data)
        {
            _data = data;
            UnityEngine.Assertions.Assert.IsTrue(_isUsable, "[AudioCue] AudioCue cannot be reused!!!");
            audioObject.OnFinishedPlaying = OnFinishedPlaying_handler;
            audioObject.isDespawnOnFinishedPlaying = false;
            float realVolume = _data.sounds[_currentItem].volume * _data.categoryVolumes[_currentItem];
            audioObject.Setup(_data.sounds[_currentItem].name, _data.sounds[_currentItem].clip, realVolume, _data.fadeInTime, _data.fadeOutTime);
            _currentItem += 1;
            audioObject.Play();
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
            string itemName = _data.sounds[_currentItem - 1].name;
            if (OnPlayEnded != null) {
                OnPlayEnded(itemName);
            }

            if (_currentItem < _data.sounds.Length)
            {
                float realVolume = _data.sounds[_currentItem].volume * _data.categoryVolumes[_currentItem];
                if (_currentItem == _data.sounds.Length - 1)
                {
                    audioObject.Setup(_data.sounds[_currentItem].name, _data.sounds[_currentItem].clip, realVolume, _data.fadeInTime, _data.fadeOutTime);
                }
                else
                {
                    audioObject.Setup(_data.sounds[_currentItem].name, _data.sounds[_currentItem].clip, realVolume);
                }
                _currentItem += 1;
                audioObject.Play();
            }
            else
            {
                Stop(true);
            }
        }
    }

    /// <summary>
    /// Used for sending data to play to AudioCue
    /// </summary>
    public struct AudioCueData
    {
        public SoundItem[] sounds;
        public float[] categoryVolumes;
        public GameObject audioPrefab;
        public float fadeInTime;
        public float fadeOutTime;
        public bool isFadeIn;
        public bool isFadeOut;
    }
}