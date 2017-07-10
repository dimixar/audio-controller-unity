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

        /// <summary>
        /// Called whenever the sound cue has finished playing or was stopped
        /// </summary>
        public Action<SoundCue> OnPlayKilled;

        public SoundObject AudioObject;

        public SoundCueData Data { get { return _data; } }

        public bool IsPlaying
        {
            get;
            private set;
        }

        /// <summary>
        /// SoundCue's unique ID given by the manager
        /// </summary>
        /// <returns></returns>
        public int ID
        {
            get;
            private set;
        }

        public SoundCue()
        {
        }

        public SoundCue(int id)
        {
            ID = id;
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
            AudioObject.OnFinishedPlaying = OnFinishedPlaying_handler;
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
            AudioObject.Pause();
        }

        /// <summary>
        /// Resume the cue from where it was paused.
        /// </summary>
        public void Resume()
        {
            UnityEngine.Assertions.Assert.IsTrue(_currentItem > 0, "[AudioCue] Cannot resume when not even started.");
            AudioObject.Resume();
        }

        public void Stop(bool shouldCallOnFinishedCue = true)
        {
            if (IsPlaying == false)
                return;
            AudioObject.OnFinishedPlaying = null;
            // ((IPoolable)audioObject).pool.Despawn(audioObject.gameObject);
            AudioObject.Stop();
            AudioObject = null;
            _currentItem = 0;
            _isUsable = false;
            IsPlaying = false;

            if (shouldCallOnFinishedCue)
            {
                if (OnPlayCueEnded != null)
                {
                    OnPlayCueEnded(this);
                }
            }

            if (OnPlayKilled != null)
            {
                OnPlayKilled(this);
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
                AudioObject.Setup(_data.sounds[_currentItem].name, GetRandomClip( _data.sounds[_currentItem].clips ), realVolume, _data.fadeInTime, _data.fadeOutTime, _data.sounds[_currentItem].mixer);
            }
            else
            {
                AudioObject.Setup(_data.sounds[_currentItem].name, GetRandomClip( _data.sounds[_currentItem].clips ), realVolume, mixer:_data.sounds[_currentItem].mixer);
            }
            AudioObject.Play();
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