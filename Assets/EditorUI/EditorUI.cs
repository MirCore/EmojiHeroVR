using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace EditorUI
{
    public class EditorUI : EditorWindow
    {
        [SerializeField] private VisualTreeAsset VisualTreeAsset;
        private static VisualElement _root;

        [SerializeField] public string SelectedWebcam;

        [MenuItem("Window/EmojiHero")]
        public static void ShowExample()
        {
            EditorUI wnd = GetWindow<EditorUI>();
            wnd.titleContent = new GUIContent("EmojiHero");
        }

        public static EditorUI Instance;

        private void OnEnable()
        {
            Instance = this;
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            _root = rootVisualElement;

            // Instantiate UXML
            VisualElement labelFromUxml = VisualTreeAsset.Instantiate();
            _root.Add(labelFromUxml);
            
            List<string> webCamDevices = WebCamTexture.devices.Select(device => device.name).ToList();

            DropdownField dropdown = _root.Q<DropdownField>("WebcamDropdown");
            dropdown.choices = webCamDevices;
            dropdown.index = webCamDevices.IndexOf(SelectedWebcam);
            dropdown.RegisterValueChangedCallback(evt =>
            {
                SelectedWebcam = evt.newValue;
                Debug.Log(SelectedWebcam);
            });
        }

        public static void SetData(Dictionary<EEmote, float>  response)
        {
            foreach (KeyValuePair<EEmote,float> pair in response)
            {
                _root.Q<ProgressBar>(pair.Key.ToString()).value = (_root.Q<ProgressBar>(pair.Key.ToString()).value + pair.Value) / 2;
            }
        }

        public string GetSelectedWebcam()
        {
            return SelectedWebcam;
        }
    }
}
