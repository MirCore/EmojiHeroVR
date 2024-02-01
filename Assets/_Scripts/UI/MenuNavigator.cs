using Manager;
using UnityEngine;

namespace UI
{
    public class MenuNavigator : MonoBehaviour
    {
        [SerializeField] private GameObject MainMenu;
        [SerializeField] private GameObject SinglePlayerMenu;
        [SerializeField] private GameObject MultiPlayerMenu;
        [SerializeField] private GameObject SettingsMenu;
        [SerializeField] private GameObject WebcamPreviewMenu;
        [SerializeField] private GameObject CreditsMenu;
        [SerializeField] private GameObject EndscreenMenu;

        private GameObject _currentMenu;
        private GameObject _lastMenu;
        
        private void OnEnable()
        {
            EventManager.OnLevelFinished += LevelFinishedCallback;
        }

        private void OnDestroy()
        {
            EventManager.OnLevelFinished -= LevelFinishedCallback;
        }


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
            EndscreenMenu.SetActive(false);
        }

        public void OnBackButtonPressed()
        {
            if (_currentMenu == WebcamPreviewMenu)
                SwitchMenu(_lastMenu);
            else
                SwitchMenu(MainMenu);
        }

        public void OnContinueEndscreenButtonPressed()
        {
            if (_lastMenu == SinglePlayerMenu)
                SwitchMenu(SinglePlayerMenu);
            else if (_lastMenu == MultiPlayerMenu)
                SwitchMenu(MainMenu);
            else
                SwitchMenu(MainMenu);
        }
        private void LevelFinishedCallback() => SwitchMenu(EndscreenMenu);

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
}