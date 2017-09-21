using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OSSC;

namespace OSSC.Editor
{
    /// <summary>
    /// Draw the custom editor inspector for SoundObject
    /// </summary>
    [CustomEditor(typeof(SoundObject))]
    public class SoundObjectEditor : UnityEditor.Editor
    {
        /// <summary>
        /// SoundObject reference
        /// </summary>
        private SoundObject _ao;
        /// <summary>
        /// Checks whether to show controls or not
        /// </summary>
        private bool _showControls = false;

        /// <summary>
        /// Draws the inspector's GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            _ao = target as SoundObject;

            EditorGUILayout.LabelField("Sound Item", _ao.ID, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Current Clip", _ao.clipName);
            _showControls = EditorGUILayout.ToggleLeft("Show Controls", _showControls);
            if (_showControls == false)
                return;

            ShowControls();
        }

        /// <summary>
        /// Draw the editor Controls of the SoundObject.
        /// </summary>
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
