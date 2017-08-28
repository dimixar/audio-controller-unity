using System.Collections.Generic;
using UnityEngine;

namespace OSSC
{
    [System.Serializable]
    public class SoundTags
    {
        #region Private fields
        [SerializeField]
        private List<TagData> _tagsData;

        private int _lastID = 0;
        #endregion

        public SoundTags()
        {
            _tagsData = new List<TagData>();
        }

        #region Public methods and properties

        public TagData[] ToArray()
        {
            return _tagsData.ToArray();
        }

        public string[] ToArrayNames()
        {
            string[] names = new string[_tagsData.Count];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = _tagsData[i].name;
            }

            return names;
        }

        public int[] ToArrayIDs()
        {
            int[] ids = new int[_tagsData.Count];
            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = _tagsData[i].ID;
            }

            return ids;
        }

        public TagData GetTagDataByName(string name)
        {
            return _tagsData.Find(data => data.name.Equals(name.ToLower()));
        }

        public TagData GetTagDataByID(int ID)
        {
            return _tagsData.Find(data => data.ID.Equals(ID));
        }

        public int GetTagIDByName(string name)
        {
            TagData result = _tagsData.Find(data => data.name.Equals(name.ToLower()));
            if (string.IsNullOrEmpty(result.name))
                return -1;

            return result.ID;
        }

        public string GetTagNameByID(int ID)
        {
            TagData result = _tagsData.Find(data => data.ID.Equals(ID));
            if (string.IsNullOrEmpty(result.name))
                return string.Empty;

            return result.name;
        }

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

        public void RemoveByTag(TagData data)
        {
            _tagsData.Remove(data);
        }
        #endregion
    }

    [System.Serializable]
    public struct TagData
    {
        public string name;
        public int ID;
    }
}
