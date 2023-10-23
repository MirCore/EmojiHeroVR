﻿using Enums;
using Manager;
using UnityEngine;

namespace States.Emojis
{
    /// <summary>
    /// Represents the state of an Emoji when the player has successfully matched the emotion.
    /// </summary>
    public class EmojiFulfilledState : EmojiState
    {
        /// <summary>
        /// Called when the Emoji enters the Fulfilled State.
        /// Sets the visual feedback and triggers relevant events.
        /// </summary>
        /// <param name="emojiManager">The manager controlling the Emoji.</param>
        public override void EnterState(EmojiManager emojiManager)
        {
            // Set the material property to give visual feedback of success.
            emojiManager.EmojiMaterial.SetFloat(emojiManager.SuccessColorAmount, 0.5f);
            
            // Play the success animation.
            emojiManager.EmojiAnimator.Play("EmojiSuccess");
            
            // Notify other systems, mainly the FER Handler, that the Emoji has been successfully matched.
            EventManager.InvokeEmoteFulfilled(emojiManager.Emote, emojiManager.ActiveAreaLeft);
            
            // If the game is in Training mode, immediately switch to the Leaving State.
            if (GameManager.Instance.Level.LevelStruct.LevelMode == ELevelMode.Training)
                emojiManager.SwitchState(emojiManager.LeavingState);
        }

        public override void Update(EmojiManager emojiManager)
        {
            // Implementation not required for this state.
        }

        public override void OnTriggerEnter(EmojiManager emojiManager)
        {
            Debug.Log("NotImplementedException");
        }

        /// <summary>
        /// This is called when the Emoji exits the Action Area.
        /// Switches the Emoji's state to LeavingState and triggers the event.
        /// </summary>
        /// <param name="emojiManager">The manager controlling the Emoji.</param>
        public override void OnTriggerExit(EmojiManager emojiManager)
        {
            // Switch to the Leaving State when the Emoji exits the Action Area.
            emojiManager.SwitchState(emojiManager.LeavingState);
            
            // Notify other systems that the Emoji has exited the Action Area.
            EventManager.InvokeEmoteExitedArea(emojiManager.Emote);
        }

        public override void OnEmotionDetectedCallback(EmojiManager emojiManager, EEmote emote)
        {
            // Implementation not required for this state.
        }
    }
}
