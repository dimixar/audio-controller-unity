using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    private GameObject _defaultPrefab;

    #region Public methods and properties

    public GameObject defaultPrefab
    {
        set
        {
            _defaultPrefab = value;
        }
    }

    //TODO: Add possibility to differentiate between prefabs
    public AudioObject GetFreeAudioObject(GameObject prefab = null)
    {
        return null;
    }

    #endregion
}

[System.Serializable]
public class PrefabBasedPool
{
    public GameObject prefab;
    public List<AudioObject> audioObjects;
}
