using System;
using System.Linq;
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
            if (GameManager.Instance.Level.LevelStruct.LevelMode == ELevelMode.Predefined)
            {
                int i = GameManager.Instance.Level.LevelStruct.EmoteArray[GameManager.Instance.SpawnedEmotesCount];
                if (GameManager.Instance.Level.LevelStruct.Emotes.Any())
                    emojiManager.Emote = GameManager.Instance.Level.LevelStruct.Emotes[i];
                else
                    emojiManager.Emote = (EEmote)(i + 1);
            }
            else
                emojiManager.Emote = (EEmote)Random.Range(1, Enum.GetValues(typeof(EEmote)).Length-1);
            
            emojiManager.EmoteTitle.text = emojiManager.Emote.ToString();
            SetEmojiTextures(emojiManager);
        }

        public override void Update(EmojiManager emojiManager)
        {
            
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
            Texture texture = ResourceSystem.Instance.GetEmoji(emojiManager.Emote).Texture;
            emojiManager.EmojiMaterial.SetTexture(emojiManager.Sprite, texture);
            emojiManager.EmojiMaterial.SetFloat(emojiManager.FailedColorAmount, 0);
            emojiManager.EmojiMaterial.SetFloat(emojiManager.SuccessColorAmount, 0);
            emojiManager.EmojiMaterial.SetFloat(emojiManager.DissolveAmount, 0);
        }
    }
}
