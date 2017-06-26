using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using OSSC.Model;

namespace OSSC
{
    /// <summary>
    /// Plays a whole cue of soundItems
    /// </summary>
    public class SoundCue
    {
        /// <summary>
        /// Called on every sound item that has ended playing;
        /// /// </summary>
        public Action<string> OnPlayEnded;

        /// <summary>
        /// Called when the whole cue finished playing;
        /// </summary>
        public Action<SoundCue> OnPlayCueEnded;
        public SoundObject audioObject;

        public SoundCueData data { get { return _data; } }

        public bool IsPlaying
        {
            get;
            private set;
        }

        private int _currentItem = 0;
        private bool _isUsable = true;
        private SoundCueData _data;

        /// <summary>
        /// Will start playing the cue.
        /// NOTE: It is called automatically if gotten from AudioController.
        /// </summary>
        public void Play(SoundCueData data)
        {
            _data = data;
            UnityEngine.Assertions.Assert.IsTrue(_isUsable, "[AudioCue] AudioCue cannot be reused!!!");
            audioObject.OnFinishedPlaying = OnFinishedPlaying_handler;
            // audioObject.isDespawnOnFinishedPlaying = false;
            PlayCurrentItem();
            _currentItem += 1;
            IsPlaying = true;
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

        public void Stop(bool shouldCallOnFinishedCue = true)
        {
            if (IsPlaying == false)
                return;
            audioObject.OnFinishedPlaying = null;
            // ((IPoolable)audioObject).pool.Despawn(audioObject.gameObject);
            audioObject.Stop();
            audioObject = null;
            _currentItem = 0;
            _isUsable = false;
            IsPlaying = false;

            if (shouldCallOnFinishedCue == false)
                return;

            if (OnPlayCueEnded != null) {
                OnPlayCueEnded(this);
            }
        }

        private void OnFinishedPlaying_handler(SoundObject obj)
        {
            string itemName = _data.sounds[_currentItem - 1].name;
            if (OnPlayEnded != null) {
                OnPlayEnded(itemName);
            }

            if (_currentItem < _data.sounds.Length)
            {
                PlayCurrentItem();
                _currentItem += 1;

            }
            else
            {
                if (_data.isLooped)
                {
                    _currentItem = 0;
                    PlayCurrentItem();
                    _currentItem += 1;
                }
                else
                {
                    Stop(true);
                }
            }
        }

        private void PlayCurrentItem()
        {
            float realVolume = _data.sounds[_currentItem].volume * _data.categoryVolumes[_currentItem];
            if (_currentItem == _data.sounds.Length - 1)
            {
                audioObject.Setup(_data.sounds[_currentItem].name, GetRandomClip( _data.sounds[_currentItem].clips ), realVolume, _data.fadeInTime, _data.fadeOutTime, _data.sounds[_currentItem].mixer);
            }
            else
            {
                audioObject.Setup(_data.sounds[_currentItem].name, GetRandomClip( _data.sounds[_currentItem].clips ), realVolume, mixer:_data.sounds[_currentItem].mixer);
            }
            audioObject.Play();
        }

        private AudioClip GetRandomClip(AudioClip[] clips)
        {
            int index = UnityEngine.Random.Range(0, clips.Length);
            return clips[index];
        }
    }

    /// <summary>
    /// Used for sending data to play to AudioCue
    /// </summary>
    public struct SoundCueData
    {
        public SoundItem[] sounds;
        public float[] categoryVolumes;
        public GameObject audioPrefab;
        public float fadeInTime;
        public float fadeOutTime;
        public bool isFadeIn;
        public bool isFadeOut;

        public bool isLooped;
    }
}