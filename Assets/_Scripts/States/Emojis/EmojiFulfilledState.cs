﻿using Enums;
using Manager;
using UnityEngine;

namespace States.Emojis
{
    public class EmojiFulfilledState : EmojiState
    {
        public override void EnterState(EmojiManager emojiManager)
        {
            emojiManager.EmojiRenderer.material.color = Color.green;
            emojiManager.EmojiAnimator.Play("EmojiSuccess");
            EventManager.InvokeEmojiFulfilled(emojiManager.Emote);
        }

        public override void OnTriggerEnter(EmojiManager emojiManager)
        {
            throw new System.NotImplementedException();
        }

        public override void OnTriggerExit(EmojiManager emojiManager)
        {
            EventManager.InvokeEmoteExitedArea();
        }

        public override void OnEmotionDetectedCallback(EmojiManager emojiManager, EEmote emote)
        {
        
        }
    }
}