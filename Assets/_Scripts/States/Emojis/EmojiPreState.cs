using System;
using System.Linq;
using Enums;
using Manager;
using Systems;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace States.Emojis
{
    /// <summary>
    /// Represents the initial state of an Emoji when it is first spawned in the game.
    /// </summary>
    public class EmojiPreState : EmojiState
    {
        /// <summary>
        /// Method called when the Emoji enters this state.
        /// </summary>
        /// <param name="emojiManager">The manager controlling the Emoji.</param>
        public override void EnterState(EmojiManager emojiManager)
        {
            // Set the Emoji's Rigidbody to be kinematic and reset its rotation.
            emojiManager.Rigidbody.isKinematic = true;
            emojiManager.transform.rotation = Quaternion.identity;
            
            // Determine and set the current Emoji's emotion.
            GetEmote(emojiManager);
            
            // Set the Emoji's title text based on its emotion.
            emojiManager.EmoteTitle.text = emojiManager.Emote.ToString();
            
            // Initialize the Emoji's visual elements.
            SetEmojiTextures(emojiManager);
        }

        /// <summary>
        /// Determines and sets the current Emoji's emotion.
        /// </summary>
        /// <param name="emojiManager">The manager of the Emoji.</param>
        private static void GetEmote(EmojiManager emojiManager)
        {
            LevelStruct level = GameManager.Instance.Level;
            
            if (level.LevelMode != ELevelMode.Predefined)
            {
                // get random Emote if level is not of type Predefined
                emojiManager.Emote = (EEmote)Random.Range(1, Enum.GetValues(typeof(EEmote)).Length - 1);
            }
            else
            {
                // Get the emotion from the predefined list or enum.
                int emoteIndex = level.EmoteArray[GameManager.Instance.LevelProgress.SpawnedEmotesCount];
                
                if (level.Emotes.Any())
                {
                    // If a custom Emote list is set, use it.
                    emojiManager.Emote = level.Emotes[emoteIndex];
                }
                else
                {
                    // Otherwise, use the EEmote enum.
                    emojiManager.Emote = (EEmote)(emoteIndex + 1);
                }
            }
        }

        public override void Update(EmojiManager emojiManager)
        {
            // Implementation not required for this state.
        }

        /// <summary>
        /// Trigger enter event handler. Switches the Emoji to the IntraState when triggered.
        /// </summary>
        public override void OnTriggerEnter(EmojiManager emojiManager)
        {
            emojiManager.SwitchState(emojiManager.IntraState);
        }

        /// <summary>
        /// Trigger exit event handler. Not used in this state.
        /// </summary>
        public override void OnTriggerExit(EmojiManager emojiManager)
        {
            Debug.Log("NotImplementedException");
        }

        /// <summary>
        /// Emotion detected event handler. Not used in this state.
        /// </summary>
        public override void OnEmotionDetectedCallback(EmojiManager emojiManager, EEmote emote)
        {
            // Implementation not required for this state.
        }

        /// <summary>
        /// Initializes the Emoji's visual elements based on its emotion.
        /// </summary>
        /// <param name="emojiManager">The manager of the Emoji.</param>
        private static void SetEmojiTextures(EmojiManager emojiManager)
        {
            // Get the texture for the current Emoji's emotion.
            Texture texture = ResourceSystem.Instance.EmojiTextures[emojiManager.Emote];
            
            // Set the Emoji's material properties.
            emojiManager.EmojiMaterial.SetTexture(emojiManager.Sprite, texture);
            emojiManager.EmojiMaterial.SetFloat(emojiManager.FailedColorAmount, 0);
            emojiManager.EmojiMaterial.SetFloat(emojiManager.SuccessColorAmount, 0);
            emojiManager.EmojiMaterial.SetFloat(emojiManager.DissolveAmount, 0);
        }
    }
}
