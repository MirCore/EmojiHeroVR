using Enums;
using Manager;
using UnityEngine;

namespace States.Emojis
{
    public class EmojiFulfilledState : EmojiState
    {
        public override void EnterState(EmojiManager emojiManager)
        {
            emojiManager.EmojiMaterial.SetFloat(emojiManager.SuccessColorAmount, 0.5f);
            emojiManager.EmojiAnimator.Play("EmojiSuccess");
            EventManager.InvokeEmojiFulfilled(emojiManager.Emote, emojiManager.ActiveAreaLeft);
            
            if (GameManager.Instance.Level.LevelStruct.LevelMode == ELevelMode.Training)
                emojiManager.SwitchState(emojiManager.LeavingState);
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
            emojiManager.SwitchState(emojiManager.LeavingState);
            EventManager.InvokeEmoteExitedArea(emojiManager.Emote);
        }

        public override void OnEmotionDetectedCallback(EmojiManager emojiManager, EEmote emote)
        {
        
        }
    }
}
