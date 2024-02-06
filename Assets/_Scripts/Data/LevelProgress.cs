using System.Collections.Generic;
using Manager;
using UnityEngine;
using Utilities;

namespace Data
{
    public class LevelProgress
    {
        /// <summary>Gets the count of finished emotes.</summary>
        internal int FinishedEmotes;

        public int FinishedEmoteCount => FinishedEmotes / GameManager.Instance.PlayerCount;

        /// <summary>List of spawned emotes.</summary>
        internal readonly List<Emoji> SpawnedEmotes = new();

        /// <summary>Gets the count of spawned emotes.</summary>
        public int SpawnedEmotesCount => SpawnedEmotes.Count / GameManager.Instance.PlayerCount;
        
        /// <summary>Gets the list of emotes currently in the action area.</summary>
        private readonly List<Emoji> _emojiInActionArea = new();

        private readonly Player[] _players = new Player[GameManager.Instance.PlayerCount];
    
        /// <summary>
        /// Adds an emote to the action area.
        /// </summary>
        public void AddEmoteToActionArea(Emoji emoji) => _emojiInActionArea.Add(emoji);

        /// <summary>
        /// Removes an emote from the action area.
        /// </summary>
        public bool RemoveEmoteFromActionArea(Emoji emoji) => _emojiInActionArea.Remove(emoji);

        public void ClearEmotesInActionAreaList() => _emojiInActionArea.Clear();

        public void OnEmoteFulfilled(int score, int playerId)
        {
            _players[playerId].MatchedEmotes++;
            _players[playerId].Score += score;
        }

        /// <summary>Gets the current level score.</summary>
        public int GetScore(int playerId)
        {
            return _players[playerId].Score;
        }

        /// <summary>Gets the count of fulfilled emotes.</summary>
        public int GetMatchedEmotes(int playerId)
        {
            return _players[playerId].MatchedEmotes;
        }
    }

    public struct Player
    {
        internal int Id;
        internal int MatchedEmotes;
        internal int Score;
    }
}