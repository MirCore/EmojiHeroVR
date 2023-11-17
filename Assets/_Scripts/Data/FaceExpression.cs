using System.Collections.Generic;
using Enums;

namespace Data
{
    public class FaceExpression
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
        /// Gets or sets the ID of the Emote in the level sequence.
        /// </summary>
        public int EmoteID;
     
        /// <summary>
        /// Gets or sets the emote to imitate.
        /// </summary>
        public EEmote EmoteEmoji;
     
        /// <summary>
        /// Gets or sets the webcam images.
        /// </summary>
        public string FaceExpressionJson;
    }
}