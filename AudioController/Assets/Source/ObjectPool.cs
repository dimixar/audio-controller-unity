using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    private GameObject _prefab;

    private Stack<GameObject> _pool;

    public GameObject prefab
    {
        set {
            _prefab = value;
        }
    }

    //TODO: Add possibility to differentiate between prefabs
    public GameObject GetGameObject()
    {
        if (_pool != null)
        {
            if (_pool.Count > 0)
                return _pool.Pop();
            else
                return GameObject.Instantiate<GameObject>(_prefab);
        }
        else
        {
            _pool = new Stack<GameObject>();
            return GameObject.Instantiate<GameObject>(_prefab);
        }
    }

    public void Put(GameObject obj)
    {
        if (_pool == null)
            _pool = new Stack<GameObject>();

        _pool.Push(obj);
    }
}
