using UnityEngine;

namespace Manager
{
    public class LightManager : MonoBehaviour
    {
        [SerializeField] private Light FillLight;
        private float _fillLightIntensity;

        private void OnEnable()
        {
            _fillLightIntensity = FillLight.intensity;
            GameStartedCallback();
            
            EventManager.OnGameStarted += GameStartedCallback;
            EventManager.OnLevelFinished += LevelFinishedCallback;
        }
    
        private void OnDestroy()
        {
            EventManager.OnGameStarted -= GameStartedCallback;
            EventManager.OnLevelFinished += LevelFinishedCallback;
        }

        private void GameStartedCallback()
        {
            FillLight.intensity = _fillLightIntensity;
        }

        private void LevelFinishedCallback()
        {
            FillLight.intensity = _fillLightIntensity / 2;
        }
    }
}
