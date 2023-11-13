using Enums;
using Utilities;

namespace Data
{
    /// <summary>
    /// Represents the data to be logged for a singe FER call.
    /// </summary>
    public class LogData
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
        /// Gets or sets the emote with the highest probability from the facial expression recognition system.
        /// </summary>
        public EEmote EmoteFer;
     
        /// <summary>
        /// Gets or sets the probabilities of all emotions from the facial expression recognition system.
        /// </summary>
        public Probabilities FerProbabilities;
     
        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public string UserID;
     
        /// <summary>
        /// Gets or sets the Face Expression data
        /// </summary>
        public string FaceExpressions;
    }
}