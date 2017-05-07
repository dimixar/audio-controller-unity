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

        public override void OnInspectorGUI()
        {
            _ao = target as AudioObject;

            EditorGUILayout.LabelField("Sound Item", _ao.ID, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Current Clip", _ao.clipName);
            if (_ao.source.isPlaying)
            {
                if (GUILayout.Button("Stop"))
                {
                    _ao.Stop();
                }
            }
        }
    }
}
