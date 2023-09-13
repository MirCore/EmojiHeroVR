using System;
using TMPro;
using UnityEngine;

namespace Manager
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject PreparingUI;
        [SerializeField] private GameObject LevelPlayingUI;
        [SerializeField] private GameObject LevelFinishedUI;
        
        private void Awake()
        {
            EventManager.OnLevelStarted += OnLevelStartedCallback;
            EventManager.OnLevelStopped += OnLevelStoppedCallback;
            EventManager.OnLevelFinished += OnOnLevelFinishedCallback;
        }

        private void OnDisable()
        {
            EventManager.OnLevelStarted -= OnLevelStartedCallback;
            EventManager.OnLevelStopped -= OnLevelStoppedCallback;
            EventManager.OnLevelFinished -= OnOnLevelFinishedCallback;
        }

        private void OnLevelStartedCallback()
        {
            PreparingUI.SetActive(false);
            LevelPlayingUI.SetActive(true);
            LevelFinishedUI.SetActive(false);
        }
        
        private void OnLevelStoppedCallback()
        {
            LevelPlayingUI.SetActive(false);
            PreparingUI.SetActive(true);
            LevelFinishedUI.SetActive(false);
        }
        
        private void OnOnLevelFinishedCallback()
        {
            LevelPlayingUI.SetActive(false);
            PreparingUI.SetActive(false);
            LevelFinishedUI.SetActive(true);
        }

        public void OnStartButtonPressed() => GameManager.Instance.OnButtonPressed(UIType.Start);
        public void OnPauseButtonPressed() => GameManager.Instance.OnButtonPressed(UIType.Pause);
        public void OnStopButtonPressed() => GameManager.Instance.OnButtonPressed(UIType.Stop);
        
    }
}

[Serializable]
public enum UIType
{
    Default,
    Start,
    Stop,
    Pause
} 