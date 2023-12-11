using System.Collections.Generic;
using System.Linq;
using Enums;
using Manager;
using Scriptables;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Utilities;

namespace UI
{
    public class MainUI : Singleton<MainUI>
    {
        [SerializeField] private UIDocument UIDocument;
        private static VisualElement _root;
        [SerializeField] public string SelectedWebcam;
        [SerializeField] public string SelectedLevel;
        private List<ScriptableLevel> _levels;

        public void OnEnable()
        {
            // Each window contains a root VisualElement object
            _root = UIDocument.rootVisualElement;

            _root.Q<Button>("StartStopButton").RegisterCallback<ClickEvent>(OnStartStopButtonClicked);
            
            CreateWebcamDropdown();
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
        
        public string GetMainWebcam() => SelectedWebcam;
        
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
