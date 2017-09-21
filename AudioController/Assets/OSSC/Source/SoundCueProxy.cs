using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OSSC
{
	/// <summary>
	/// Given by the SoundController to User as ISoundCue to control the playing SoundCue.
	/// </summary>
    public class SoundCueProxy : ISoundCue
    {
        #region Private fields
	    /// <summary>
	    /// The real sound cue played.
	    /// </summary>
        private SoundCue _soundCue;
	    /// <summary>
	    /// Data used by the SoundCue.
	    /// </summary>
        private SoundCueData _data;
        #endregion
        #region Public Methods and Properties
	    /// <summary>
	    /// Sets, Gets the SoundCue.
	    /// </summary>
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
	    /// <summary>
	    /// Check ISoundCue
	    /// </summary>
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
	    /// <summary>
	    /// Check ISoundCue
	    /// </summary>
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
	    /// <summary>
	    /// Check ISoundCue
	    /// </summary>
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

	    /// <summary>
	    /// Check ISoundCue
	    /// </summary>
        public SoundCueData Data { get { return _data; } }

	    /// <summary>
	    /// Check ISoundCue
	    /// </summary>
        public bool IsPlaying
		{
			get {
                return _soundCue != null && _soundCue.IsPlaying;
            }
		}

	    /// <summary>
	    /// Check ISoundCue
	    /// </summary>
        public int ID
		{
			get {
                return _soundCue == null ? -999 : _soundCue.ID;
            }
		}

	    /// <summary>
	    /// Check ISoundCue.
	    /// </summary>
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

	    /// <summary>
	    /// Check ISoundCue.
	    /// </summary>
        public void Pause()
        {
			if (_soundCue == null)
			{
                Debug.LogError("NO SOUND CUE to pause!!!");
                return;
            }
            _soundCue.Pause();
        }

	    /// <summary>
	    /// Check ISoundCue.
	    /// </summary>
        public void Resume()
        {
			if (_soundCue == null)
			{
                Debug.LogError("NO SOUND CUE to Resume!!!");
                return;
            }
            _soundCue.Resume();
        }

	    /// <summary>
	    /// Check ISoundCue.
	    /// </summary>
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
