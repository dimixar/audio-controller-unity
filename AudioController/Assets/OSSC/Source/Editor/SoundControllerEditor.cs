using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEditor;
using OSSC.Model;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

namespace OSSC.Editor
{
    /// <summary>
    /// Draws the Custom Editor for SoundController
    /// </summary>
    [CustomEditor(typeof(SoundController))]
    public class SoundControllerEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Max search string length
        /// </summary>
        private const int NAME_ABV_LEN = 50;
        /// <summary>
        /// Max pitch limit range
        /// </summary>
        private const float PITCH_RANGE_MAX = 3f;
        /// <summary>
        /// Min pitch limit range
        /// </summary>
        private const float PITCH_RANGE_MIN = -3f;
        /// <summary>
        /// Reference to SoundController
        /// </summary>
        private SoundController _ac;
        /// <summary>
        /// Local category search string storage.
        /// </summary>
        private string categoryNameSearch = "";
        /// <summary>
        /// Local tag name string storage
        /// </summary>
        private string _tagName = "";

        /// <summary>
        /// Draws the Inspector GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _ac = target as SoundController;

            if (_ac._database == null)
            {
                EditorGUILayout.HelpBox("Create SoundControllerData asset, then throw it here.", MessageType.Info);
            }
            else
            {
                DrawMain();
            }

            EditorUtility.SetDirty(_ac);
            if (_ac._database != null)
                EditorUtility.SetDirty(_ac._database);
        }

        /// <summary>
        /// Draw the main window of the SoundController.
        /// </summary>
        private void DrawMain()
        {
            if (_ac._database == null)
                return;
            DrawSoundTags();
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            if (GUILayout.Button("DELETE DATA"))
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_ac._database));
                return;
            }
            var db = _ac._database;
            if (GUILayout.Button("ADD CATEGORY"))
            {
                var category = new Model.CategoryItem();
                var categories = new Model.CategoryItem[db.items != null ? db.items.Length + 1 : 1];
                if (db.items != null)
                    db.items.CopyTo(categories, 0);
                categories[categories.Length - 1] = category;
                db.items = categories;
            }
            EditorGUILayout.EndHorizontal();
            DrawCategories(_ac._database);
        }

        /// <summary>
        /// Draws the category
        /// </summary>
        /// <param name="db">Uses SoundControllerData to fetch Category data.</param>
        private void DrawCategories(Model.SoundControllerData db)
        {

            if (db.items == null)
                return;
            if (db.items.Length == 0)
                return;

            categoryNameSearch = EditorGUILayout.TextField("Search Category", categoryNameSearch);
            db.foldOutCategories = EditorGUILayout.Foldout(db.foldOutCategories, "CATEGORIES", true);
            if (!db.foldOutCategories)
                return;

            for (int i = db.items.Length - 1; i >= 0; i--)
            {
                if (!string.IsNullOrEmpty(db.items[i].name))
                    if (db.items[i].name.ToLower().Contains(categoryNameSearch.ToLower()) == false && string.IsNullOrEmpty(categoryNameSearch) == false)
                        continue;
                DrawCategory(db.items[i], i);
            }
        }

        /// <summary>
        /// Draws a CategoryItem.
        /// </summary>
        /// <param name="item">CategoryItem data</param>
        /// <param name="index">CategoryItem's index</param>
        private void DrawCategory(Model.CategoryItem item, int index)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            item.name = EditorGUILayout.TextField("Name", item.name);
            item.audioObjectPrefab = (GameObject)EditorGUILayout.ObjectField("Category AO prefab", item.audioObjectPrefab, typeof(GameObject), false);
            item.usingDefaultPrefab = item.audioObjectPrefab == null;
            item.isMute = EditorGUILayout.Toggle("Is Mute", item.isMute);
            item.categoryVolume = EditorGUILayout.Slider("Category Volume", item.categoryVolume, 0f, 1f);

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            if (GUILayout.Button("ADD SOUND ITEM"))
            {
                var soundItem = new Model.SoundItem();
                bool isNoSoundItems = item.soundItems == null;
                var soundItems = new Model.SoundItem[!isNoSoundItems ? item.soundItems.Length + 1 : 1];
                if (!isNoSoundItems)
                    item.soundItems.CopyTo(soundItems, 0);
                soundItems[soundItems.Length - 1] = soundItem;
                item.soundItems = soundItems;
            }
            string nameAbv = "";
            if (string.IsNullOrEmpty(item.name) == false)
                nameAbv = item.name.Length > NAME_ABV_LEN ? item.name.Substring(0, NAME_ABV_LEN) : item.name;
            if (GUILayout.Button("Delete " + nameAbv))
            {
                DeleteCategory(index);
            }
            EditorGUILayout.EndHorizontal();

            if (item.soundItems != null)
                if (item.soundItems.Length != 0)
                    item.soundsSearchName = EditorGUILayout.TextField("Search sound item", item.soundsSearchName);

            item.foldOutSoundItems = DrawSoundItems(item, item.foldOutSoundItems, item.soundsSearchName);

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Deletes a CategoryItem by index.
        /// </summary>
        /// <param name="index">CategoryItem's index</param>
        private void DeleteCategory(int index)
        {
            var categories = new Model.CategoryItem[_ac._database.items.Length - 1];
            int catInd = 0;
            for (int i = 0; i < _ac._database.items.Length; i++)
            {
                if (i == index)
                    continue;

                categories[catInd] = _ac._database.items[i];
                catInd += 1;
            }
            _ac._database.items = categories;
        }

        /// <summary>
        /// Draws all SoundItems from a CategoryItem.
        /// </summary>
        /// <param name="item">data that has SoundItem array</param>
        /// <param name="foldOut">Check if should fold out the window</param>
        /// <param name="searchName">Filter SoundItems by name</param>
        /// <returns></returns>
        private bool DrawSoundItems(Model.CategoryItem item, bool foldOut, string searchName)
        {
            Model.SoundItem[] items = item.soundItems;
            if (items == null || items.Length == 0)
                return foldOut;

            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            foldOut = EditorGUILayout.Foldout(foldOut, "SOUND ITEMS", true);
            if (items != null)
            {
                if (items.Length != 0)
                    if (GUILayout.Button("DELETE ALL SOUNDS"))
                    {
                        items = new Model.SoundItem[0];
                        item.soundItems = items;
                        return foldOut;
                    }
            }
            EditorGUILayout.EndHorizontal();

            if (foldOut)
            {
                for (int j = items.Length - 1; j >= 0; j--)
                {
                    if (!string.IsNullOrEmpty(items[j].name))
                        if (items[j].name.ToLower().Contains(searchName.ToLower()) == false && string.IsNullOrEmpty(searchName) == false)
                            continue;
                    DrawSoundItem(items[j], j, items);
                }
            }
            EditorGUI.indentLevel--;
            return foldOut;
        }

        /// <summary>
        /// Draw a SoundItem
        /// </summary>
        /// <param name="item">SoundItem data</param>
        /// <param name="index">SoundItem's index</param>
        /// <param name="items">The array of SoundItems</param>
        private void DrawSoundItem(Model.SoundItem item, int index, Model.SoundItem[] items)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            item.name = EditorGUILayout.TextField("Name", item.name);

            string[] names = _ac._database.soundTags.ToArrayNames();
            int[] ids = _ac._database.soundTags.ToArrayIDs();
            if (ids.Length != 0)
            {
                int indexTag = System.Array.IndexOf(ids, item.tagID);
                indexTag = EditorGUILayout.Popup("Tag", indexTag, names);
                if (indexTag != -1)
                    item.tagID = ids[indexTag];
            }
            else
            {
                item.tagID = -1;
            }
            

            item.mixer = (AudioMixerGroup)EditorGUILayout.ObjectField("Mixer", item.mixer, typeof(AudioMixerGroup), false);
            if (item.clips == null)
            {
                item.clips = new AudioClip[1];
            }
            int size = item.clips.Length;
            size = EditorGUILayout.IntField("Size", size);
            if (size != item.clips.Length)
            {
                var newClips = new AudioClip[size];
                for (int i = 0; i < item.clips.Length; i++)
                {
                    if (i >= size)
                        break;
                    newClips[i] = item.clips[i];
                }
                item.clips = newClips;
            }
            for (int i = 0; i < item.clips.Length; i++)
            {
                item.clips[i] = (AudioClip)EditorGUILayout.ObjectField(item.clips[i], typeof(AudioClip), false);
            }
            
            item.isRandomVolume =
                EditorGUILayout.ToggleLeft("Use Random Volume", item.isRandomVolume, EditorStyles.boldLabel);
            if (!item.isRandomVolume)
                item.volume = EditorGUILayout.Slider("Volume", item.volume, 0f, 1f);
            else
            {
                EditorGUILayout.LabelField("Min Volume:", item.volumeRange.min.ToString(), EditorStyles.largeLabel);
                EditorGUILayout.LabelField("Max Volume:", item.volumeRange.max.ToString(), EditorStyles.largeLabel);
                EditorGUILayout.MinMaxSlider("Volume Range", ref item.volumeRange.min, ref item.volumeRange.max, 0f, 1f);
            }

            item.isRandomPitch =
                EditorGUILayout.ToggleLeft("Use Random Pitch", item.isRandomPitch, EditorStyles.boldLabel);
            if (item.isRandomPitch)
            {
                EditorGUILayout.LabelField("Min Pitch:", item.pitchRange.min.ToString(), EditorStyles.largeLabel);
                EditorGUILayout.LabelField("Max Pitch:", item.pitchRange.max.ToString(), EditorStyles.largeLabel);
                EditorGUILayout.MinMaxSlider("Pitch Range", ref item.pitchRange.min, ref item.pitchRange.max, PITCH_RANGE_MIN, PITCH_RANGE_MAX);
            }
            string nameAbv = "";
            if (string.IsNullOrEmpty(item.name) == false)
                nameAbv = item.name.Length > NAME_ABV_LEN ? item.name.Substring(0, NAME_ABV_LEN) : item.name;
            if (GUILayout.Button("Delete Item " + nameAbv))
            {
                DeleteSoundItem(index, items);
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Delete SoundItem by index.
        /// </summary>
        /// <param name="index">SoundItem's index</param>
        /// <param name="items">Array which will have the SoundItem deleted from.</param>
        private void DeleteSoundItem(int index, Model.SoundItem[] items)
        {
            var category = System.Array.Find(_ac._database.items, (x) => {
                return x.soundItems == items;
            });

            var soundItems = new Model.SoundItem[category.soundItems.Length - 1];
            int soundInd = 0;
            for (int i = 0; i < category.soundItems.Length; i++)
            {
                if (i == index)
                    continue;
                soundItems[soundInd] = category.soundItems[i];
                soundInd += 1;
            }
            category.soundItems = soundItems;
        }

        /// <summary>
        /// Draw SoundTags.
        /// </summary>
        private void DrawSoundTags()
        {
            if (_ac._database.soundTags == null)
            {
                _ac._database.soundTags = new SoundTags();
            }

            _ac._database.foldOutTags = EditorGUILayout.Foldout(_ac._database.foldOutTags, "Tags", true);
            if (_ac._database.foldOutTags == false)
            {
                EditorGUILayout.HelpBox("Add tags filter sounds by them.", MessageType.Info);
                return;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawAddNewTag();
            
            TagData[] data = _ac._database.soundTags.ToArray();
            for (int i = 0; i < data.Length; i++)
            {
                DrawSoundTag(data[i], _ac._database.soundTags);
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draw a SoundTag
        /// </summary>
        /// <param name="data">SoundTag's data</param>
        /// <param name="tags">Reference to SoundTags</param>
        private void DrawSoundTag(TagData data, SoundTags tags)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ID: " + data.ID.ToString(), "name: " + data.name);
            if (GUILayout.Button("Delete"))
            {
                tags.RemoveByTag(data);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draw button to add new sound tags.
        /// </summary>
        private void DrawAddNewTag()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            _tagName = EditorGUILayout.TextField("Add Tag:", _tagName);
            if (GUILayout.Button("Add"))
            {
                _ac._database.soundTags.SetTag(_tagName);
                _tagName = string.Empty;
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}