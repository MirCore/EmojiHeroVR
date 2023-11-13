using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// Represents the data to be logged for a single webcam snapshot.
    /// </summary>
    public class Snapshot
    {
        /// <summary>
        /// Gets or sets the timestamp (same as timestamp.png)
        /// </summary>
        public string Timestamp;
     
        /// <summary>
        /// Gets or sets the level ID.
        /// </summary>
        public string LevelID;
     
        /// <summary>
        /// Gets or sets the emote to imitate.
        /// </summary>
        public EEmote EmoteEmoji;
     
        /// <summary>
        /// Gets or sets the webcam images.
        /// </summary>
        public List<Color32[]> ImageTextures;
    }
}