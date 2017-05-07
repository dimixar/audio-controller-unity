using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OSAC
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioObject : MonoBehaviour, IPoolable
    {

        public System.Action<AudioObject> OnFinishedPlaying;

        #region private fields
        private string _id;
        private AudioClip _clip;
        private AudioSource _source;

        private Coroutine _playingRoutine;
        private bool _isPaused;

        private bool _isFree = true;
        private PrefabBasedPool _pool;
        #endregion

        #region Public methods and properties
        public bool isDespawnOnFinishedPlaying {
            get; set;
        }
        public string clipName
        {
            get
            {
                return _clip != null ? _clip.name : "NONE";
            }
        }

        public void Setup(string id, AudioClip clip)
        {
            _id = id;
            _clip = clip;
            gameObject.name = id;
        }

        public void Play()
        {
            if (_source == null)
                _source = GetComponent<AudioSource>();
            _source.clip = _clip;
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

            _source.Stop();
        }

        [ContextMenu("Test Play")]
        private void TestPlay()
        {
            Play();
        }
        #endregion

        private IEnumerator PlayingRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.05f);
                if (!_source.isPlaying && !_isPaused)
                {
                    break;
                }
            }

            if (OnFinishedPlaying != null)
            {
                OnFinishedPlaying(this);
            }

            _source.clip = null;
            _playingRoutine = null;
            _isFree = true;

            if (isDespawnOnFinishedPlaying)
                _pool.Despawn(gameObject);
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
