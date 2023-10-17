using System;
using Enums;
using Manager;
using UnityEngine;

namespace States.Emojis
{
    public class EmojiIntraState : EmojiState
    {
        public override void EnterState(EmojiManager emojiManager)
        {
            EventManager.InvokeEmoteEnteredArea(emojiManager.Emote);
            emojiManager.ActiveAreaLeft = emojiManager.ActionAreaSize/GameManager.Instance.Level.LevelStruct.MovementSpeed;
        }

        public override void Update(EmojiManager emojiManager)
        {
            emojiManager.ActiveAreaLeft -= Time.deltaTime;
        }

        public override void OnTriggerEnter(EmojiManager emojiManager)
        {
            
        }

        public override void OnTriggerExit(EmojiManager emojiManager)
        {
            emojiManager.SwitchState(emojiManager.FailedState);
        }

        public override void OnEmotionDetectedCallback(EmojiManager emojiManager, EEmote emote)
        {
            if (emote == emojiManager.Emote)
                emojiManager.SwitchState(emojiManager.FulfilledState);
        }
    }
}
