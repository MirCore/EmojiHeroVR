using Enums;
using Manager;

namespace States.Emojis
{
    public class EmojiLeavingState : EmojiState
    {
        public override void EnterState(EmojiManager emojiManager)
        {
            emojiManager.FadeOut();
        }

        public override void Update(EmojiManager emojiManager)
        {
            
        }

        public override void OnTriggerEnter(EmojiManager emojiManager)
        {
            
        }

        public override void OnTriggerExit(EmojiManager emojiManager)
        {
            
        }

        public override void OnEmotionDetectedCallback(EmojiManager emojiManager, EEmote emote)
        {
            
        }
    }
}