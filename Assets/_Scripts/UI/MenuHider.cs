using Manager;
using UnityEngine;

namespace UI
{
    public class MenuHider : MonoBehaviour
    {
        [Header("ToggleUI")]
        [SerializeField] private bool HideOnLevelStarted;
        [SerializeField] private bool HideOnLevelFinished;
        [SerializeField] private bool HideOnGameStarted;
        [SerializeField] private bool HideOnGameStopped;
    
        [Header("Show")]
        [SerializeField] private bool ShowOnLevelStarted;
        [SerializeField] private bool ShowOnLevelFinished;
        [SerializeField] private bool ShowOnGameStarted;
        [SerializeField] private bool ShowOnGameStopped;

        private GameObject _ui;
    
        private void Awake()
        {
            EventManager.OnLevelStarted += OnLevelStartedCallback;
            EventManager.OnGameStarted += GameStartedCallback;
            EventManager.OnGameStopped += GameStoppedCallback;
            EventManager.OnLevelFinished += OnLevelFinishedCallback;

            _ui = transform.GetChild(0).gameObject;
        
            HideMenu();
        }

        private void OnDestroy()
        {
            EventManager.OnLevelStarted -= OnLevelStartedCallback;
            EventManager.OnGameStarted -= GameStartedCallback;
            EventManager.OnGameStopped -= GameStoppedCallback;
            EventManager.OnLevelFinished -= OnLevelFinishedCallback;
        }

        private void OnLevelStartedCallback()
        {
            if (HideOnLevelStarted)
                HideMenu();
            if (ShowOnLevelStarted)
                ShowMenu();
        }

        private void GameStartedCallback()
        {
            if (HideOnGameStarted)
                HideMenu();
            if (ShowOnGameStarted)
                ShowMenu();
        }

        private void GameStoppedCallback()
        {
            if (HideOnGameStopped)
                HideMenu();
            if (ShowOnGameStopped)
                ShowMenu();
        }

        private void OnLevelFinishedCallback()
        {
            if (HideOnLevelFinished)
                HideMenu();
            if (ShowOnLevelFinished)
                ShowMenu();
        }

        private void HideMenu()
        {
            _ui.SetActive(false);
        }

        private void ShowMenu()
        {
            _ui.SetActive(true);
        }
    }
}