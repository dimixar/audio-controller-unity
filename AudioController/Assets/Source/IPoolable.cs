using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    PrefabBasedPool pool {
        get; set;
    }

    bool IsFree();
}
