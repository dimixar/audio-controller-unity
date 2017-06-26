using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OSSC.Model
{
    public class SoundControllerData : ScriptableObject
    {
        public CategoryItem[] items;
        public bool foldOutCategories = false;
        public string relativePath;
        public string assetName;
    }
}