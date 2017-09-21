using System.Collections.Generic;
using UnityEngine;

namespace OSSC
{
    /// <summary>
    /// Used By the SoundController for tagging SoundItems.
    /// </summary>
    [System.Serializable]
    public class SoundTags
    {
        #region Private fields
        /// <summary>
        /// SoundTags data
        /// </summary>
        [SerializeField]
        private List<TagData> _tagsData;

        private int _lastID = 0;
        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SoundTags()
        {
            _tagsData = new List<TagData>();
        }

        #region Public methods and properties

        /// <summary>
        /// Returns all data in form of an array
        /// </summary>
        /// <returns>Array of TagData.</returns>
        public TagData[] ToArray()
        {
            return _tagsData.ToArray();
        }

        /// <summary>
        /// Returns the names of the tags.
        /// </summary>
        /// <returns>string[] with names</returns>
        public string[] ToArrayNames()
        {
            string[] names = new string[_tagsData.Count];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = _tagsData[i].name;
            }

            return names;
        }

        /// <summary>
        /// Returns the IDs of the tags.
        /// </summary>
        /// <returns>int[] with ids</returns>
        public int[] ToArrayIDs()
        {
            int[] ids = new int[_tagsData.Count];
            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = _tagsData[i].ID;
            }

            return ids;
        }

        /// <summary>
        /// Gets TagData by name.
        /// </summary>
        /// <param name="name">name of the Tag.</param>
        /// <returns>TagData with the corresponding name.</returns>
        public TagData GetTagDataByName(string name)
        {
            return _tagsData.Find(data => data.name.Equals(name.ToLower()));
        }

        /// <summary>
        /// Gets TagData by ID.
        /// </summary>
        /// <param name="ID">ID of the Tag.</param>
        /// <returns>TagData with the corresponding ID.</returns>
        public TagData GetTagDataByID(int ID)
        {
            return _tagsData.Find(data => data.ID.Equals(ID));
        }

        /// <summary>
        /// Gets Tag ID by name.
        /// </summary>
        /// <param name="name">name of the Tag.</param>
        /// <returns>ID of the Tag.</returns>
        public int GetTagIDByName(string name)
        {
            TagData result = _tagsData.Find(data => data.name.Equals(name.ToLower()));
            if (string.IsNullOrEmpty(result.name))
                return -1;

            return result.ID;
        }

        /// <summary>
        /// Gets Tag name by ID
        /// </summary>
        /// <param name="ID">ID of the Tag</param>
        /// <returns>Name of the Tag.</returns>
        public string GetTagNameByID(int ID)
        {
            TagData result = _tagsData.Find(data => data.ID.Equals(ID));
            if (string.IsNullOrEmpty(result.name))
                return string.Empty;

            return result.name;
        }

        /// <summary>
        /// Sets a new Tag.
        /// </summary>
        /// <param name="name">Name of the Tag.</param>
        public void SetTag(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            string nameLowercase = name.ToLower();
            TagData result = _tagsData.Find(data => data.name.Equals(nameLowercase));
            if (string.IsNullOrEmpty(result.name) == false)
                return;

            result.name = nameLowercase;
            result.ID = _lastID;
            _lastID += 1;
            _tagsData.Add(result);
        }

        /// <summary>
        /// Removes a Tag by TagData.
        /// </summary>
        /// <param name="data">TagData that wants to be removed.</param>
        public void RemoveByTag(TagData data)
        {
            _tagsData.Remove(data);
        }
        #endregion
    }

    /// <summary>
    /// Used by the SoundTags to save Tags.
    /// </summary>
    [System.Serializable]
    public struct TagData
    {
        /// <summary>
        /// Tag Name
        /// </summary>
        public string name;
        /// <summary>
        /// Tag ID
        /// </summary>
        public int ID;
    }
}
