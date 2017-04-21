using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioObject : MonoBehaviour, IPoolable
{

    public System.Action<AudioObject> OnFinishedPlaying;

    #region private fields
    private string _id;
    private AudioClip _clip;
    private AudioSource _source;

    private Coroutine _playingRoutine;

    private bool _isFree = true;
    #endregion

    #region Public methods and properties
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
        Debug.Log("Playing Started");
        while (true)
        {
            Debug.Log("Checking if it's playing.");
            yield return new WaitForSeconds(0.05f);
            if (!_source.isPlaying)
            {
                Debug.Log("AudioSource Finished Playing");
                break;
            }
        }

        Debug.Log("Not playing anymore");

        if (OnFinishedPlaying != null)
        {
            OnFinishedPlaying(this);
        }

        _source.clip = null;
        _playingRoutine = null;
        _isFree = true;
    }

    #region IPoolable methods
    public bool IsFree()
    {
        return _isFree;
    }
    #endregion
}
