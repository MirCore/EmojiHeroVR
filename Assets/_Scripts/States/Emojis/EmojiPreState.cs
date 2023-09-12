using System;
using Enums;
using Manager;
using Systems;
using UnityEngine;
using Random = UnityEngine.Random;

namespace States.Emojis
{
    public class EmojiPreState : EmojiState
    {
        public override void EnterState(EmojiManager emojiManager)
        {
            emojiManager.Emote = (EEmote)Random.Range(1, Enum.GetValues(typeof(EEmote)).Length-1);
            SetEmojiTextures(emojiManager);
        }

        public override void OnTriggerEnter(EmojiManager emojiManager)
        {
            emojiManager.SwitchState(emojiManager.IntraState);
        }

        public override void OnTriggerExit(EmojiManager emojiManager)
        {
            throw new NotImplementedException();
        }

        public override void OnEmotionDetectedCallback(EmojiManager emojiManager, EEmote emote)
        {
            
        }

        private void SetEmojiTextures(EmojiManager emojiManager)
        {
            emojiManager.EmojiRenderer.material.color = Color.gray;
            Texture texture = ResourceSystem.Instance.GetEmoji(emojiManager.Emote).Texture;
            emojiManager.EmojiRenderer.material.mainTexture = texture;
            emojiManager.EmojiRenderer.material.SetTexture(emojiManager.EmissionMap, texture);
        }
    }
}
