using UnityEngine;

namespace OSSC.Model
{
    [System.Serializable]
    public class SoundItem
    {
        public string name;
        public UnityEngine.Audio.AudioMixerGroup mixer;
        public AudioClip[] clips;

        [RangeAttribute(0f, 1f)]
        public float volume = 1f;
    }
}
