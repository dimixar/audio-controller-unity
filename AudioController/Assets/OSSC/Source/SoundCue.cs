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
    public class SoundCue : ISoundCue
    {
        /// <summary>
        /// Called on every sound item that has ended playing;
        /// /// </summary>
        public Action<string> OnPlayEnded { get; set; }

        /// <summary>
        /// Called when the whole cue finished playing;
        /// </summary>
        public Action<SoundCue> OnPlayCueEnded { get; set; }

        /// <summary>
        /// Called whenever the sound cue has finished playing or was stopped
        /// </summary>
        public Action<SoundCue, SoundCueProxy> OnPlayKilled { get; set; }

        public SoundObject AudioObject { get; set; }

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
        private SoundCueData _data;
        private SoundCueProxy _currentProxy;

        /// <summary>
        /// Will start playing the cue.
        /// NOTE: It is called automatically if gotten from AudioController.
        /// </summary>
        public void Play(SoundCueData data)
        {
            _data = data;
            AudioObject.OnFinishedPlaying = OnFinishedPlaying_handler;
            // audioObject.isDespawnOnFinishedPlaying = false;
            if (TryPlayNext() == false)
            {
                return;
            }
            IsPlaying = true;
        }

        public void Play(SoundCueData data, SoundCueProxy proxy)
        {
            Play(data);
            _currentProxy = proxy;
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
                OnPlayKilled(this, _currentProxy);
                _currentProxy = null;
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
                if (TryPlayNext() == false)
                {
                    Stop(true);
                }
            }
            else
            {
                if (_data.isLooped)
                {
                    _currentItem = 0;
                    if (TryPlayNext() == false)
                    {
                        Stop(true);
                    }
                }
                else
                {
                    Stop(true);
                }
            }
        }

        private bool TryPlayNext()
        {
            bool isPlaying = false;
            if (_data.categoriesForSounds[_currentItem].isMute == false)
            {
                PlayCurrentItem();
                _currentItem += 1;
                isPlaying = true;
            }
            else
            {
                for (int i = _currentItem; i < _data.sounds.Length; i++)
                {
                    if (_data.categoriesForSounds[i].isMute == false)
                    {
                        _currentItem = i;
                        PlayCurrentItem();
                        _currentItem += 1;
                        isPlaying = true;
                        break;
                    }
                }
            }
            return isPlaying;
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
                AudioObject.Setup(_data.sounds[_currentItem].name, GetRandomClip( _data.sounds[_currentItem].clips ), realVolume, mixer: _data.sounds[_currentItem].mixer);
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
        public CategoryItem[] categoriesForSounds;
        public float[] categoryVolumes;
        public GameObject audioPrefab;
        public float fadeInTime;
        public float fadeOutTime;
        public bool isFadeIn;
        public bool isFadeOut;

        public bool isLooped;
    }
}