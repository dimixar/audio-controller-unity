using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OSSC.Model
{
    /// <summary>
    /// SoundController's Database.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSoundControllerData", menuName = "Sound Controller/New SoundControllerData")]
    public class SoundControllerData : ScriptableObject
    {
        /// <summary>
        /// Stores all created Categories.
        /// </summary>
        public CategoryItem[] items;
        /// <summary>
        /// Checks in editor whether the categories should fold out or not.
        /// </summary>
        public bool foldOutCategories = false;
        /// <summary>
        /// check if editor should fold out the tags or not.
        /// </summary>
        public bool foldOutTags = false;
        /// <summary>
        /// Database name.
        /// </summary>
        public string assetName;
        /// <summary>
        /// Stores the Created tags from Editor.
        /// </summary>
        public SoundTags soundTags;
    }
}