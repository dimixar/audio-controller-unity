using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OSAC.Model
{
    public class AudioControllerData : ScriptableObject
    {
        public CategoryItem[] items;
        public bool foldOutCategories = false;
        public string relativePath;
        public string assetName;
    }
}