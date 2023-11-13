using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Enums;
using Manager;
using Scriptables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

#if UNITY_EDITOR
namespace EditorUI
{
    public class EditorUI : EditorWindow
    {
        [SerializeField] private VisualTreeAsset VisualTreeAsset;
        private static VisualElement _root;

        [SerializeField] public string SelectedWebcam;
        [SerializeField] public string SelectedSecondaryWebcam;
        [SerializeField] public string SelectedLevel;
        [SerializeField] public string RestBasePath;
        [SerializeField] public string UserID;
        private List<ScriptableLevel> _levels;
        private IBinding _binding;


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

            if (RestBasePath == "")
                RestBasePath = _root.Q<TextField>("RestBasePath").value;
            _root.Q<TextField>("RestBasePath").value = RestBasePath;
            _root.Q<TextField>("RestBasePath").RegisterValueChangedCallback(evt => RestBasePath = evt.newValue);
            
            _root.Q<TextField>("UserID").value = UserID;
            _root.Q<TextField>("UserID").RegisterValueChangedCallback(evt => UserID = evt.newValue);

            _root.Q<Button>("StartStopButton").RegisterCallback<ClickEvent>(OnStartStopButtonClicked);

            if (EditorUIFerStats.Instance != null)
            {
                SerializedObject ferStats = new(EditorUIFerStats.Instance);
                _root.Q<Label>("PendingRestResponses").BindProperty(ferStats.FindProperty("CurrentActiveRestPosts"));
                _root.Q<Label>("TimeBetweenPosts").BindProperty(ferStats.FindProperty("CurrentTimeBetweenPosts"));
                _root.Q<Label>("PostsFPS").BindProperty(ferStats.FindProperty("CurrentPostsFPS"));
                _root.Q<Label>("TotalRestCalls").BindProperty(ferStats.FindProperty("TotalPosts"));
            }
            
            CreateWebcamDropdown();
            CreateSecondaryWebcamDropdown();
            CreateLevelDropdown();
        }

        private static void OnStartStopButtonClicked(ClickEvent evt)
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
                if (GameManager.Instance != null) GameManager.Instance.SetNewLevel(_levels.FirstOrDefault(l => l.name == SelectedLevel));
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

        private void CreateSecondaryWebcamDropdown()
        {
            List<string> webCamDevices = WebCamTexture.devices.Select(device => device.name).ToList();
            webCamDevices.Insert(0, "-");

            DropdownField dropdown = _root.Q<DropdownField>("SecondaryWebcamDropdown");
            dropdown.choices = webCamDevices;
            dropdown.index = webCamDevices.IndexOf(SelectedSecondaryWebcam);
            dropdown.RegisterValueChangedCallback(evt =>
            {
                SelectedSecondaryWebcam = evt.newValue;
            });
        }

        public static void SetRestResponseData(LogData logData)
        {
            Probabilities probabilities = logData.FerProbabilities;

            _root.Q<ProgressBar>("Anger").value = probabilities.anger;
            _root.Q<ProgressBar>("Disgust").value = probabilities.disgust;
            _root.Q<ProgressBar>("Fear").value = probabilities.fear;
            _root.Q<ProgressBar>("Happiness").value = probabilities.happiness;
            _root.Q<ProgressBar>("Neutral").value = probabilities.neutral;
            _root.Q<ProgressBar>("Sadness").value = probabilities.sadness;
            _root.Q<ProgressBar>("Surprise").value = probabilities.surprise;
        }

        public string GetMainWebcam() => SelectedWebcam;
        public string GetSecondaryWebcam() => SelectedSecondaryWebcam;

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

        public void SetWebcamTexture(WebCamTexture mainWebcam)
        {
            VisualElement webcamUI = _root.Q<VisualElement>("WebcamTexture");
        }

    }
}
#endif
