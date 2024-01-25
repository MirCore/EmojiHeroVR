using System.Collections;
using Enums;
using UnityEngine;

namespace Manager
{
    public class FerManager : MonoBehaviour
    {
        private void OnEnable()
        {
            EventManager.OnLevelStarted += LevelStartedCallback;
        }

        private void OnDestroy()
        {
            EventManager.OnLevelStarted -= LevelStartedCallback;
        }

        private void LevelStartedCallback()
        {
            DetectEmotion();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void DetectEmotion()
        {
            if (!GameManager.Instance.LevelIsPlaying)
                return;
            Color32[] image = WebcamManager.TakeSnapshot();
            EEmote emote = FerService.GetEmotion(image, 0.3f, 0.1f);
    
            if (emote != EEmote.None)
                EventManager.InvokeEmotionDetected(emote);

            StartCoroutine(DetectEmotionNextFrame());
        }

        private IEnumerator DetectEmotionNextFrame()
        {
            yield return null;  // Wait until the next frame to reduce lag
            DetectEmotion();
        }
    }
}
