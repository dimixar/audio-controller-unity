using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OSAC.Model;

namespace OSAC
{
    [RequireComponent(typeof(ObjectPool))]
    public class AudioController : MonoBehaviour
    {
        #region Serialized Data
        public GameObject _defaultPrefab;

        #endregion

        #region Private fields

        private ObjectPool _pool;

        #endregion

        #region Public methods and properties

        [HideInInspector]
        public string _dbName;

        // This Path starts relatively to Assets folder.
        [HideInInspector]
        public string _dbPath;

        public AudioControllerData _database;

        public GameObject defaultPrefab
        {
            set
            {
                _defaultPrefab = value;
            }
        }

        /// <summary>
        /// Plays a sound item
        /// </summary>
        /// <param name="name">Name of the sound item.</param>
        /// <param name="categoryName">OPTIONAL: Using sound item only from that category.</param>
        /// <returns>An AudioCue object which is used to track currently playing sound.</returns>
        public AudioCue Play(string name, string categoryName = null)
        {
            return Play(new[] { name }, categoryName);
        }

        /// <summary>
        /// Plays a list of sound items in a row.
        /// </summary>
        /// <param name="names">Array with sound item names.</param>
        /// <param name="categoryName">OPTIONAL: Using sound items only from that category.</param>
        /// <returns>An AudioCue object which is used to track currently playing sound.</returns>
        public AudioCue Play(string[] names, string categoryName = null)
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

                Debug.Log(category);
                if (category == null)
                    return null;

                prefab = category.usingDefaultPrefab ? _defaultPrefab : category.audioObjectPrefab;
                for (int i = 0; i < names.Length; i++)
                {
                    SoundItem item = System.Array.Find(category.soundItems, (x) =>
                    {
                        return x.name == names[i];
                    });

                    if (item != null)
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
                        if (item != null)
                        {
                            catVolumes.Add(categoryItems[j].categoryVolume);
                            break;
                        }
                    }
                    if (item != null)
                        items.Add(item);
                }
            }

            Debug.Log("sound items count = " + items.Count);
            if (items.Count == 0)
                return null;

            AudioCue cue = new AudioCue();
            AudioCueData data;
            data.audioPrefab = prefab;
            data.sounds = items.ToArray();
            data.categoryVolumes = catVolumes.ToArray();
            cue.audioObject = _pool.GetFreeObject(prefab).GetComponent<AudioObject>();
            cue.Play(data);

            return cue;
        }

        /// <summary>
        /// Creates a new copy of the AudioCue and plays it.
        /// </summary>
        /// <param name="cue">AudioCue to clone and play.</param>
        /// <returns>Cloned AudioCue.</returns>
        public AudioCue Play(AudioCue cue)
        {
            var ncue = new AudioCue();
            ncue.audioObject = _pool.GetFreeObject(cue.data.audioPrefab).GetComponent<AudioObject>();
            ncue.Play(cue.data);
            return ncue;
        }

        public void PlayFromCategory(string categoryName, string name = null)
        {
            //TODO: Add implementation
        }

        #endregion

        #region Private methods
        #endregion

        #region MonoBehaviour methods

        void Awake()
        {
            _pool = GetComponent<ObjectPool>();
        }

        void OnEnable()
        {
        }

        [ContextMenu("Test Play")]
        void TestPlay()
        {
            Play("test");
        }


        [ContextMenu("Test Play 2")]
        void TestPlay2()
        {
            Play("test2");
        }

        [ContextMenu("Test Cue")]
        void TestCue()
        {
            Play(new[] { "test","test","test", "test2" });
        }

        #endregion
    }

}
