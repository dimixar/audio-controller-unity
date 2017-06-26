using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace OSSC
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundObject : MonoBehaviour, IPoolable
    {

        public System.Action<SoundObject> OnFinishedPlaying;

        #region private fields
        private string _id;
        private AudioClip _clip;
        private AudioSource _source;

        private Coroutine _playingRoutine;
        private bool _isPaused;

        private bool _isFree = true;
        private PrefabBasedPool _pool;
        private float _fadeInTime;
        private float _fadeOutTime;
        private float _volume;
        private bool _isDespawnOnFinishedPlaying = true;
        #endregion

        #region Public methods and properties
        public bool isDespawnOnFinishedPlaying {
            get { return _isDespawnOnFinishedPlaying; }
            set { _isDespawnOnFinishedPlaying = value; }
        }
        public string clipName
        {
            get
            {
                return _clip != null ? _clip.name : "NONE";
            }
        }

        public AudioSource source
        {
            get { return _source; }
        }

        public string ID {
            get { return _id; }
        }

        public void Setup(string id, AudioClip clip, float volume, float fadeInTime = 0f, float fadeOutTime = 0f, AudioMixerGroup mixer = null)
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
            _fadeInTime = fadeInTime;
            _fadeOutTime = fadeOutTime;
        }

        public void Play()
        {
            if (_source == null)
                _source = GetComponent<AudioSource>();
            _source.clip = _clip;
            gameObject.SetActive(true);
            StartCoroutine(FadeRoutine(_fadeInTime, _volume));
            _source.Play();
            _isFree = false;
            _playingRoutine = StartCoroutine(PlayingRoutine());
        }

        public void Pause()
        {
            if (_source == null)
                return;
            _isPaused = true;
            _source.Pause();
        }

        public void Resume()
        {
            if (_source == null)
                return;
            _source.Play();
            _isPaused = false;
        }

        public void Stop()
        {
            if (_playingRoutine == null)
                return;

            StartCoroutine(StopRoutine());
        }

        [ContextMenu("Test Play")]
        private void TestPlay()
        {
            Play();
        }
        #endregion

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

            if (isDespawnOnFinishedPlaying)
                _pool.Despawn(gameObject);

            if (OnFinishedPlaying != null)
            {
                OnFinishedPlaying(this);
            }
        }

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
        PrefabBasedPool IPoolable.pool {
            get { return _pool; }
            set { _pool = value; }
        }

        public bool IsFree()
        {
            return _isFree;
        }
        #endregion
    }

}
