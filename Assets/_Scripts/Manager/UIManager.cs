using System;
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
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject PreparingUI;
        [SerializeField] private GameObject LevelPlayingUI;
        [SerializeField] private GameObject LevelEndScreenUI;
        [SerializeField] private GameObject LevelPrefab;
        [SerializeField] private GameObject LevelUI;

        [Header("Score UI")]
        [SerializeField] private TMP_Text LevelNameField;
        [SerializeField] private List<TMP_Text> ResultField = new();
        [SerializeField] private List<TMP_Text> ScoreField = new();
        
        private LevelStruct _level;

        private void Awake()
        {
            EventManager.OnLevelStarted += OnLevelStartedCallback;
            EventManager.OnLevelStopped += OnLevelStoppedCallback;
            EventManager.OnLevelFinished += OnOnLevelFinishedCallback;
            EventManager.OnEmoteExitedArea += OnEmoteExitedAreaCallback;

            LoadLevelUI();
            OnLevelStoppedCallback();
        }


        private void OnDisable()
        {
            EventManager.OnLevelStarted -= OnLevelStartedCallback;
            EventManager.OnLevelStopped -= OnLevelStoppedCallback;
            EventManager.OnLevelFinished -= OnOnLevelFinishedCallback;
            EventManager.OnEmoteExitedArea -= OnEmoteExitedAreaCallback;
        }

        private void OnEmoteExitedAreaCallback(EEmote emote)
        {
            LoadScoreUI();
        }

        private void LoadLevelUI()
        {
            Dictionary<ScriptableLevel, GameObject> uis = new ();
            foreach (ScriptableLevel level in ResourceSystem.Instance.GetLevels)
            {
                GameObject ui = Instantiate(LevelPrefab, LevelUI.transform);
                uis.Add(level, ui);
            }

            int id = 0;

            foreach ((ScriptableLevel scriptableLevel, GameObject uiGameObject) in uis)
            {
                TMP_Text[] texts = uiGameObject.GetComponentsInChildren<TMP_Text>();
                Button button = uiGameObject.GetComponent<Button>();
                button.onClick.AddListener(() => OnLevelButtonClicked(scriptableLevel));
                
                texts[0].text = "#" + id;
                texts[1].text = scriptableLevel.LevelStruct.LevelName;

                texts[2].text = scriptableLevel.LevelStruct.LevelMode switch
                {
                    ELevelMode.Training => "<sprite index=1>",
                    ELevelMode.Predefined => scriptableLevel.LevelStruct.EmoteArray.Length + "<sprite index=0>",
                    _ => scriptableLevel.LevelStruct.Count + "<sprite index=0>"
                };

                if (scriptableLevel.LevelStruct.LevelMode == ELevelMode.Training)
                    texts[3].text = "<sprite index=2>";
                else
                    texts[3].text = 60 * scriptableLevel.LevelStruct.MovementSpeed + "<sprite index=0>/m";

                id++;
            }
        }

        private void LoadEndScreenUI()
        {
            LevelNameField.text = _level.LevelName;
            LoadScoreUI();
        }

        private void LoadScoreUI()
        {
            for (int i = 0; i <= 1; i++)
            {
                int emojiProgress = GameManager.Instance.GetLevelEmojiProgress;
                
                ResultField[i].text = _level.LevelMode switch
                {
                    ELevelMode.Training => $"{emojiProgress}",
                    ELevelMode.Predefined => emojiProgress + "/" + _level.EmoteArray.Length,
                    _ => emojiProgress + "/" + _level.Count
                };
                ScoreField[i].text = GameManager.Instance.GetLevelScore.ToString();
            }
        }

        private void OnLevelStartedCallback()
        {
            _level = GameManager.Instance.Level;
            
            PreparingUI.SetActive(false);
            LevelPlayingUI.SetActive(true);
            LevelEndScreenUI.SetActive(false);
            LoadScoreUI();
        }
        
        private void OnLevelStoppedCallback()
        {
            LevelPlayingUI.SetActive(false);
            PreparingUI.SetActive(true);
            LevelEndScreenUI.SetActive(false);
        }
        
        private void OnOnLevelFinishedCallback()
        {
            LevelPlayingUI.SetActive(false);
            PreparingUI.SetActive(false);
            LevelEndScreenUI.SetActive(true);
            LoadEndScreenUI();
        }

        public void OnStartButtonPressed() => GameManager.Instance.OnButtonPressed(UIType.StartLevel);
        public void OnPauseButtonPressed() => GameManager.Instance.OnButtonPressed(UIType.PauseLevel);
        public void OnStopButtonPressed() => GameManager.Instance.OnButtonPressed(UIType.StopLevel);
        public void OnEndScreenButtonPressed() => GameManager.Instance.OnButtonPressed(UIType.ContinueEndScreen);
        private static void OnLevelButtonClicked(ScriptableLevel level)
        {
            GameManager.Instance.OnButtonPressed(level);
        }
        
    }
}

[Serializable]
public enum UIType
{
    Default,
    StartLevel,
    StopLevel,
    PauseLevel,
    ContinueEndScreen,
    StartStopLevel
} 