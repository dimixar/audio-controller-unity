using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(ObjectPool))]
public class AudioController : MonoBehaviour
{
    [HideInInspector]
    public string _dbName;

    // This Path starts relatively to Assets folder.
    [HideInInspector]
    public string _dbPath;

    public AudioControllerData _database;

    private ObjectPool _pool;

    public void Play(string name)
    {
        //TODO: Add Implementation
    }

    void Awake()
    {
        _pool = GetComponent<ObjectPool>();
    }

    void OnEnable()
    {
    }
}
