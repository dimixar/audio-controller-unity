using UnityEngine;

namespace OSSC.Model
{
    [System.Serializable]
    public class SoundItem
    {
        public string name;
        public int tagID = -1;
        public UnityEngine.Audio.AudioMixerGroup mixer;
        public AudioClip[] clips;
        public bool isRandomPitch;
        public CustomRange pitchRange = new CustomRange();
        public bool isRandomVolume;
        public CustomRange volumeRange = new CustomRange();

        [RangeAttribute(0f, 1f)]
        public float volume = 1f;
    }

    [System.Serializable]
    public class CustomRange
    {
        public float min = 1f;
        public float max = 1f;

        public float GetRandomRange()
        {
            return Random.Range(min, max);
        }
    }
}
