using System;
using System.Linq;
using Enums;
using UnityEngine;
using Random = UnityEngine.Random;

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
        throw new NotImplementedException();
    }

    private void SetEmojiTextures(EmojiManager emojiManager)
    {
        emojiManager.EmojiRenderer.material.color = Color.gray;
        Texture texture = (from textureMapping in GameManager.Instance.TextureMappings where textureMapping.EEmote == emojiManager.Emote select textureMapping.Texture).FirstOrDefault();
        emojiManager.EmojiRenderer.material.mainTexture = texture;
        emojiManager.EmojiRenderer.material.SetTexture(emojiManager.EmissionMap, texture);
    }
}
