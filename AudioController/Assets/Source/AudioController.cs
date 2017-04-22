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

        [SerializeField]
        private GameObject _defaultPrefab;

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
        /// Plays the first found Sound Item that has that name.
        /// </summary>
        /// <param name="name">The name of the sound item.</param>
        public void Play(string name)
        {
            PlayImpl(name);
        }

        public void PlayFromCategory(string categoryName, string name = null)
        {
            //TODO: Add implementation
        }

        #endregion

        #region Private methods
        private void PlayImpl(string name)
        {
            SoundItem item = null;
            System.Predicate<SoundItem> soundItemMatch = (x) => x.name == name;
            CategoryItem category = System.Array.Find(_database.items, (x) =>
            {
                item = System.Array.Find(x.soundItems, soundItemMatch);
                return item != null;
            });

            if (category == null)
                return;

            GameObject prefab = category.usingDefaultPrefab ? _defaultPrefab : category.audioObjectPrefab;
            GameObject obj = _pool.GetFreeObject(prefab);
            var audioObj = obj.GetComponent<AudioObject>();
            // TODO: Add random id generator
            audioObj.Setup("TEMP", item.clip);
            audioObj.Play();
        }
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

        #endregion
    }

}
