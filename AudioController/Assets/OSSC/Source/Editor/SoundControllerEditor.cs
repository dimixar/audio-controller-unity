using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OSSC.Model;
using UnityEngine.Audio;

namespace OSSC.Editor
{
    [CustomEditor(typeof(SoundController))]
    public class SoundControllerEditor : UnityEditor.Editor
    {
        private const int NAME_ABV_LEN = 50;
        private SoundController _ac;
        private string categoryNameSearch = "";

        private string _dbPath;
        private string _dbName;

        public override void OnInspectorGUI()
        {
            _ac = target as SoundController;

            _ac._defaultPrefab = (GameObject)EditorGUILayout.ObjectField("Default AudioObject prefab", (Object)(_ac._defaultPrefab), typeof(GameObject), false);
            if (_ac._database == null)
            {
                _dbName = EditorGUILayout.TextField("Asset Name", _dbName);
                _dbPath = EditorGUILayout.TextField("Relative path", _dbPath);
            }
            else
            {
                _dbName = _ac._database.assetName;
                _dbPath = _ac._database.relativePath;
                EditorGUILayout.LabelField("Asset Name", _dbName, EditorStyles.largeLabel);
                EditorGUILayout.LabelField("Relative Path", _dbPath, EditorStyles.miniBoldLabel);
            }

            string path = "Assets/" + (string.IsNullOrEmpty(_dbPath) ? "" : (_dbPath + "/")) + _dbName + ".asset";

            if (string.IsNullOrEmpty(_ac._dbName))
            {
                _ac._dbName = path;
            }
            else
            {
                if (!string.IsNullOrEmpty(_dbPath) && !string.IsNullOrEmpty(_dbName))
                    if ((_ac._dbName.Contains(_dbPath) && _ac._dbName.Contains(_dbName)) == false)
                    {
                        _ac._dbName = path;
                    }
            }

            if (_ac._database == null)
            {
                SoundControllerData db = AssetDatabase.LoadAssetAtPath<SoundControllerData>(_ac._dbName);
                if (db == null)
                {
                    if (GUILayout.Button("Create Database"))
                    {
                        var asset = ScriptableObject.CreateInstance<SoundControllerData>();
                        AssetDatabase.CreateAsset(asset, _ac._dbName);

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        _ac._database = asset;
                        _ac._database.relativePath = _dbPath;
                        _ac._database.assetName = _dbName;
                    }
                }
                else
                {
                    _ac._database = db;
                }
            }
            else
            {
                DrawMain();
            }

            EditorUtility.SetDirty(target);
            EditorUtility.SetDirty(this);
            if (_ac._database != null)
                EditorUtility.SetDirty(_ac._database);
        }

        private void DrawMain()
        {
            if (_ac._database == null)
                return;
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

        private void DrawCategory(Model.CategoryItem item, int index)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            item.name = EditorGUILayout.TextField("Name", item.name);
            item.audioObjectPrefab = (GameObject)EditorGUILayout.ObjectField("Category AO prefab", item.audioObjectPrefab, typeof(GameObject), false);
            item.usingDefaultPrefab = item.audioObjectPrefab == null;
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

        private void DrawSoundItem(Model.SoundItem item, int index, Model.SoundItem[] items)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            item.name = EditorGUILayout.TextField("Name", item.name);

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
            item.volume = EditorGUILayout.Slider("Volume", item.volume, 0f, 1f);
            string nameAbv = "";
            if (string.IsNullOrEmpty(item.name) == false)
                nameAbv = item.name.Length > NAME_ABV_LEN? item.name.Substring(0, NAME_ABV_LEN) : item.name;
            if (GUILayout.Button("Delete Item " + nameAbv))
            {
                DeleteSoundItem(index, items);
            }
            EditorGUILayout.EndVertical();
        }

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
    }
}