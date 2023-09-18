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
            emojiManager.ActiveAreaLeft = GameManager.Instance.ActionArea.GetComponent<Renderer>().bounds.size.z/GameManager.Instance.Level.EmojiMovementSpeed;
        }

        public override void Update(EmojiManager emojiManager)
        {
            emojiManager.ActiveAreaLeft -= Time.deltaTime;
        }

        public override void OnTriggerEnter(EmojiManager emojiManager)
        {
            throw new System.NotImplementedException();
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
