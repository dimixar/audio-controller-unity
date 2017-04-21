using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    #region Public fields
    public List<PrefabBasedPool> pools;
    #endregion
    #region Public methods and properties

    public AudioObject GetFreeAudioObject(GameObject prefab = null)
    {
        if (prefab == null)
            return null;

        return null;
    }

    #endregion

    #region Monobehaviour methods
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        pools = new List<PrefabBasedPool>();
    }
    #endregion
}

[System.Serializable]
public class PrefabBasedPool
{
    public PrefabBasedPool(GameObject prefab)
    {
        pool = new List<GameObject>();
        this.prefab = prefab;
    }
    public GameObject prefab;
    public List<GameObject> pool;

    /// <summary>
    /// Where pooled objects will reside.
    /// </summary>
    public Transform parent;

    public GameObject GetFreeObject()
    {
        GameObject freeObj = pool.Find((x) => {
            var poolable = x.GetComponent<IPoolable>();
            return poolable.IsFree();
        });

        if (freeObj != null)
            return freeObj;

        var obj = GameObject.Instantiate(prefab, parent);
        pool.Add(obj);

        return obj;
    }
}
