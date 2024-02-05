using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using Enums;
using Scriptables;
using TMPro;
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
        [Header("MainMenu")]
        [SerializeField] private GameObject TogglePrefab;
        [SerializeField] private ToggleGroup SinglePlayerLevelToggleGroup;
        [SerializeField] private TMP_Text SinglePlayerLevelInfo;
        [SerializeField] private ToggleGroup MultiPlayerLevelToggleGroup;
        [SerializeField] private TMP_Text MultiPlayerLevelInfo;
        [SerializeField] private TMP_Dropdown WebcamDropdown;

        private List<ScriptableLevel> _levels;
        private ScriptableLevel _selectedLevel;
        
        
        [Header("Score UI")]
        [SerializeField] private List<TMP_Text> LevelNameField;
        [SerializeField] private TMP_Text ProgressField;
        [SerializeField] private List<TMP_Text> ResultField;
        [SerializeField] private List<TMP_Text> ScoreField;
        
        // Holds the data for the current level.
        private LevelStruct _level;
        
        private int _maxScore;

        private void Awake()
        {
            EventManager.OnLevelStarted += OnLevelStartedCallback;
            EventManager.OnGameStopped += GameStoppedCallback;
            EventManager.OnLevelFinished += OnLevelFinishedCallback;
            EventManager.OnEmoteExitedActionArea += EmoteExitedActionAreaCallback;
            EventManager.OnEmoteFulfilled += OnEmoteFulfilledCallback;

            // Load the UI elements for level selection.
            ResetScoreUI();
            // Initialize the UI state.
            GameStoppedCallback();

            CreateSinglePlayerLevelList();
            CreateMultiPlayerLevelList();
            CreateWebcamDropdown();
        }

        private void OnDisable()
        {
            EventManager.OnLevelStarted -= OnLevelStartedCallback;
            EventManager.OnGameStopped -= GameStoppedCallback;
            EventManager.OnLevelFinished -= OnLevelFinishedCallback;
            EventManager.OnEmoteExitedActionArea -= EmoteExitedActionAreaCallback;
            EventManager.OnEmoteFulfilled -= OnEmoteFulfilledCallback;
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
    
            foreach (ScriptableLevel level in _levels)
            {
                GameObject toggleObject = Instantiate(TogglePrefab, SinglePlayerLevelToggleGroup.transform);
                Toggle toggle = toggleObject.GetComponent<Toggle>();
                toggle.onValueChanged.AddListener(delegate {OnLevelSelected(toggle, level); });
                toggle.group = SinglePlayerLevelToggleGroup;
                toggleObject.GetComponentInChildren<TMP_Text>().text = level.LevelStruct.LevelName;
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

        /// <summary>
        /// Updates the score UI when an emote exits the action area.
        /// </summary>
        private void EmoteExitedActionAreaCallback(Emoji emoji) => StartCoroutine(UpdateScoreUI());
        
        /// <summary>
        /// Updates the score UI when an emote is fulfilled.
        /// </summary>
        private void OnEmoteFulfilledCallback(Emoji emoji, TimeSpan time) => StartCoroutine(UpdateScoreUI());

        /// <summary>
        /// Loads the UI for the end screen, displaying level name and score.
        /// </summary>
        private void LoadEndScreenUI() => StartCoroutine(UpdateScoreUI());

        /// <summary>
        /// Loads the score UI with the current game progress and score.
        /// </summary>
        private void LoadScoreUI()
        {
            foreach (TMP_Text t in LevelNameField)
            {
                t.text = _level.LevelName;
            }

            StartCoroutine(UpdateScoreUI());
        }

        /// <summary>
        /// Updates the score UI based on the current game progress and score.
        /// </summary>
        private IEnumerator UpdateScoreUI()
        {
            // Wait to the end of the frame to ensure LevelProgress has been properly updated
            yield return new WaitForEndOfFrame();
            
            LevelProgress levelProgress = GameManager.LevelProgress;

            ProgressField.text = _level.LevelMode switch
            {
                ELevelMode.Training => "",
                ELevelMode.Predefined => $"{Math.Round((float)levelProgress.FinishedEmoteCount / _level.EmoteArray.Length * 100)}%",
                _ => $"{Math.Round((float)levelProgress.FinishedEmoteCount / _level.Count * 100)}%"
            };

            foreach (TMP_Text t in ResultField)
            {
                t.text = _level.LevelMode switch
                {
                    ELevelMode.Training => $"{levelProgress.FulfilledEmoteCount} / {GameManager.LevelProgress.SpawnedEmotesCount}",
                    ELevelMode.Predefined => $"{levelProgress.FulfilledEmoteCount} / {_level.EmoteArray.Length} ({Math.Round((float)levelProgress.FulfilledEmoteCount / _level.EmoteArray.Length * 100 , 1)}%)",
                    _ => $"{levelProgress.FulfilledEmoteCount} / {_level.Count} ({Math.Round((float)levelProgress.FulfilledEmoteCount / _level.EmoteArray.Length * 100 , 1)}%)"
                };
            }

            foreach (TMP_Text t in ScoreField)
            {
                string maxScoreText = "";
                if (_maxScore > 0)
                    maxScoreText = $" / {_maxScore}";
                t.text = $"{levelProgress.LevelScore}{maxScoreText}";
            }

        }

        /// <summary>
        /// Resets the score UI.
        /// </summary>
        private void ResetScoreUI()
        {
            ProgressField.text = "";
            
            foreach (TMP_Text t in LevelNameField)
            {
                t.text = "";
            }

            foreach (TMP_Text t in ResultField)
            {
                t.text = "";
            }

            foreach (TMP_Text t in ScoreField)
            {
                t.text = "";
            }
        }

        /// <summary>
        /// Updates the UI to reflect the level started state.
        /// </summary>
        private void OnLevelStartedCallback()
        {
            _level = GameManager.Instance.Level;

            _maxScore = GameManager.GetMaxScore();
            
            LoadScoreUI();
        }

        /// <summary>
        /// Updates the UI to reflect the level stopped state.
        /// </summary>
        private void GameStoppedCallback()
        {
            ResetScoreUI();
        }
        
        /// <summary>
        /// Updates the UI to reflect the level finished state.
        /// </summary>
        private void OnLevelFinishedCallback()
        {
            LoadEndScreenUI();
        }

        // Methods to handle button presses, triggering corresponding actions in the GameManager.
        public void OnStartGameButtonPressed()
        {
            if (_selectedLevel == null)
                return;
            GameManager.Instance.PlayerCount = 1;
            GameManager.Instance.StartGame(_selectedLevel);
        }

        // Methods to handle button presses, triggering corresponding actions in the GameManager.
        public void OnRestartGameButtonPressed()
        {
            if (_selectedLevel == null)
                return;
            GameManager.Instance.StartGame(_selectedLevel);
        }

        // Methods to handle button presses, triggering corresponding actions in the GameManager.
        public void OnStartMultiplayerGameButtonPressed()
        {
            if (_selectedLevel == null)
                return;
            GameManager.Instance.PlayerCount = 2;
            GameManager.Instance.StartGame(_selectedLevel);
        }

        public void OnFrontButtonPressed() => GameManager.Instance.OnFrontButtonPressed();

        private void OnLevelSelected(Toggle toggle, ScriptableLevel level)
        {
            if (!toggle.isOn)
                return;
            _selectedLevel = level;
            LevelStruct levelStruct = level.LevelStruct;
            string text = $"{levelStruct.LevelName}\n\n" +
                          $"Mode: {levelStruct.LevelMode}\n" +
                          $"Speed: {levelStruct.MovementSpeed}\n" +
                          $"Length: {levelStruct.Count / levelStruct.SpawnInterval}";
            SinglePlayerLevelInfo.text = text;
            MultiPlayerLevelInfo.text = text;
        }
        
        public void OnWebcamSelected(TMP_Dropdown change)
        {
            WebcamManager.SetupWebcam(WebCamTexture.devices[change.value]);
        }
    }
}