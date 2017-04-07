using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(ObjectPool))]
public class AudioController : MonoBehaviour
{
    #region Serialized Data

    [SerializeField]
    private GameObject _defaultPrefab;

    #endregion

    #region Private fields

    private ObjectPool _pool;

    #endregion

    #region Public methods and properties

    [HideInInspector]
    public string _dbName;

    // This Path starts relatively to Assets folder.
    [HideInInspector]
    public string _dbPath;

    public AudioControllerData _database;

    public GameObject defaultPrefab
    {
        set
        {
            _defaultPrefab = value;
        }
    }

    public void Play(string name)
    {
        PlayImpl(name);
    }

    #endregion

    #region Private methods
    private void PlayImpl(string name) {
        //TODO: Add Implementation
    }
    #endregion

    #region MonoBehaviour methods

    void Awake()
    {
        _pool = GetComponent<ObjectPool>();
    }

    void OnEnable()
    {
    }

    #endregion
}
