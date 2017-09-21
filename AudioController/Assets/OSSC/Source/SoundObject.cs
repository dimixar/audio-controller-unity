using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace OSSC
{
    /// <summary>
    /// Used by the SoundCue.
    /// Controls the AudioSource.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SoundObject : MonoBehaviour, IPoolable
    {

        /// <summary>
        /// Called when SoundObject finishes playing.
        /// </summary>
        public System.Action<SoundObject> OnFinishedPlaying;

        #region private fields
        /// <summary>
        /// SoundObject's ID.
        /// </summary>
        private string _id;
        /// <summary>
        /// Played AudioClip.
        /// </summary>
        private AudioClip _clip;
        /// <summary>
        /// The thing that playes the sound.
        /// </summary>
        private AudioSource _source;

        /// <summary>
        /// Curoutine used for playing the sound.
        /// </summary>
        private Coroutine _playingRoutine;
        /// <summary>
        /// Flag to check if SoundObject is paused.
        /// </summary>
        private bool _isPaused;

        /// <summary>
        /// Checks whether ObjectPool can use this SoundObject.
        /// </summary>
        private bool _isFree = true;
        /// <summary>
        /// Reference to the pool that this SoundObject belongs to.
        /// </summary>
        private PrefabBasedPool _pool;
        /// <summary>
        /// Fade In time.
        /// </summary>
        private float _fadeInTime;
        /// <summary>
        /// Fade Out time.
        /// </summary>
        private float _fadeOutTime;
        /// <summary>
        /// Volume of the sound.
        /// </summary>
        private float _volume;
        /// <summary>
        /// Pitch of the sound.
        /// </summary>
        private float _pitch;
        private bool _isDespawnOnFinishedPlaying = true;
        #endregion

        #region Public methods and properties
        /// <summary>
        /// Check whether SoundObject should despawn after finishing playing.
        /// </summary>
        public bool isDespawnOnFinishedPlaying {
            get { return _isDespawnOnFinishedPlaying; }
            set { _isDespawnOnFinishedPlaying = value; }
        }
        
        /// <summary>
        /// AudioClip name played.
        /// </summary>
        public string clipName
        {
            get
            {
                return _clip != null ? _clip.name : "NONE";
            }
        }

        /// <summary>
        /// Gets the SoundObject's AudioSource.
        /// </summary>
        public AudioSource source
        {
            get { return _source; }
        }

        /// <summary>
        /// Gets the SoundObject's ID.
        /// </summary>
        public string ID {
            get { return _id; }
        }

        /// <summary>
        /// Prepares the SoundObject for playing an AudioClip.
        /// </summary>
        /// <param name="id">SoundObject's ID</param>
        /// <param name="clip">AudioClip to play</param>
        /// <param name="volume">volume of the sound.</param>
        /// <param name="fadeInTime">Fade In Time</param>
        /// <param name="fadeOutTime">Fade Out Time</param>
        /// <param name="mixer">Audio Mixer group</param>
        /// <param name="pitch">Pitch of the sound</param>
        public void Setup(string id, AudioClip clip, float volume, float fadeInTime = 0f, float fadeOutTime = 0f, AudioMixerGroup mixer = null, float pitch = 1f)
        {
            _id = id;
            _clip = clip;
            gameObject.name = _id;
            if (_source == null)
                _source = GetComponent<AudioSource>();

            _source.volume = 0;
            _source.time = 0f;
            _source.outputAudioMixerGroup = mixer;
            _volume = volume;
            _pitch = pitch;
            _fadeInTime = fadeInTime;
            _fadeOutTime = fadeOutTime;
        }

        /// <summary>
        /// Plays the AudioSource.
        /// </summary>
        public void Play()
        {
            if (_source == null)
                _source = GetComponent<AudioSource>();
            _source.clip = _clip;
            gameObject.SetActive(true);
            _source.pitch = _pitch;
            StartCoroutine(FadeRoutine(_fadeInTime, _volume));
            _source.Play();
            _isFree = false;
            _playingRoutine = StartCoroutine(PlayingRoutine());
        }

        /// <summary>
        /// Pauses the AudioSource.
        /// </summary>
        public void Pause()
        {
            if (_source == null)
                return;
            _isPaused = true;
            _source.Pause();
        }

        /// <summary>
        /// Resumes from Pause.
        /// </summary>
        public void Resume()
        {
            if (_source == null)
                return;
            _source.Play();
            _isPaused = false;
        }

        /// <summary>
        /// Stops the SoundObject from playing.
        /// </summary>
        public void Stop()
        {
            if (_playingRoutine == null)
                return;

            StartCoroutine(StopRoutine());
        }

        /// <summary>
        /// Internal test method
        /// </summary>
        [ContextMenu("Test Play")]
        private void TestPlay()
        {
            Play();
        }
        #endregion

        /// <summary>
        /// Fades the volume of the AudioSource.
        /// </summary>
        /// <param name="fadeTime">time to fade</param>
        /// <param name="value">target volume to fade to.</param>
        /// <returns></returns>
        private IEnumerator FadeRoutine(float fadeTime, float value)
        {
            if (fadeTime < 0.1f)
            {
                _source.volume = value;
                yield break;
            }

            float initVal = _source.volume;
            float fadeSpeed = 1f / (fadeTime / Time.deltaTime);
            for (float t = 0f; t < 1f; t += fadeSpeed)
            {
                float val = Mathf.SmoothStep(initVal, value, t);
                _source.volume = val;
                yield return null;
            }

            _source.volume = value;
        }

        /// <summary>
        /// Internal method to Stop the SoundObject.
        /// </summary>
        /// <returns></returns>
        private IEnumerator StopRoutine()
        {
            StopCoroutine(_playingRoutine);
            yield return StartCoroutine(FadeRoutine(_fadeOutTime, 0f));
            _source.Stop();
            _source.clip = null;
            _playingRoutine = null;
            _isFree = true;
            _volume = 0f;
            _source.time = 0f;
            _source.pitch = 1f;

            if (isDespawnOnFinishedPlaying)
                _pool.Despawn(gameObject);

            if (OnFinishedPlaying != null)
            {
                OnFinishedPlaying(this);
            }
        }

        /// <summary>
        /// Internal method to play the SoundObject.
        /// </summary>
        /// <returns></returns>
        private IEnumerator PlayingRoutine()
        {
            while (true)
            {
                yield return null;
                float fadeOutTrigger = _source.clip.length - _fadeOutTime;
                if (_source.time >= fadeOutTrigger)
                {
                    yield return StartCoroutine(FadeRoutine(_fadeOutTime, 0f));
                }
                if (!_source.isPlaying && !_isPaused)
                {
                    break;
                }
            }

            _source.clip = null;
            _playingRoutine = null;
            _isFree = true;
            _volume = 0f;
            _source.time = 0f;

            if (isDespawnOnFinishedPlaying)
                _pool.Despawn(gameObject);

            if (OnFinishedPlaying != null)
            {
                OnFinishedPlaying(this);
            }
        }

        #region IPoolable methods
        /// <summary>
        /// Check IPoolable
        /// </summary>
        PrefabBasedPool IPoolable.pool {
            get { return _pool; }
            set { _pool = value; }
        }

        /// <summary>
        /// Check IPoolable
        /// </summary>
        public bool IsFree()
        {
            return _isFree;
        }
        #endregion
    }

}
