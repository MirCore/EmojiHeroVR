using System;
using System.Collections.Generic;
using Scriptables;
using Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject PreparingUI;
        [SerializeField] private GameObject LevelPlayingUI;
        [SerializeField] private GameObject LevelEndScreenUI;
        [SerializeField] private GameObject LevelPrefab;
        [SerializeField] private GameObject LevelUI;

        [Header("Endgame UI")]
        [SerializeField] private TMP_Text LevelNameField;
        [SerializeField] private TMP_Text ResultField;
        [SerializeField] private TMP_Text ScoreField;
        
        private void Awake()
        {
            EventManager.OnLevelStarted += OnLevelStartedCallback;
            EventManager.OnLevelStopped += OnLevelStoppedCallback;
            EventManager.OnLevelFinished += OnOnLevelFinishedCallback;

            LoadLevelUI();
        }


        private void OnDisable()
        {
            EventManager.OnLevelStarted -= OnLevelStartedCallback;
            EventManager.OnLevelStopped -= OnLevelStoppedCallback;
            EventManager.OnLevelFinished -= OnOnLevelFinishedCallback;
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
                texts[1].text = scriptableLevel.Name;
                texts[2].text = scriptableLevel.Count + "<sprite index=0>";
                texts[3].text = 60 / scriptableLevel.EmojiMovementSpeed + "<sprite index=0>/m";

                id++;
            }
        }

        private void LoadEndScreenUI()
        {
            LevelNameField.text = GameManager.Instance.Level.Name;
            ResultField.text = GameManager.Instance.LevelEmojiProgress + "/" + GameManager.Instance.Level.Count;
            ScoreField.text = GameManager.Instance.LevelScore.ToString();
        }


        private void OnLevelStartedCallback()
        {
            PreparingUI.SetActive(false);
            LevelPlayingUI.SetActive(true);
            LevelEndScreenUI.SetActive(false);
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
    ContinueEndScreen
} 