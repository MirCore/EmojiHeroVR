using Enums;
using Manager;
using UnityEngine;

namespace States.Emojis
{
    public class EmojiFailedState : EmojiState
    {
        public override void EnterState(EmojiManager emojiManager)
        {
            emojiManager.EmojiMaterial.SetFloat(emojiManager.FailedColorAmount, 0.5f);
            EventManager.InvokeEmoteExitedArea();
            emojiManager.EmojiAnimator.Play("EmojiFail");
        }

        public override void Update(EmojiManager emojiManager)
        {
            if (emojiManager.transform.position.z < GameManager.Instance.EmojiEndPosition.position.z)
            {
                emojiManager.SwitchState(emojiManager.LeavingState);
            }
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
