using System;
using Enums;
using Manager;
using UnityEngine;

namespace States.Emojis
{
    /// <summary>
    /// Represents the state of an Emoji when it is in the ActionArea.
    /// </summary>
    public class EmojiIntraState : EmojiState
    {
        /// <summary>
        /// Called when the Emoji enters the Intra State.
        /// Initializes necessary variables and triggers relevant events.
        /// </summary>
        /// <param name="emojiManager">The manager controlling the Emoji.</param>
        public override void EnterState(EmojiManager emojiManager)
        {
            // Notify other systems, mainly the FER Handler that an Emoji has entered the Action Area.
            EventManager.InvokeEmoteEnteredActionArea(emojiManager.Emoji);
            
            // Calculate the time the Emoji has left in the Action Area based on the movement speed and area size. Used for the score.
            emojiManager.SpawnTime = DateTime.Now;
        }

        public override void Update(EmojiManager emojiManager)
        {
            
        }

        public override void OnTriggerEnter(Collider collider, EmojiManager emojiManager)
        {
            if (collider.CompareTag("WebcamArea"))
                EventManager.InvokeEmoteEnteredWebcamArea(emojiManager.Emoji);
        }

        public override void OnTriggerExit(Collider collider, EmojiManager emojiManager)
        {
            if (collider.CompareTag("ActionArea"))
                // If the Emoji exits the Action Area without successful reenactment, switch to FailedState.
                emojiManager.SwitchState(emojiManager.FailedState);
            else if (collider.CompareTag("WebcamArea"))
                EventManager.InvokeEmoteExitedWebcamArea(emojiManager.Emoji);
        }

        public override void OnEmotionDetectedCallback(EmojiManager emojiManager, DetectedFace face)
        {
            // If the detected emotion matches the Emoji's emotion, switch to FulfilledState.
            if (face.Emote != emojiManager.Emoji.Emote)
                return;
            
            if (GameManager.Instance.PlayerCount == 1 || face.PlayerId == emojiManager.PlayerId)
                emojiManager.SwitchState(emojiManager.FulfilledState);
        }

        public override void Despawn(EmojiManager emojiManager)
        {
            emojiManager.SwitchState(emojiManager.FailedState);
        }
    }
}
