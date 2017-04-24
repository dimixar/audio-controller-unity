using UnityEngine;

namespace OSAC.Model
{
    [System.Serializable]
    public class SoundItem
    {
        public string name;
        public AudioClip clip;

        [RangeAttribute(0f, 1f)]
        public float volume = 1f;
    }

}
