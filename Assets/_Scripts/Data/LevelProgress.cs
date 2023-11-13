using System.Collections.Generic;
using System.Linq;
using Enums;

namespace Data
{
    public class LevelProgress
    {
        /// <summary>Gets the count of fulfilled emotes.</summary>
        public int FulfilledEmoteCount { get; internal set; }
        
        /// <summary>Gets the count of finished emotes.</summary>
        public int FinishedEmoteCount { get; internal set; }
        
        /// <summary>Gets the count of spawned emotes.</summary>
        public int SpawnedEmotesCount { get; internal set; }
        
        /// <summary>Gets the current level score.</summary>
        public int LevelScore { get; internal set; }

        /// <summary>Gets the list of emotes currently in the action area.</summary>
        private readonly List<EEmote> _emojiInActionArea = new();
    

        ///<summary>
        /// Indicates whether any emotes are in the action area.
        /// </summary>
        public bool EmojisAreInActionArea => _emojiInActionArea.Any();
        
        /// <summary>
        /// Gets the first emote in the action area or the default value.
        /// </summary>
        public EEmote GetEmojiInActionArea => _emojiInActionArea.FirstOrDefault();
        
        /// <summary>
        /// Adds an emote to the action area.
        /// </summary>
        public void AddEmoteToActionArea(EEmote emote) => _emojiInActionArea.Add(emote);

        /// <summary>
        /// Removes an emote from the action area.
        /// </summary>
        public bool RemoveEmoteFromActionArea(EEmote emote) => _emojiInActionArea.Remove(emote);
        
    }
}