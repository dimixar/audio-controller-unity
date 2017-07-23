using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OSSC
{
    public class SoundCueProxy : ISoundCue
    {
        #region Private fields
        private SoundCue _soundCue;
        private SoundCueData _data;
        #endregion
        #region Public Methods and Properties
        public SoundCue SoundCue
		{
			get {
                return _soundCue;
            }
			set {
                _soundCue = value;
            }
        }
        #endregion
        #region ISoundCue implementation
        public Action<string> OnPlayEnded
		{
			get {
                return _soundCue == null ? null : _soundCue.OnPlayEnded;
            }
			set {
                if (_soundCue != null)
				{
                    _soundCue.OnPlayEnded = value;
                }
            }
		}
        public Action<SoundCue> OnPlayCueEnded
		{
			get {
                return _soundCue == null ? null : _soundCue.OnPlayCueEnded;
            }
			set {
                if (_soundCue != null)
				{
                    _soundCue.OnPlayCueEnded = value;
                }
            }
		}
        public SoundObject AudioObject
		{
			get {
                return _soundCue == null ? null : _soundCue.AudioObject;
            }
			set {
                if (_soundCue != null)
				{
                    _soundCue.AudioObject = value;
                }
            }
		}

        public SoundCueData Data { get { return _data; } }

        public bool IsPlaying
		{
			get {
                return _soundCue == null ? false : _soundCue.IsPlaying;
            }
		}

        public int ID
		{
			get {
                return _soundCue == null ? -999 : _soundCue.ID;
            }
		}

		public void Play(SoundCueData data)
		{
            _data = data;
			if (_soundCue == null)
			{
                Debug.LogError("NO SOUND CUE to play!!!");
                return;
            }
            _soundCue.Play(data, this);
        }

        public void Pause()
        {
			if (_soundCue == null)
			{
                Debug.LogError("NO SOUND CUE to pause!!!");
                return;
            }
            _soundCue.Pause();
        }

        public void Resume()
        {
			if (_soundCue == null)
			{
                Debug.LogError("NO SOUND CUE to Resume!!!");
                return;
            }
            _soundCue.Resume();
        }

        public void Stop(bool shouldCallOnFinishedCue = true)
        {
			if (_soundCue == null)
			{
                Debug.LogError("NO SOUND CUE to Stop!!!");
                return;
            }
            _soundCue.Stop(shouldCallOnFinishedCue);
        }
        #endregion
    }
}
