using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used by the ObjectPool
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// Saves the pool that it belongs to.
    /// </summary>
    PrefabBasedPool pool {
        get; set;
    }

    /// <summary>
    /// Checks whether the poolable object is free.
    /// </summary>
    /// <returns>True - is Free, False - is busy</returns>
    bool IsFree();
}
