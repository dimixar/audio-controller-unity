using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OSSC.Model
{
    [CreateAssetMenu(fileName = "NewSoundControllerData", menuName = "Sound Controller/New SoundControllerData")]
    public class SoundControllerData : ScriptableObject
    {
        public CategoryItem[] items;
        public bool foldOutCategories = false;
        public string assetName;
    }
}