using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CategoryItem
{
    public string name;
    public SoundItem[] soundItems;
    public GameObject audioObjectPrefab;
    public bool usingDefaultPrefab = true;
}
