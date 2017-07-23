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
        public SoundCueProxy Play(PlaySoundSettings settings)
        {
            if (settings.soundCueProxy != null)
            {
                return PlaySoundCue(settings);
            }

            if (settings.names == null && string.IsNullOrEmpty(settings.name))
            {
                return null;
            }

            string[] names = null;
            string categoryName = settings.categoryName;
            float fadeInTime = settings.fadeInTime;
            float fadeOutTime = settings.fadeOutTime;
            bool isLooped = settings.isLooped;
            Transform parent = settings.parent;

            if (settings.names != null)
            {
                names = settings.names;
            }
            else
            {
                names = new[] { settings.name };
            }
            UnityEngine.Assertions.Assert.IsNotNull(names, "[AudioController] names cannot be null");
            if (names != null)
                UnityEngine.Assertions.Assert.IsFalse(names.Length == 0, "[AudioController] names cannot have 0 strings");

            CategoryItem category = null;
            GameObject prefab = null;
            List<SoundItem> items = new List<SoundItem>();
            List<float> catVolumes = new List<float>();
            List<CategoryItem> categories = new List<CategoryItem>();

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
                        categories.Add(category);
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
                        categories.Add(categories[items.IndexOf(item)]);
                        items.Add(item);
                        continue;
                    }

                    for (int j = 0; j < categoryItems.Length; j++)
                    {
                        item = System.Array.Find(categoryItems[j].soundItems, (x) => x.name == names[i]);
                        if (item != null && categoryItems[j].isMute == false)
                        {
                            catVolumes.Add(categoryItems[j].categoryVolume);
                            categories.Add(categoryItems[j]);
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
            data.categoriesForSounds = categories.ToArray();
            data.fadeInTime = fadeInTime;
            data.fadeOutTime = fadeOutTime;
            data.isFadeIn = data.fadeInTime >= 0.1f;
            data.isFadeOut = data.fadeOutTime >= 0.1f;
            data.isLooped = isLooped;
            cue.AudioObject = _pool.GetFreeObject(prefab).GetComponent<SoundObject>();
            if (parent != null)
                cue.AudioObject.transform.SetParent(parent, false);

            SoundCueProxy proxy = new SoundCueProxy();
            proxy.SoundCue = cue;
            proxy.Play(data);
            return proxy;
        }


        #endregion

        #region Private methods
        private SoundCueProxy PlaySoundCue(PlaySoundSettings settings)
        {
            SoundCueProxy cue = settings.soundCueProxy;
            Transform parent = settings.parent;
            float fadeInTime = settings.fadeInTime;
            float fadeOutTime = settings.fadeOutTime;
            bool isLooped = settings.isLooped;
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
            cue.SoundCue = ncue;
            cue.Play(data);
            return cue;
        }

        [ContextMenu("Test play")]
        void Test()
        {
            PlaySoundSettings settings = new PlaySoundSettings();
            settings.Init();
            settings.name = "Test";
            var proxyCue = Play(settings);
            Debug.Log(proxyCue.ID);
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

    /// <summary>
    /// Set the settings to play a particular cue with particular preferences.
    /// </summary>
    public struct PlaySoundSettings
    {
        public string name;
        public string[] names;
        public Transform parent;
        public float fadeInTime;
        public float fadeOutTime;
        public string categoryName;
        public bool isLooped;
        public SoundCueProxy soundCueProxy;

        public void Init()
        {
            name = string.Empty;
            names = null;
            parent = null;
            fadeInTime = 0f;
            fadeOutTime = 0f;
            categoryName = string.Empty;
            isLooped = false;
            soundCueProxy = null;
        }
    }

}
