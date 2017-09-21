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
        /// Check ISoundCue
        /// </summary>
        public Action<string> OnPlayEnded { get; set; }

        /// <summary>
        /// Check ISoundCue
        /// </summary>
        public Action<SoundCue> OnPlayCueEnded { get; set; }

        /// <summary>
        /// Called whenever the sound cue has finished playing or was stopped
        /// </summary>
        public Action<SoundCue, SoundCueProxy> OnPlayKilled { get; set; }

        /// <summary>
        /// Check ISoundCue
        /// </summary>
        public SoundObject AudioObject { get; set; }

        /// <summary>
        /// Check ISoundCue
        /// </summary>
        public SoundCueData Data { get { return _data; } }

        /// <summary>
        /// Check ISoundCue
        /// </summary>
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

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SoundCue()
        {
        }

        /// <summary>
        /// Custom Constructor
        /// </summary>
        /// <param name="id">Sets the ID of the SoundCue.</param>
        public SoundCue(int id)
        {
            ID = id;
        }

        /// <summary>
        /// Current index of the SoundItem playing.
        /// </summary>
        private int _currentItem = 0;
        /// <summary>
        /// SoundCue data
        /// </summary>
        private SoundCueData _data;
        /// <summary>
        /// The proxy that the user uses to control the SoundCue.
        /// </summary>
        private SoundCueProxy _currentProxy;

        /// <summary>
        /// Will start playing the cue.
        /// NOTE: It is called from SoundCueProxy that is created by the SoundController.
        /// </summary>
        public void Play(SoundCueData data)
        {
            _data = data;
            AudioObject.isDespawnOnFinishedPlaying = !data.isLooped;
            AudioObject.OnFinishedPlaying = OnFinishedPlaying_handler;
            // audioObject.isDespawnOnFinishedPlaying = false;
            if (TryPlayNext() == false)
            {
                return;
            }
            IsPlaying = true;
        }

        /// <summary>
        /// Plays the SoundCue.
        /// </summary>
        /// <param name="data">SoundCue's data</param>
        /// <param name="proxy">Proxy created by SoundController that called this method.</param>
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

        /// <summary>
        /// Stops the SoundCue.
        /// </summary>
        /// <param name="shouldCallOnFinishedCue">Checks whether to call OnEnd events, or not.</param>
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

        /// <summary>
        /// Internal event handler.
        /// </summary>
        /// <param name="obj"></param>
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

        /// <summary>
        /// Tries to play the next SoundItem in SoundCue.
        /// </summary>
        /// <returns>True - can play, False - Cannot</returns>
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

        /// <summary>
        /// Plays the Current SoundItem.
        /// </summary>
        private void PlayCurrentItem()
        {
            SoundItem item = _data.sounds[_currentItem];
            
            float itemVolume = item.isRandomVolume
                ? item.volumeRange.GetRandomRange()
                : item.volume;
            float realVolume = itemVolume * _data.categoryVolumes[_currentItem];

            float realPitch = item.isRandomPitch
                ? item.pitchRange.GetRandomRange()
                : 1f;
            
            if (_currentItem == _data.sounds.Length - 1)
            {
                AudioObject.Setup(
                    item.name,
                    GetRandomClip( item.clips ),
                    realVolume,
                    _data.fadeInTime,
                    _data.fadeOutTime,
                    item.mixer,
                    realPitch);
            }
            else
            {
                AudioObject.Setup(
                    item.name,
                    GetRandomClip( item.clips ),
                    realVolume,
                    mixer: item.mixer,
                    pitch: realPitch);
            }
            AudioObject.Play();
        }

        /// <summary>
        /// Gets a random AudioClip from and array of AudioClips.
        /// </summary>
        /// <param name="clips">Array of SoundClips</param>
        /// <returns>An AudioClip</returns>
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
        /// <summary>
        /// sound items that played by the SoundCue.
        /// </summary>
        public SoundItem[] sounds;
        /// <summary>
        /// category items that correspond with each of SoundItem in sounds.
        /// </summary>
        public CategoryItem[] categoriesForSounds;
        /// <summary>
        /// Category sound volumes that correspond with Sound items.
        /// </summary>
        public float[] categoryVolumes;
        /// <summary>
        /// Prefab with SoundObject to play Sound items.
        /// </summary>
        public GameObject audioPrefab;
        /// <summary>
        /// Fade In time.
        /// </summary>
        public float fadeInTime;
        /// <summary>
        /// Fade Out time.
        /// </summary>
        public float fadeOutTime;
        /// <summary>
        /// Should SoundCue Fade In?
        /// </summary>
        public bool isFadeIn;
        /// <summary>
        /// Should SoundCue Fade Out?
        /// </summary>
        public bool isFadeOut;

        /// <summary>
        /// Should SoundCue be looped?
        /// </summary>
        public bool isLooped;
    }
}