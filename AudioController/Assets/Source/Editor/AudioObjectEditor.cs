using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OSAC;

namespace OSAC.Editor
{
    [CustomEditor(typeof(AudioObject))]
    public class AudioObjectEditor : UnityEditor.Editor
    {
        private AudioObject _ao;
        private bool _showControls = false;

        public override void OnInspectorGUI()
        {
            _ao = target as AudioObject;

            EditorGUILayout.LabelField("Sound Item", _ao.ID, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Current Clip", _ao.clipName);
            _showControls = EditorGUILayout.ToggleLeft("Show Controls", _showControls);
            if (_showControls == false)
                return;

            ShowControls();
        }

        private void ShowControls()
        {
            if (_ao.source.isPlaying)
            {
                if (GUILayout.Button("Stop"))
                {
                    _ao.Stop();
                }
            }

            if (_ao.source.clip != null)
            {
                int minutes = (int)(_ao.source.time / 60f);
                int seconds = (int)(_ao.source.time - minutes * 60);
                EditorGUILayout.LabelField("Current Time", minutes + ":" + seconds);
                minutes = (int)(_ao.source.clip.length / 60f);
                seconds = (int)(_ao.source.clip.length - minutes * 60);
                EditorGUILayout.LabelField("Clip Time", minutes + ":" + seconds);
                _ao.source.time = EditorGUILayout.Slider("Seek", _ao.source.time, 0f, _ao.source.clip.length);
                Repaint();
            }
        }
    }
}
