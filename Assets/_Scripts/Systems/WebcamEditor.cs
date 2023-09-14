using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Systems
{
    [CustomEditor(typeof(Webcam))]
    internal class WebcamEditor : Editor
    {
        public List<SerializedProperty> WebcamName = new();
        private static readonly List<GUIContent> DropDowns = new();

        private void OnEnable()
        {
            foreach (SerializedProperty webcamName in serializedObject.FindProperty("WebcamName"))
            {
                WebcamName.Add(webcamName);
                DropDowns.Add(new GUIContent(webcamName.stringValue));
            }
        }
    
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            WebcamName.Clear();
            DropDowns.Clear();
            foreach (SerializedProperty webcamName in serializedObject.FindProperty("WebcamName"))
            {
                WebcamName.Add(webcamName);
                DropDowns.Add(new GUIContent(webcamName.stringValue));
            }

            for (int i = 0; i < WebcamName.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(("Element " + i), "");
                Rect rect = EditorGUILayout.GetControlRect();
                if (EditorGUI.DropdownButton(rect, DropDowns[i], FocusType.Keyboard))
                    ShowDeviceSelector(rect, i);
                EditorGUILayout.EndHorizontal();
            }
            
        
            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }

        private void ShowDeviceSelector(Rect rect, int i)
        {
            GenericMenu menu = new();

            foreach (WebCamDevice device in WebCamTexture.devices)
            {
                menu.AddItem(new GUIContent(device.name), false, () => ChangeWebcam(device.name, i));
            
                menu.DropDown(rect);
            }
        }

        private void ChangeWebcam(string deviceName, int i)
        {
            serializedObject.Update();
            WebcamName[i].stringValue = deviceName;
            DropDowns[i].text = deviceName;
            serializedObject.ApplyModifiedProperties();
        }
    }
}