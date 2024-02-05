using System.Collections;
using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace Manager
{
    public class FerManager : MonoBehaviour
    {
        public FerManager(int i)
        {
            _i = i;
        }

        private int _i;

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
            StartCoroutine(DetectEmotionNextFrame());
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void DetectEmotions()
        {
            if (!GameManager.LevelIsPlaying)
                return;
            Color32[] image = WebcamManager.TakeSnapshot();
            IEnumerable<DetectedFace> emotions = FerService.GetEmotions(image, 0.3f, 0.1f, GameManager.Instance.PlayerCount);

            foreach (DetectedFace face in emotions)
            {
                if (face == null)
                    continue;
                if (face.Emote != EEmote.None)
                    EventManager.InvokeEmotionDetected(face);
            }

            StartCoroutine(DetectEmotionNextFrame());
        }

        private IEnumerator DetectEmotionNextFrame()
        {
            yield return null;  // Wait until the next frame to reduce lag
            
            DetectEmotions();
        }
    }
}
