using System;
using Enums;
using UnityEngine;

namespace Manager
{
    public class EventManager : MonoBehaviour
    {
        /// <summary>Triggered when an emote enters the ActionArea.</summary>
        public static event Action<EEmote> OnEmoteEnteredArea;
        /// <summary>Invokes the OnEmoteEnteredArea event.</summary>
        /// <param name="emote">The emote that entered the area.</param>
        public static void InvokeEmoteEnteredArea(EEmote emote) => OnEmoteEnteredArea?.Invoke(emote);

        
        /// <summary>Triggered when an emotion is detected.</summary>
        public static event Action<EEmote> OnEmotionDetected;
        /// <summary>Invokes the OnEmotionDetected event.</summary>
        /// <param name="emote">The detected emotion.</param>
        public static void InvokeEmotionDetected(EEmote emote) => OnEmotionDetected?.Invoke(emote);

        
        /// <summary>Triggered when an emote was correctly reenacted, along with a score indicating the probability.</summary>
        public static event Action<EEmote, float> OnEmoteFulfilled;
        /// <summary>Invokes the OnEmoteFulfilled event.</summary>
        /// <param name="emote">The fulfilled emote.</param>
        /// <param name="score">The score indicating the probability of the emote.</param>
        public static void InvokeEmoteFulfilled(EEmote emote, float score) => OnEmoteFulfilled?.Invoke(emote, score);

        
        /// <summary>Triggered when an emote exits the ActionArea.</summary>
        public static event Action<EEmote> OnEmoteExitedArea;
        /// <summary>Invokes the OnEmoteExitedArea event.</summary>
        /// <param name="emote">The emote that exited the area.</param>
        public static void InvokeEmoteExitedArea(EEmote emote) => OnEmoteExitedArea?.Invoke(emote);

        
        /// <summary>Triggered when an emote fails to be reenacted.</summary>
        public static event Action<EEmote> OnEmoteFailed;
        /// <summary>Invokes the OnEmoteFailed event.</summary>
        /// <param name="emote">The emote that failed to be reenacted.</param>
        public static void InvokeEmoteFailed(EEmote emote) => OnEmoteFailed?.Invoke(emote);

        
        /// <summary>Triggered when the level starts.</summary>
        public static event Action OnLevelStarted;
        /// <summary>Invokes the OnLevelStarted event.</summary>
        public static void InvokeLevelStarted() => OnLevelStarted?.Invoke();

        
        /// <summary>Triggered when the level stopped.</summary>
        public static event Action OnLevelStopped;
        /// <summary>Invokes the OnLevelStopped event.</summary>
        public static void InvokeLevelStopped() => OnLevelStopped?.Invoke();

        
        /// <summary>Triggered when the level finishes.</summary>
        public static event Action OnLevelFinished;
        /// <summary>Invokes the OnLevelFinished event.</summary>
        public static void InvokeLevelFinished() => OnLevelFinished?.Invoke();
    }
}
