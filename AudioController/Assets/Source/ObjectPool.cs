using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    #region Public fields
    public List<PrefabBasedPool> pools;
    #endregion
    #region Public methods and properties

    public GameObject GetFreeObject(GameObject prefab = null)
    {
        if (prefab == null)
            return null;

        PrefabBasedPool pool = pools.Find((x) => {
            return x.prefab == prefab;
        });

        if (pool != null)
            return pool.GetFreeObject();

        pool = new PrefabBasedPool(prefab);
        GameObject parent = new GameObject();
        parent.name = pool.prefab.name + " ::: POOL";
        parent.transform.parent = this.gameObject.transform;
        pool.parent = parent.transform;
        pools.Add(pool);
        return pool.GetFreeObject();
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
            if (x == null)
            {
                pool.Remove(x);
                return false;
            }
            var poolable = x.GetComponent<IPoolable>();
            return poolable.IsFree();
        });

        if (freeObj != null)
            return freeObj;

        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
        pool.Add(obj);

        return obj;
    }
}
