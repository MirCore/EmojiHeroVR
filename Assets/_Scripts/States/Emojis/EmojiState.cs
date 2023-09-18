using Enums;
using Manager;

namespace States.Emojis
{
    public abstract class EmojiState
    {
        public abstract void EnterState(EmojiManager emojiManager);

        public abstract void Update(EmojiManager emojiManager);

        public abstract void OnTriggerEnter(EmojiManager emojiManager);
    
        public abstract void OnTriggerExit(EmojiManager emojiManager);

        public abstract void OnEmotionDetectedCallback(EmojiManager emojiManager, EEmote emote);

    }
}
