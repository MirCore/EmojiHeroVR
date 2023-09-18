using Enums;
using Manager;
using UnityEngine;

namespace States.Emojis
{
    public class EmojiFailedState : EmojiState
    {
        public override void EnterState(EmojiManager emojiManager)
        {
            emojiManager.EmojiRenderer.material.color = Color.red;
            EventManager.InvokeEmoteExitedArea();
            emojiManager.EmojiAnimator.Play("EmojiFail");
        }

        public override void Update(EmojiManager emojiManager)
        {
            
        }

        public override void OnTriggerEnter(EmojiManager emojiManager)
        {
            throw new System.NotImplementedException();
        }

        public override void OnTriggerExit(EmojiManager emojiManager)
        {
        
        }

        public override void OnEmotionDetectedCallback(EmojiManager emojiManager, EEmote emote)
        {
        
        }
    }
}
