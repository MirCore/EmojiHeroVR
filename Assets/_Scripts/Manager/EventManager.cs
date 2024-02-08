using System;
using UnityEngine;
using Utilities;

namespace Manager
{
    public class EventManager : MonoBehaviour
    {
        /// <summary>Triggered when an emote is spawned.</summary>
        public static event Action<Emoji> OnEmoteSpawned;
        /// <summary>Invokes the OnEmoteSpawned event.</summary>
        /// <param name="emoji">The emote that spawned.</param>
        public static void InvokeEmoteSpawned(Emoji emoji) => OnEmoteSpawned?.Invoke(emoji);
        
        /// <summary>Triggered when an emote enters the ActionArea.</summary>
        public static event Action<Emoji> OnEmoteEnteredActionArea;
        /// <summary>Invokes the OnEmoteEnteredActionArea event.</summary>
        /// <param name="emoji">The emote that entered the area.</param>
        public static void InvokeEmoteEnteredActionArea(Emoji emoji) => OnEmoteEnteredActionArea?.Invoke(emoji);

        
        /// <summary>Triggered when an emotion is detected.</summary>
        public static event Action<DetectedFace> OnEmotionDetected;
        /// <summary>Invokes the OnEmotionDetected event.</summary>
        /// <param name="face"></param>
        public static void InvokeEmotionDetected(DetectedFace face)
        {
            OnEmotionDetected?.Invoke(face);
            //Debug.Log("Event: Emotion Detected " + face.Emote + " at pos " + face.PlayerId);
        }


        /// <summary>Triggered when an emote was correctly reenacted, along with a score indicating the probability.</summary>
        public static event Action<Emoji, TimeSpan, int> OnEmoteFulfilled;

        /// <summary>Invokes the OnEmoteFulfilled event.</summary>
        /// <param name="emoji">The fulfilled emote.</param>
        /// <param name="time">The score indicating the probability of the emote.</param>
        /// <param name="playerId"></param>
        public static void InvokeEmoteFulfilled(Emoji emoji, TimeSpan time, int playerId) => OnEmoteFulfilled?.Invoke(emoji, time, playerId);

        
        /// <summary>Triggered when an emote exits the ActionArea.</summary>
        public static event Action<Emoji> OnEmoteExitedActionArea;
        /// <summary>Invokes the OnEmoteExitedActionArea event.</summary>
        /// <param name="emoji">The emote that exited the area.</param>
        public static void InvokeEmoteExitedActionArea(Emoji emoji) => OnEmoteExitedActionArea?.Invoke(emoji);

        
        /// <summary>Triggered when an emote fails to be reenacted.</summary>
        public static event Action<Emoji> OnEmoteFailed;
        /// <summary>Invokes the OnEmoteFailed event.</summary>
        /// <param name="emoji">The emote that failed to be reenacted.</param>
        public static void InvokeEmoteFailed(Emoji emoji) => OnEmoteFailed?.Invoke(emoji);
        
        
        /// <summary>Triggered when an emote enters the WebcamArea.</summary>
        public static event Action<Emoji> OnEmoteEnteredWebcamArea;
        /// <summary>Invokes the OnEmoteEnteredWebcamArea event.</summary>
        /// <param name="emoji">The emote that entered the WebcamArea.</param>
        public static void InvokeEmoteEnteredWebcamArea(Emoji emoji) => OnEmoteEnteredWebcamArea?.Invoke(emoji);
        
        
        /// <summary>Triggered when an emote exits the WebcamArea.</summary>
        public static event Action<Emoji> OnEmoteExitedWebcamArea;
        /// <summary>Invokes the OnEmoteExitedWebcamArea event.</summary>
        /// <param name="emoji">The emote that exited the WebcamArea.</param>
        public static void InvokeEmoteExitedWebcamArea(Emoji emoji) => OnEmoteExitedWebcamArea?.Invoke(emoji);
        
        /// <summary>Triggered when a FER call is triggered.</summary>
        public static event Action OnFerCall;
        /// <summary>Invokes the OnFerCall event.</summary>
        public static void InvokeFerCall() => OnFerCall?.Invoke();
        
        
        //--  Level States  --//
        
        
        /// <summary>Triggered when the game is started.</summary>
        public static event Action OnGameStarted;
        /// <summary>Invokes the OnGameStarted event.</summary>
        public static void InvokeGameStarted() => OnGameStarted?.Invoke();
        
        /// <summary>Triggered when the game stopped.</summary>
        public static event Action OnGameStopped;
        /// <summary>Invokes the OnGameStopped event.</summary>
        public static void InvokeGameStopped() => OnGameStopped?.Invoke();

        /// <summary>Triggered when the level starts.</summary>
        public static event Action OnLevelStarted;
        /// <summary>Invokes the OnLevelStarted event.</summary>
        public static void InvokeLevelStarted() => OnLevelStarted?.Invoke();
        
        /// <summary>Triggered when the level finishes.</summary>
        public static event Action OnLevelFinished;
        /// <summary>Invokes the OnLevelFinished event.</summary>
        public static void InvokeLevelFinished()
        {
            Debug.Log("Event: Level Finished");
            OnLevelFinished?.Invoke();
        }
    }
}
