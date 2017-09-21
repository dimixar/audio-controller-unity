using UnityEngine;

namespace OSSC.Model
{
    /// <summary>
    /// Used by CategoryItem to store sounds data.
    /// </summary>
    [System.Serializable]
    public class SoundItem
    {
        /// <summary>
        /// SoundItem Name
        /// </summary>
        public string name;
        /// <summary>
        /// Tag ID associated with SoundItem
        /// </summary>
        public int tagID = -1;
        /// <summary>
        /// Mixer group associated with this SoundItem.
        /// </summary>
        public UnityEngine.Audio.AudioMixerGroup mixer;
        /// <summary>
        /// List of Audioclips
        /// </summary>
        public AudioClip[] clips;
        /// <summary>
        /// Is SoundItem using Random Pitch?
        /// </summary>
        public bool isRandomPitch;
        /// <summary>
        /// Range of the Random pitch.
        /// </summary>
        public CustomRange pitchRange = new CustomRange();
        /// <summary>
        /// Is SoundItem using Random Volume?
        /// </summary>
        public bool isRandomVolume;
        /// <summary>
        /// Range of the Random Volume.
        /// </summary>
        public CustomRange volumeRange = new CustomRange();

        /// <summary>
        /// Standard volume of the SoundItem
        /// </summary>
        [RangeAttribute(0f, 1f)]
        public float volume = 1f;
    }

    /// <summary>
    /// Used by SoundItem to store Random Ranges.
    /// </summary>
    [System.Serializable]
    public class CustomRange
    {
        /// <summary>
        /// Minimum limit
        /// </summary>
        public float min = 1f;
        /// <summary>
        /// Maximum limit
        /// </summary>
        public float max = 1f;

        /// <summary>
        /// Gets a random value from it's Minimum and Maximum limits.
        /// </summary>
        /// <returns></returns>
        public float GetRandomRange()
        {
            return Random.Range(min, max);
        }
    }
}
