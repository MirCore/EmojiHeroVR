using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using Scriptables;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Manager
{
    /// <summary>
    /// Manages the User Interface (UI) elements and interactions within the game.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        private MenuNavigator MenuNavigator;
        
        [Header("Main Menu")]
        [SerializeField] private GameObject TogglePrefab;
        [SerializeField] private ToggleGroup SinglePlayerLevelToggleGroup;
        [SerializeField] private TMP_Text SinglePlayerLevelName;
        [SerializeField] private TMP_Text SinglePlayerLevelInfo;
        [SerializeField] private List<TMP_Text> SinglePlayerHighScoreInfo;
        [SerializeField] private ToggleGroup MultiPlayerLevelToggleGroup;
        [SerializeField] private TMP_Text MultiPlayerLevelName;
        [SerializeField] private TMP_Text MultiPlayerLevelInfo;
        
        [Header("Settings Menu")]
        [SerializeField] private TMP_Dropdown WebcamDropdown;
        [SerializeField] private Slider MusicSlider;
        [SerializeField] private Slider EffectsSlider;

        [Header("InGame UI")]
        [SerializeField] private GameObject InGameScoreUIPrefab;
        [SerializeField] private GameObject InGameScoreUI;
        private readonly List<InGameScoreUI> _inGameScoreUIs = new();

        [Header("Endscreen UI")]
        [SerializeField] private GameObject EndscreenUIPrefab;
        [SerializeField] private GameObject EndscreenUI;
        private readonly List<EndscreenUI> _endscreenUIs = new();

        private List<ScriptableLevel> _levels;
        private ScriptableLevel _selectedLevel;
        
        // Holds the data for the current level.
        private LevelStruct _level;

        private void Awake()
        {
            EventManager.OnLevelStarted += LevelStartedCallback;
            
            CreateSinglePlayerLevelList();
            CreateMultiPlayerLevelList();
            CreateWebcamDropdown();
            SetInitialSettingValues();

            MenuNavigator = GetComponent<MenuNavigator>();
        }

        private void OnDestroy()
        {
            EventManager.OnLevelStarted -= LevelStartedCallback;
        }

        private void SetInitialSettingValues()
        {
            MusicSlider.value = AudioManager.Instance.MusicVolume;
            EffectsSlider.value = AudioManager.Instance.EffectsVolume;
        }

        private void LevelStartedCallback()
        {
            InitiateInGameUI();
            InitiateEndscreenUI();
        }

        private void InitiateEndscreenUI()
        {
            int playerCount = GameManager.Instance.PlayerCount;

            
            // Adjust the number of EndscreenUI instances to match the current player count
            for (int i = _endscreenUIs.Count; i < playerCount; i++)
            {
                EndscreenUI newEndscreenUI = Instantiate(EndscreenUIPrefab, EndscreenUI.transform).GetComponent<EndscreenUI>();
                newEndscreenUI.SetPlayerId(i);
                _endscreenUIs.Add(newEndscreenUI);
            }

            // Activate or deactivate EndscreenUIs based on current player count.
            for (int i = 0; i < _endscreenUIs.Count; i++)
            {
                _endscreenUIs[i].gameObject.SetActive(i < playerCount);
            }
        }

        private void InitiateInGameUI()
        {
            int playerCount = GameManager.Instance.PlayerCount;

            
            // Adjust the number of InGameScoreUI instances to match the current player count
            for (int i = _inGameScoreUIs.Count; i < playerCount; i++)
            {
                InGameScoreUI newInGameScoreUI = Instantiate(InGameScoreUIPrefab, InGameScoreUI.transform).GetComponent<InGameScoreUI>();
                newInGameScoreUI.SetPlayerId(i);
                _inGameScoreUIs.Add(newInGameScoreUI);
            }

            // Activate or deactivate ScoreUIs based on current player count.
            for (int i = 0; i < _inGameScoreUIs.Count; i++)
            {
                _inGameScoreUIs[i].gameObject.SetActive(i < playerCount);
                if (playerCount > 2)
                    _inGameScoreUIs[i].SetLayoutVertical();
                else
                    _inGameScoreUIs[i].SetLayoutHorizontal();
            }
        }

        private void CreateWebcamDropdown()
        {
            if (WebCamTexture.devices.Length == 0) {
                Debug.Log("No webcam devices found.");
                return;
            }
            
            // Clear any existing options
            WebcamDropdown.options.Clear();
    
            foreach(WebCamDevice device in WebCamTexture.devices) {
                WebcamDropdown.options.Add(new TMP_Dropdown.OptionData(device.name));
            }

            WebcamDropdown.RefreshShownValue();
        }
        
        private void CreateSinglePlayerLevelList()
        {
            _levels =  Resources.LoadAll<ScriptableLevel>("Levels").ToList();
            bool first = true;
    
            foreach (ScriptableLevel level in _levels)
            {
                GameObject toggleObject = Instantiate(TogglePrefab, SinglePlayerLevelToggleGroup.transform);
                Toggle toggle = toggleObject.GetComponent<Toggle>();
                toggle.onValueChanged.AddListener(delegate {OnLevelSelected(toggle, level); });
                toggle.group = SinglePlayerLevelToggleGroup;
                toggleObject.GetComponentInChildren<TMP_Text>().text = level.LevelStruct.LevelName;
                if (first)
                {
                    toggle.gameObject.AddComponent<ToggleSelect>();
                    OnLevelSelected(toggle, level);
                    first = false;
                }
            }
        }
        
        private void CreateMultiPlayerLevelList()
        {
            _levels =  Resources.LoadAll<ScriptableLevel>("Levels").ToList();
    
            foreach (ScriptableLevel level in _levels)
            {
                GameObject toggleObject = Instantiate(TogglePrefab, MultiPlayerLevelToggleGroup.transform);
                Toggle toggle = toggleObject.GetComponent<Toggle>();
                toggle.onValueChanged.AddListener(delegate {OnLevelSelected(toggle, level); });
                toggle.group = MultiPlayerLevelToggleGroup;
                toggleObject.GetComponentInChildren<TMP_Text>().text = level.LevelStruct.LevelName;
            }
        }

        // Methods to handle button presses, triggering corresponding actions in the GameManager.
        public void OnPreStartGameButtonPressed(int playerCount)
        {
            SetPlayerCount(playerCount);
            MenuNavigator.OnPreviewWebcamButtonPressed();
        }

        // Methods to handle button presses, triggering corresponding actions in the GameManager.
        public void OnStartGameButtonPressed()
        {
            StartGame();
            EventManager.InvokeUIClicked();
        }

        // Methods to handle button presses, triggering corresponding actions in the GameManager.
        public void OnRestartGameButtonPressed()
        {
            SetPlayerCount(GameManager.Instance.PlayerCount);
            StartGame();
            EventManager.InvokeUIClicked();
        }

        private static void SetPlayerCount(int playerCount)
        {
            GameManager.Instance.PlayerCount = playerCount;
        }

        private void StartGame()
        {
            if (_selectedLevel == null)
                return;
            GameManager.Instance.StartGame(_selectedLevel);
        }

        public void OnFrontButtonPressed() => GameManager.Instance.OnFrontButtonPressed();

        private void OnLevelSelected(Toggle toggle, ScriptableLevel level)
        {
            if (!toggle.isOn)
                return;
            _selectedLevel = level;
            LevelStruct levelStruct = level.LevelStruct;
            string text = //$"{levelStruct.LevelName}\n\n" +
                          $"Mode: {levelStruct.LevelMode}\n" +
                          $"Speed: {levelStruct.MovementSpeed * 10}\n";
            
            text += levelStruct.LevelMode switch
            {
                ELevelMode.Training => $"Length: \u221e",
                ELevelMode.Endless => $"Length: \u221e",
                ELevelMode.Predefined => $"Length: {levelStruct.EmoteArray.Length / levelStruct.SpawnInterval}",
                _ => $"Length: {levelStruct.Count / levelStruct.SpawnInterval}",
            };
            SinglePlayerLevelName.text = level.name;
            SinglePlayerLevelInfo.text = text;
            MultiPlayerLevelName.text = level.name;
            MultiPlayerLevelInfo.text = text;

            CreateHighScoreUI(level);
            
            EventManager.InvokeUIClicked();
        }

        private void CreateHighScoreUI(ScriptableLevel level)
        {
            if (SinglePlayerHighScoreInfo.Count != 3)
                return;
            
            List<HighScore> highScores = level.GetHighScores();

            string[] text = new string[3];

            text[0] = "High scores:\nPlayer";
            text[1] = "\nScore";
            text[2] = "\nEmojis";
            
            for (int i = 0; i < Math.Min(highScores.Count, 10); i++)
            {
                text[0] += $"\n{highScores[i].UserID}: ";
                text[1] += $"\n{highScores[i].LevelScore}";
                text[2] += $"\n({highScores[i].MatchedEmojis}\\{highScores[i].TotalEmotes})";
            }

            SinglePlayerHighScoreInfo[0].text = text[0];
            SinglePlayerHighScoreInfo[1].text = text[1];
            SinglePlayerHighScoreInfo[2].text = text[2];
        }

        public void OnWebcamSelected(TMP_Dropdown change)
        {
            WebcamManager.SetupWebcam(WebCamTexture.devices[change.value]);
            
            EventManager.InvokeUIClicked();
        }

        public void OnMusicVolumeChanged(Slider slider)
        {
            AudioManager.Instance.MusicVolume = slider.value;
            
            EventManager.InvokeUIClicked();
        }

        public void OnEffectVolumeChanged(Slider slider)
        {
            AudioManager.Instance.EffectsVolume = slider.value;
            
            EventManager.InvokeUIClicked();
        }
    }
}