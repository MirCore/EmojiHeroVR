using Manager;
using UnityEngine;

public class MenuHider : MonoBehaviour
{
    [Header("Hide")]
    [SerializeField] private bool HideOnLevelStarted;
    [SerializeField] private bool HideOnLevelStopped;
    [SerializeField] private bool HideOnLevelFinished;
    
    [Header("Show")]
    [SerializeField] private bool ShowOnLevelStarted;
    [SerializeField] private bool ShowOnLevelStopped;
    [SerializeField] private bool ShowOnLevelFinished;

    private GameObject _ui;
    
    private void Awake()
    {
        EventManager.OnLevelStarted += OnLevelStartedCallback;
        EventManager.OnGameStopped += GameStoppedCallback;
        EventManager.OnLevelFinished += OnLevelFinishedCallback;

        _ui = transform.GetChild(0).gameObject;
    }

    private void OnDestroy()
    {
        EventManager.OnLevelStarted -= OnLevelStartedCallback;
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

    private void GameStoppedCallback()
    {
        if (HideOnLevelStopped)
            HideMenu();
        if (ShowOnLevelStopped)
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