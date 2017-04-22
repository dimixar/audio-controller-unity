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
            base.OnInspectorGUI();

            _ac = target as AudioController;

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

            EditorUtility.SetDirty(this);
        }
    }
}