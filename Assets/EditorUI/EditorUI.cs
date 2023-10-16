using System.Collections.Generic;
using System.Linq;
using Enums;
using Manager;
using Scriptables;
using Systems;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
namespace EditorUI
{
    public class EditorUI : EditorWindow
    {
        [SerializeField] private VisualTreeAsset VisualTreeAsset;
        private static VisualElement _root;

        [SerializeField] public string SelectedWebcam;
        [SerializeField] public string SelectedLevel;
        [SerializeField] public string RestBasePath;
        [SerializeField] public string UserID;
        private List<ScriptableLevel> _levels;


        [MenuItem("Window/EmojiHero Editor Window")]
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

            _root.Q<TextField>("RestBasePath").value = RestBasePath;
            _root.Q<TextField>("RestBasePath").RegisterValueChangedCallback(evt => RestBasePath = evt.newValue);
            
            _root.Q<TextField>("UserID").value = UserID;
            _root.Q<TextField>("UserID").RegisterValueChangedCallback(evt => UserID = evt.newValue);

            _root.Q<Button>("StartStopButton").RegisterCallback<ClickEvent>(OnStartStopButtonClicked);
            
            CreateWebcamDropdown();
            CreateLevelDropdown();
        }

        private void OnStartStopButtonClicked(ClickEvent evt)
        {
            GameManager.Instance.OnButtonPressed(UIType.StartStopLevel);
        }

        private void CreateLevelDropdown()
        {
            _levels =  Resources.LoadAll<ScriptableLevel>("Levels").ToList();

            DropdownField dropdown = _root.Q<DropdownField>("LevelSelect");
            foreach (ScriptableLevel level in _levels)
            {
                dropdown.choices.Add(level.name);
            }
            dropdown.index = _levels.IndexOf(_levels.FirstOrDefault(l => l.name == SelectedLevel));
            dropdown.RegisterValueChangedCallback(evt =>
            {
                SelectedLevel = evt.newValue;
                GameManager.Instance.SetNewLevel(_levels.FirstOrDefault(l => l.name == SelectedLevel));
            });
        }

        private void CreateWebcamDropdown()
        {
            List<string> webCamDevices = WebCamTexture.devices.Select(device => device.name).ToList();

            DropdownField dropdown = _root.Q<DropdownField>("WebcamDropdown");
            dropdown.choices = webCamDevices;
            dropdown.index = webCamDevices.IndexOf(SelectedWebcam);
            dropdown.RegisterValueChangedCallback(evt =>
            {
                SelectedWebcam = evt.newValue;
            });
        }

        public static void SetRestResponseData(Dictionary<EEmote, float>  response)
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

        public void ResetUserID()
        {
            _root.Q<TextField>("UserID").value = "";
            UserID = null;
        }

        public ScriptableLevel GetSelectedLevel()
        {
            return _levels.FirstOrDefault(l => l.name == SelectedLevel);
        }

        public void SetNewLevel(ScriptableLevel level)
        {
            SelectedLevel = level.name;
            CreateLevelDropdown();
        }
    }
}
#endif
