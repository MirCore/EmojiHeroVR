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
            EventManager.InvokeEmoteEnteredArea(emojiManager.Emote);
            
            // Calculate the time the Emoji has left in the Action Area based on the movement speed and area size. Used for the score.
            emojiManager.ActiveAreaLeft = emojiManager.ActionAreaSize/GameManager.Instance.Level.LevelStruct.MovementSpeed;
        }

        public override void Update(EmojiManager emojiManager)
        {
            // Decrease the time left for the Emoji in the Action Area, ensuring it doesn't go below 0.
            emojiManager.ActiveAreaLeft = Mathf.Max(emojiManager.ActiveAreaLeft - Time.deltaTime, 0);
        }

        public override void OnTriggerEnter(EmojiManager emojiManager)
        {
            Debug.Log("NotImplementedException");
        }

        public override void OnTriggerExit(EmojiManager emojiManager)
        {
            // If the Emoji exits the Action Area without successful reenactment, switch to FailedState.
            emojiManager.SwitchState(emojiManager.FailedState);
        }

        public override void OnEmotionDetectedCallback(EmojiManager emojiManager, EEmote emote)
        {
            // If the detected emotion matches the Emoji's emotion, switch to FulfilledState.
            if (emote == emojiManager.Emote)
                emojiManager.SwitchState(emojiManager.FulfilledState);
        }
    }
}
