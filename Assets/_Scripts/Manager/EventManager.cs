using Enums;
using UnityEngine;

namespace Manager
{
    public class EventManager : MonoBehaviour
    {
        public delegate void EmoteEnteredArea(EEmote emote);
        public static event EmoteEnteredArea OnEmoteEnteredArea;
        public static void InvokeEmoteEnteredArea(EEmote emote)
        {
            OnEmoteEnteredArea?.Invoke(emote);
        }
    
        public delegate void EmotionDetected(EEmote emote);
        public static event EmotionDetected OnEmotionDetected;
        public static void InvokeEmotionDetected(EEmote emote)
        {
            OnEmotionDetected?.Invoke(emote);
        }
    
        public delegate void EmojiFulfilled(EEmote emote, float score);
        public static event EmojiFulfilled OnEmoteFulfilled;
        public static void InvokeEmojiFulfilled(EEmote emote, float score)
        {
            OnEmoteFulfilled?.Invoke(emote, score);
        }
    
        public delegate void EmoteExitedArea(EEmote emote);
        public static event EmoteExitedArea OnEmoteExitedArea;
        public static void InvokeEmoteExitedArea(EEmote emote)
        {
            OnEmoteExitedArea?.Invoke(emote);
        }
    
        public delegate void EmoteFailed(EEmote emote);
        public static event EmoteFailed OnEmoteFailed;
        public static void InvokeEmoteFailed(EEmote emote)
        {
            OnEmoteFailed?.Invoke(emote);
        }
    
        public delegate void LevelStarted();
        public static event LevelStarted OnLevelStarted;
        public static void InvokeLevelStarted()
        {
            OnLevelStarted?.Invoke();
        }
    
        public delegate void LevelStopped();
        public static event LevelStopped OnLevelStopped;
        public static void InvokeLevelStopped()
        {
            OnLevelStopped?.Invoke();
        }
    
        public delegate void LevelFinished();
        public static event LevelFinished OnLevelFinished;
        public static void InvokeLevelFinished()
        {
            OnLevelFinished?.Invoke();
        }
    }
}
