using UnityEngine;

public class MenuNavigator : MonoBehaviour
{
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject SinglePlayerMenu;
    [SerializeField] private GameObject MultiPlayerMenu;
    [SerializeField] private GameObject SettingsMenu;
    [SerializeField] private GameObject WebcamPreviewMenu;
    [SerializeField] private GameObject CreditsMenu;

    private GameObject _currentMenu;
    private GameObject _lastMenu;

    private void Start()
    {
        HideMenus();
        SwitchMenu(MainMenu);
    }

    private void SwitchMenu(GameObject menu)
    {
        HideMenus();
        _lastMenu = _currentMenu;
        menu.SetActive(true);
        _currentMenu = menu;
    }

    private void HideMenus()
    {
        MainMenu.SetActive(false);
        SinglePlayerMenu.SetActive(false);
        //MultiPlayerMenu.SetActive(false);
        WebcamPreviewMenu.SetActive(false);
        SettingsMenu.SetActive(false);
        CreditsMenu.SetActive(false);
    }

    public void OnBackButtonPressed()
    {
        if (_currentMenu == WebcamPreviewMenu)
            SwitchMenu(_lastMenu);
        else
            SwitchMenu(MainMenu);
    }

    public void OnSinglePlayerButtonPressed() => SwitchMenu(SinglePlayerMenu);

    public void OnMultiplayerButtonPressed() => SwitchMenu(MultiPlayerMenu);

    public void OnPreviewWebcamButtonPressed() => SwitchMenu(WebcamPreviewMenu);

    public void OnSettingsButtonPressed() => SwitchMenu(SettingsMenu);

    public void OnCreditsButtonPressed() => SwitchMenu(CreditsMenu);

    public void OnExitButtonPressed()
    {
        Application.Quit();
    }
}