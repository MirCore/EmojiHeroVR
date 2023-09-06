using Enums;
using UnityEngine;

public class EmojiFulfilledState : EmojiState
{
    public override void EnterState(EmojiManager emojiManager)
    {
        emojiManager.EmojiRenderer.material.color = Color.green;
        emojiManager.EmojiAnimator.Play("EmojiSuccess");
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
