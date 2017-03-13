using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class AudioController : MonoBehaviour 
{
    private ObjectPool _pool;
    
    public void Play(string name)
    {
        //TODO: Add Implementation
    }
    
    void Awake()
    {
        _pool = GetComponent<ObjectPool>();
    }
}
