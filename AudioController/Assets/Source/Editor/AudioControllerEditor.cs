using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OSAC.Model;

namespace OSAC.Editor
{
    [CustomEditor(typeof(AudioController))]
    public class AudioControllerEditor : UnityEditor.Editor
    {
        private AudioController _ac;

        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();

            _ac = target as AudioController;

            _ac._defaultPrefab = (GameObject)EditorGUILayout.ObjectField("Default AudioObject prefab", (Object)(_ac._defaultPrefab), typeof(GameObject), false);
            _ac._dbPath = EditorGUILayout.TextField("DB Path", _ac._dbPath);
            _ac._dbName = EditorGUILayout.TextField("DB Name", _ac._dbName);

            string path = "Assets/" + (string.IsNullOrEmpty(_ac._dbPath) ? "" : (_ac._dbPath + "/")) + _ac._dbName + ".asset";

            if (_ac._database == null)
            {
                AudioControllerData db = AssetDatabase.LoadAssetAtPath<AudioControllerData>(path);
                if (db == null)
                {
                    if (GUILayout.Button("Create Database"))
                    {
                        var asset = ScriptableObject.CreateInstance<AudioControllerData>();
                        AssetDatabase.CreateAsset(asset, path);

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        _ac._database = asset;
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

            EditorUtility.SetDirty(this);
        }

        private void DrawMain()
        {
            if (GUILayout.Button("DELETE DATA"))
            {
                AssetDatabase.DeleteAsset("Assets/" + (string.IsNullOrEmpty(_ac._dbPath) ? "" : (_ac._dbPath + "/")) + _ac._dbName + ".asset");
                return;
            }
            DrawCategories(_ac._database);
        }

        private void DrawCategories(Model.AudioControllerData db)
        {
            if (GUILayout.Button("ADD CATEGORY"))
            {
                var category = new Model.CategoryItem();
                var categories = new Model.CategoryItem[db.items != null ? db.items.Length + 1 : 1];
                if (db.items != null)
                    db.items.CopyTo(categories, 0);
                categories[categories.Length - 1] = category;
                db.items = categories;
            }

            if (db.items == null)
                return;
            if (db.items.Length == 0)
                return;

            db.foldOutCategories = EditorGUILayout.Foldout(db.foldOutCategories, "CATEGORIES", true);
            if (!db.foldOutCategories)
                return;

            for (int i = db.items.Length - 1; i >= 0; i--)
            {
                DrawCategory(db.items[i]);
            }
        }

        private void DrawCategory(Model.CategoryItem item)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            item.name = EditorGUILayout.TextField("Name", item.name);
            item.audioObjectPrefab = (GameObject)EditorGUILayout.ObjectField("Category AO prefab", item.audioObjectPrefab, typeof(GameObject), false);
            item.usingDefaultPrefab = item.audioObjectPrefab == null;
            item.categoryVolume = EditorGUILayout.Slider("Category Volume", item.categoryVolume, 0f, 1f);
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
            item.foldOutSoundItems = DrawSoundItems(item.soundItems, item.foldOutSoundItems);
            EditorGUILayout.EndVertical();
        }

        private bool DrawSoundItems(Model.SoundItem[] items, bool foldOut)
        {
            if (items == null || items.Length == 0)
                return foldOut;

            EditorGUI.indentLevel++;
            foldOut = EditorGUILayout.Foldout(foldOut, "SOUND ITEMS", true);
            if (foldOut)
            {
                for (int j = items.Length - 1; j >= 0; j--)
                {
                    DrawSoundItem(items[j]);
                }
            }
            EditorGUI.indentLevel--;
            return foldOut;
        }

        private void DrawSoundItem(Model.SoundItem item)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            item.name = EditorGUILayout.TextField("Name", item.name);
            item.clip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", item.clip, typeof(AudioClip), false);
            item.volume = EditorGUILayout.Slider("Volume", item.volume, 0f, 1f);
            EditorGUILayout.EndVertical();
        }
    }
}