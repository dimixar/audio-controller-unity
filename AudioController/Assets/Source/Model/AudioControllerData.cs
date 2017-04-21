using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControllerData : ScriptableObject
{
    public CategoryItem[] items;

    public CategoryItem getCategoryItem(string name)
    {
        return System.Array.Find(items, (x) =>
        {
            return x.name == name;
        });
    }
}
