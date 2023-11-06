using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace Utilities
{
    /// <summary>
    /// Represents the probabilities of the emotions.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public struct Probabilities
    {
        public float anger;
        public float disgust;
        public float fear;
        public float happiness;
        public float neutral;
        public float sadness;
        public float surprise;
    }

    /// <summary>
    /// Represents the data to be logged for a specific event.
    /// </summary>
    public struct LogData
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

    /// <summary>
    /// Represents the data to be logged for a specific event.
    /// </summary>
    public struct Snapshot
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

    /// <summary>
    /// Represents the settings and data for a game level.
    /// </summary>
    [Serializable]
    public struct LevelStruct
    {
        /// <summary>
        /// Gets or sets the name of the level.
        /// </summary>
        public string LevelName;

        /// <summary>
        /// Gets or sets the game mode of the level.
        /// </summary>
        public ELevelMode LevelMode;

        /// <summary>
        /// Gets or sets the movement speed in the level.
        /// </summary>
        public float MovementSpeed;

        /// <summary>
        /// Gets or sets the interval between spawns in the level.
        /// </summary>
        public float SpawnInterval;

        /// <summary>
        /// Gets or sets the count of Emojis in the level.
        /// </summary>
        public int Count;

        /// <summary>
        /// Gets or sets the predefined emotes for the level.
        /// </summary>
        [Header("Predefined Level Settings")]
        public List<EEmote> Emotes;

        /// <summary>
        /// Gets or sets the amount to repeat each emote in the level.
        /// </summary>
        public int EmoteRepeatAmount;

        /// <summary>
        /// Gets or sets the array of emote IDs for the level.
        /// </summary>
        public int[] EmoteArray;
    }
}