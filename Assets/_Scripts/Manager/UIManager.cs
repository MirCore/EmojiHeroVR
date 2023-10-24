using System.Collections.Generic;
using Enums;
using Scriptables;
using Systems;
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
        [Header("UI GameObjects")]
        [SerializeField] private GameObject PreparingUI;
        [SerializeField] private GameObject LevelPlayingUI;
        [SerializeField] private GameObject LevelEndScreenUI;
        [SerializeField] private GameObject LevelUI;
        
        [SerializeField] private GameObject LevelPrefab;

        [Header("Score UI")]
        [SerializeField] private TMP_Text LevelNameField;
        [SerializeField] private List<TMP_Text> ResultField;
        [SerializeField] private List<TMP_Text> ScoreField;
        
        // Holds the data for the current level.
        private LevelStruct _level;

        private void Awake()
        {
            EventManager.OnLevelStarted += OnLevelStartedCallback;
            EventManager.OnLevelStopped += OnLevelStoppedCallback;
            EventManager.OnLevelFinished += OnLevelFinishedCallback;
            EventManager.OnEmoteExitedArea += OnEmoteExitedAreaCallback;

            // Load the UI elements for level selection.
            LoadLevelUI();
            // Initialize the UI state.
            OnLevelStoppedCallback();
        }

        private void OnDisable()
        {
            EventManager.OnLevelStarted -= OnLevelStartedCallback;
            EventManager.OnLevelStopped -= OnLevelStoppedCallback;
            EventManager.OnLevelFinished -= OnLevelFinishedCallback;
            EventManager.OnEmoteExitedArea -= OnEmoteExitedAreaCallback;
        }

        /// <summary>
        /// Updates the score UI when an emote exits the action area.
        /// </summary>
        private void OnEmoteExitedAreaCallback(EEmote emote) => LoadScoreUI();

        /// <summary>
        /// Instantiates UI elements for each level based on the levels available in the ResourceSystem.
        /// </summary>
        private void LoadLevelUI()
        {
            int id = 0;
            foreach (ScriptableLevel level in ResourceSystem.Instance.Levels)
            {
                GameObject ui = Instantiate(LevelPrefab, LevelUI.transform);
                
                TMP_Text[] texts = ui.GetComponentsInChildren<TMP_Text>();
                Button button = ui.GetComponent<Button>();
                button.onClick.AddListener(() => OnLevelButtonClicked(level));
                
                // Set UI text fields based on level properties.
                texts[0].text = "#" + id;
                texts[1].text = level.LevelStruct.LevelName;

                texts[2].text = level.LevelStruct.LevelMode switch
                {
                    ELevelMode.Training => "<sprite index=1>",
                    ELevelMode.Predefined => level.LevelStruct.EmoteArray.Length + "<sprite index=0>",
                    _ => level.LevelStruct.Count + "<sprite index=0>"
                };

                texts[3].text = level.LevelStruct.LevelMode switch
                {
                    ELevelMode.Training => "<sprite index=2>",
                    _ => 60 * level.LevelStruct.MovementSpeed + "<sprite index=0>/m"
                };

                id++;
            }
        }

        /// <summary>
        /// Loads the UI for the end screen, displaying level name and score.
        /// </summary>
        private void LoadEndScreenUI()
        {
            LevelNameField.text = _level.LevelName;
            LoadScoreUI();
        }

        /// <summary>
        /// Updates the score UI based on the current game progress and score.
        /// </summary>
        private void LoadScoreUI()
        {
            int emojiProgress = GameManager.Instance.GetLevelEmojiProgress;

            foreach (TMP_Text t in ResultField)
            {
                t.text = _level.LevelMode switch
                {
                    ELevelMode.Training => $"{emojiProgress}",
                    ELevelMode.Predefined => emojiProgress + "/" + _level.EmoteArray.Length,
                    _ => emojiProgress + "/" + _level.Count
                };
            }

            foreach (TMP_Text t in ScoreField)
            {
                t.text = GameManager.Instance.GetLevelScore.ToString();
            }
            
        }

        /// <summary>
        /// Updates the UI to reflect the level started state.
        /// </summary>
        private void OnLevelStartedCallback()
        {
            _level = GameManager.Instance.Level;
            
            PreparingUI.SetActive(false);
            LevelPlayingUI.SetActive(true);
            LevelEndScreenUI.SetActive(false);
            LoadScoreUI();
        }
        
        /// <summary>
        /// Updates the UI to reflect the level stopped state.
        /// </summary>
        private void OnLevelStoppedCallback()
        {
            LevelPlayingUI.SetActive(false);
            PreparingUI.SetActive(true);
            LevelEndScreenUI.SetActive(false);
        }
        
        /// <summary>
        /// Updates the UI to reflect the level finished state.
        /// </summary>
        private void OnLevelFinishedCallback()
        {
            LevelPlayingUI.SetActive(false);
            PreparingUI.SetActive(false);
            LevelEndScreenUI.SetActive(true);
            LoadEndScreenUI();
        }

        // Methods to handle button presses, triggering corresponding actions in the GameManager.
        public void OnStartButtonPressed() => GameManager.Instance.OnButtonPressed(UIType.StartLevel);
        public void OnPauseButtonPressed() => GameManager.Instance.OnButtonPressed(UIType.PauseLevel);
        public void OnStopButtonPressed() => GameManager.Instance.OnButtonPressed(UIType.StopLevel);
        public void OnEndScreenButtonPressed() => GameManager.Instance.OnButtonPressed(UIType.ContinueEndScreen);
        private static void OnLevelButtonClicked(ScriptableLevel level) => GameManager.Instance.OnButtonPressed(level);
    }
}