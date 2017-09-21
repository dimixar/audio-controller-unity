using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OSSC.Model
{
    /// <summary>
    /// Used by the SoundControllerData to store categories.
    /// </summary>
    [System.Serializable]
    public class CategoryItem
    {
        /// <summary>
        /// Category name
        /// </summary>
        public string name;
        /// <summary>
        /// Array of SoundItems
        /// </summary>
        public SoundItem[] soundItems;
        /// <summary>
        /// Alternative SoundObject prefab to use, instead of the Default one from SoundController.
        /// </summary>
        public GameObject audioObjectPrefab;
        /// <summary>
        /// Check whether to use alternative SoundObject prefab.
        /// </summary>
        public bool usingDefaultPrefab = true;

        /// <summary>
        /// Volume of the category
        /// </summary>
        [Range(0f, 1f)]
        public float categoryVolume = 1f;

        /// <summary>
        /// Used for Editor to save whether the SoundItems are folded out or not.
        /// </summary>
        public bool foldOutSoundItems = false;
        /// <summary>
        /// Save the last search name written in editor.
        /// </summary>
        public string soundsSearchName = "";
        /// <summary>
        /// Is Category mute?
        /// </summary>
        public bool isMute = false;
    }

}
