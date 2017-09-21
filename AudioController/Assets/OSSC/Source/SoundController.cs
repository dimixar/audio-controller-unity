using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OSSC.Model;

namespace OSSC
{
    /// <summary>
    /// The main class that is used for Playing and controlling all sounds.
    /// </summary>
    [RequireComponent(typeof(ObjectPool))]
    public class SoundController : MonoBehaviour
    {
        #region Serialized Data
        /// <summary>
        /// Default prefab with SoundObject and AudioSource.
        /// It is used by the Soundcontroller to play SoundCues.
        /// </summary>
        public GameObject _defaultPrefab;
        /// <summary>
        /// Saves all the data that the SoundController uses.
        /// </summary>
        public SoundControllerData _database;

        #endregion

        #region Private fields

        /// <summary>
        /// Gives instances of GameObjects thrown in it.
        /// </summary>
        private ObjectPool _pool;
        /// <summary>
        /// Manages all created SoundCues.
        /// </summary>
        private CueManager _cueManager;
        /// <summary>
        /// Initial pool size of SoundCues for CueManager.
        /// </summary>
        private int _initialCueManagerSize = 10;

        #endregion

        #region Public methods and properties

        /// <summary>
        /// Set the default Prefab with SoundObject and AudioSource in it.
        /// </summary>
        public GameObject defaultPrefab
        {
            set
            {
                _defaultPrefab = value;
            }
        }

        /// <summary>
        /// Stop all Playing Sound Cues.
        /// </summary>
        /// <param name="shouldCallOnEndCallback">Control whether to call the OnEnd event, or not.</param>
        public void StopAll(bool shouldCallOnEndCallback = true)
        {
            _cueManager.StopAllCues(shouldCallOnEndCallback);
        }

        /// <summary>
        /// Set mute a category.
        /// </summary>
        /// <param name="categoryName">Name of the cateogory</param>
        /// <param name="value">True to mute, false to unmute</param>
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
        /// <param name="settings">A struct which contains all data for SoundController to work</param>
        /// <returns>A soundCue interface which can be subscribed to it's events.</returns>
        public ISoundCue Play(PlaySoundSettings settings)
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
            int tagID = _database.soundTags.GetTagIDByName(settings.tagName);
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
                        bool canAddItem = tagID == -1 || tagID == item.tagID;
                        if (canAddItem)
                        {
                            catVolumes.Add(category.categoryVolume);
                            items.Add(item);
                            categories.Add(category);
                        }
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
                        bool canAddItem = tagID == -1 || tagID == item.tagID;
                        if (canAddItem == false)
                            continue;
                        
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
                            bool canAddItem = tagID == -1 || tagID == item.tagID;
                            if (canAddItem == false)
                                continue;
                            catVolumes.Add(categoryItems[j].categoryVolume);
                            categories.Add(categoryItems[j]);
                            items.Add(item);
                            break;
                        }
                    }
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
        /// <summary>
        /// This method is called only when PlaySoundSettings has a SoundCue reference in it.
        /// Same as Play(), but much faster.
        /// </summary>
        /// <param name="settings">PlaySoundSettings instance with SoundCue reference in it.</param>
        /// <returns>Same SoundCue from PlaySoundSettings</returns>
        private SoundCueProxy PlaySoundCue(PlaySoundSettings settings)
        {
            SoundCueProxy cue = settings.soundCueProxy as SoundCueProxy;
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

        #region Internal tests
        [ContextMenu("Test play")]
        void Test()
        {
            PlaySoundSettings settings = new PlaySoundSettings();
            settings.Init();
            settings.name = "Test";
            var proxyCue = Play(settings);
            Debug.Log(proxyCue.ID);
        }

        [ContextMenu("Test Play looped")]
        void TestLoop()
        {
            PlaySoundSettings settings = new PlaySoundSettings();
            settings.Init();
            settings.name = "Test";
            settings.isLooped = true;
            var proxyCue = Play(settings);
            Debug.Log(proxyCue.ID);
        }

        [ContextMenu("Test sequence")]
        void TestSequence()
        {
            PlaySoundSettings settings = new PlaySoundSettings();
            settings.Init();
            settings.names = new[] {"Test", "Test1", "Test2"};
            var proxyCue = Play(settings);
            Debug.Log(proxyCue.ID);
        }

        [ContextMenu("Test sequence looped")]
        void TestSequenceLooped()
        {
            PlaySoundSettings settings = new PlaySoundSettings();
            settings.Init();
            settings.names = new[] {"Test", "Test1", "Test2"};
            settings.isLooped = true;
            var proxyCue = Play(settings);
            Debug.Log(proxyCue.ID);
        }

        [ContextMenu("Test sequence plays 2 times")]
        void TestSequence2TimesPlay()
        {
            PlaySoundSettings settings = new PlaySoundSettings();
            settings.Init();
            settings.names = new[] {"Test", "Test1", "Test2"};
            var proxyCue = Play(settings);
            proxyCue.OnPlayCueEnded += cue =>
            {
                var sett = new PlaySoundSettings();
                sett.soundCueProxy = proxyCue;
                proxyCue = Play(sett);
            };
            Debug.Log(proxyCue.ID);
        }
        #endregion
        
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
        /// <summary>
        /// Name of the soundItem to be played
        /// </summary>
        public string name;
        /// <summary>
        /// A list of sound Items to be played consecutively
        /// </summary>
        public string[] names;
        /// <summary>
        /// Attach the Playing sound to a Specific GameObject
        /// </summary>
        public Transform parent;
        /// <summary>
        /// Fade In time of the whole SoundCue
        /// </summary>
        public float fadeInTime;
        /// <summary>
        /// Fade Out time of the whole SoundCue
        /// </summary>
        public float fadeOutTime;
        /// <summary>
        /// Play SoundItems from a specific Category
        /// </summary>
        public string categoryName;
        /// <summary>
        /// Control whether the SoundCue should loop
        /// </summary>
        public bool isLooped;
        /// <summary>
        /// Use the same SoundCue to play again the sounds played in that SoundCue
        /// This is recommended to do, because searching by names all the Sounds to play is very expensive.
        /// </summary>
        public ISoundCue soundCueProxy;
        /// <summary>
        /// Play soundItems that correspond to the tag
        /// </summary>
        public string tagName;

        /// <summary>
        /// Initializes the PlaySoundSettings with predefined values. It is required to be called after the creation
        /// of the PlaySoundSettings instance.
        /// </summary>
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
            tagName = string.Empty;
        }
    }

}
