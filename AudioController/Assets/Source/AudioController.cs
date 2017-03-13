using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

    void OnEnable()
    {
        /*var asset = ScriptableObject.CreateInstance<AudioControllerData>();*/
        //AssetDatabase.CreateAsset(asset, "ACDB");

        //AssetDatabase.SaveAssets();
        /*AssetDatabase.Refresh();*/
    }
}
