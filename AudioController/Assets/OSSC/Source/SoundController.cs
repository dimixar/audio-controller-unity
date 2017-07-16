using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OSSC.Model;

namespace OSSC
{
    [RequireComponent(typeof(ObjectPool))]
    public class SoundController : MonoBehaviour
    {
        #region Serialized Data
        public GameObject _defaultPrefab;
        private int _initialCueManagerSize = 10;

        #endregion

        #region Private fields

        private ObjectPool _pool;
        private CueManager _cueManager;

        #endregion

        #region Public methods and properties
        public CueManager CueManager
        {
            get { return _cueManager; }
        }

        public string _dbName;

        public SoundControllerData _database;

        public GameObject defaultPrefab
        {
            set
            {
                _defaultPrefab = value;
            }
        }

        public SoundCue Play(string[] names, bool isLooped = false)
        {
            return Play(names, (Transform)(null), isLooped);
        }
        public SoundCue Play(string[] names, Transform parent, bool isLooped = false)
        {
            return Play(names, parent, null, isLooped);
        }
        public SoundCue Play(string[] names, Transform parent, string categoryName, bool isLooped = false)
        {
            return Play(names, parent, 0f, 0f, categoryName, isLooped);
        }
        public SoundCue Play(string[] names, float fadeInTime = 0, float fadeOutTime = 0f, bool isLooped = false)
        {
            return Play(names, null, fadeInTime, fadeOutTime, null, isLooped);
        }
        public SoundCue Play(string[] names, Transform parent, float fadeInTime, float fadeOutTime = 0f, bool isLooped = false)
        {
            return Play(names, parent, fadeInTime, fadeOutTime, null, isLooped);
        }
        public SoundCue Play(string[] names, string categoryName, bool isLooped = false)
        {
            return Play(names, null, 0f, 0f, categoryName, isLooped);
        }
        public SoundCue Play(string[] names, string categoryName, float fadeInTime, float fadeOutTime = 0f, bool isLooped = false)
        {
            return Play(names, null, fadeInTime, fadeOutTime, categoryName, isLooped);
        }
        public SoundCue Play(string name, bool isLooped = false)
        {
            return Play(new[] {name}, isLooped);
        }
        public SoundCue Play(string name, Transform parent, bool isLooped = false)
        {
            return Play(new[] {name}, parent, isLooped);
        }
        public SoundCue Play(string name, Transform parent, string categoryName, bool isLooped = false)
        {
            return Play(new [] {name}, parent, categoryName, isLooped);
        }
        public SoundCue Play(string name, Transform parent, float fadeInTime, float fadeOutTime = 0f, bool isLooped = false)
        {
            return Play(new [] {name}, parent, fadeInTime, fadeOutTime, isLooped);
        }
        public SoundCue Play(string name, string categoryName = null, bool isLooped = false)
        {
            return Play(new[] { name }, categoryName, isLooped);
        }
        public SoundCue Play(string name, string categoryName, float fadeInTime, float fadeOutTime = 0f, bool isLooped = false)
        {
            return Play(new[] { name }, categoryName, fadeInTime, fadeOutTime, isLooped);
        }
        public SoundCue Play(string name, float fadeInTime = 0, float fadeOutTime = 0f, bool isLooped = false)
        {
            return Play(new[] { name }, fadeInTime, fadeOutTime, isLooped);
        }

        public void StopAll(bool shouldCallOnEndCallback = true)
        {
            _cueManager.StopAllCues(shouldCallOnEndCallback);
        }

        public void SetMute(string categoryName, bool value)
        {
            for (int i = 0; i < _database.items.Length; i++)
            {
                if (_database.items[i].name == categoryName)
                {
                    _database.items[i].isMute = value;
                }
            }
        }

        /// <summary>
        /// Creates a SoundCue and plays it.
        /// </summary>
        /// <param name="names">array of sounds to play.</param>
        /// <param name="parent">attaches soundObject to the transform.</param>
        /// <param name="fadeInTime">time to fade in volume.</param>
        /// <param name="fadeOutTime">time to fade out volume.</param>
        /// <param name="categoryName">filter sounds by a category.</param>
        /// <returns>A soundCue which can be subscribed to it's events.</returns>
        public SoundCue Play(string[] names, Transform parent = null, float fadeInTime = 0f, float fadeOutTime = 0f, string categoryName = null, bool isLooped = false)
        {
            UnityEngine.Assertions.Assert.IsNotNull(names, "[AudioController] names cannot be null");
            if (names != null)
                UnityEngine.Assertions.Assert.IsFalse(names.Length == 0, "[AudioController] names cannot have 0 strings");

            CategoryItem category = null;
            GameObject prefab = null;
            List<SoundItem> items = new List<SoundItem>();
            List<float> catVolumes = new List<float>();

            if (string.IsNullOrEmpty(categoryName) == false)
            {
                category = System.Array.Find(_database.items, (item) =>
                {
                    return item.name == categoryName;
                });

                // Debug.Log(category);
                if (category == null)
                    return null;

                prefab = category.usingDefaultPrefab ? _defaultPrefab : category.audioObjectPrefab;
                for (int i = 0; i < names.Length; i++)
                {
                    SoundItem item = System.Array.Find(category.soundItems, (x) =>
                    {
                        return x.name == names[i];
                    });

                    if (item != null && category.isMute == false)
                    {
                        catVolumes.Add(category.categoryVolume);
                        items.Add(item);
                    }
                }
            }
            else
            {
                prefab = _defaultPrefab;
                CategoryItem[] categoryItems = _database.items;
                for (int i = 0; i < names.Length; i++)
                {
                    SoundItem item = null;
                    item = items.Find((x) => names[i] == x.name);
                    if (item != null)
                    {
                        catVolumes.Add(catVolumes[items.IndexOf(item)]);
                        items.Add(item);
                        continue;
                    }

                    for (int j = 0; j < categoryItems.Length; j++)
                    {
                        item = System.Array.Find(categoryItems[j].soundItems, (x) => x.name == names[i]);
                        if (item != null && categoryItems[j].isMute == false)
                        {
                            catVolumes.Add(categoryItems[j].categoryVolume);
                            break;
                        }
                    }
                    if (item != null)
                        items.Add(item);
                }
            }

            if (items.Count == 0)
                return null;

            SoundCue cue = _cueManager.GetSoundCue();
            SoundCueData data;
            data.audioPrefab = prefab;
            data.sounds = items.ToArray();
            data.categoryVolumes = catVolumes.ToArray();
            data.fadeInTime = fadeInTime;
            data.fadeOutTime = fadeOutTime;
            data.isFadeIn = data.fadeInTime >= 0.1f;
            data.isFadeOut = data.fadeOutTime >= 0.1f;
            data.isLooped = isLooped;
            cue.AudioObject = _pool.GetFreeObject(prefab).GetComponent<SoundObject>();
            if (parent != null)
                cue.AudioObject.transform.SetParent(parent, false);
            cue.Play(data);

            return cue;
        }

        /// <summary>
        /// Creates a new copy of the AudioCue and plays it.
        /// </summary>
        /// <param name="cue">AudioCue to clone and play.</param>
        /// <returns>Cloned AudioCue.</returns>
        public SoundCue Play(SoundCue cue, bool isLooped = false)
        {
            return Play(cue, null, isLooped);
        }

        public SoundCue Play(SoundCue cue, Transform parent, bool isLooped = false)
        {
            return Play(cue, parent, 0f, 0f, isLooped);
        }

        public SoundCue Play(SoundCue cue, Transform parent, float fadeInTime, float fadeOutTime = 0f, bool isLooped = false)
        {
            var ncue = _cueManager.GetSoundCue();
            ncue.AudioObject = _pool.GetFreeObject(cue.Data.audioPrefab).GetComponent<SoundObject>();
            if (parent != null)
                ncue.AudioObject.transform.SetParent(parent, false);
            SoundCueData data = cue.Data;
            data.fadeInTime = fadeInTime;
            data.fadeOutTime = fadeOutTime;
            data.isFadeIn = data.fadeInTime >= 0.1f;
            data.isFadeOut = data.fadeOutTime >= 0.1f;
            data.isLooped = isLooped;
            ncue.Play(data);
            return ncue;
        }

        #endregion

        #region Private methods
        [ContextMenu("Play test1")]
        private void TestPlay()
        {
            Play("test1", true);
        }
        #endregion

        #region MonoBehaviour methods

        void Awake()
        {
            _pool = GetComponent<ObjectPool>();
            _cueManager = new CueManager(_initialCueManagerSize);
        }

        #endregion
    }

}
