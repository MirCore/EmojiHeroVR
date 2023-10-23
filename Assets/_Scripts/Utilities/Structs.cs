using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Enums;
using UnityEngine;

namespace Utilities
{
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

    public struct LogData
    {
        public string Timestamp;                // Timestamp (same as timestamp.jpg)
        public string LevelID;                  // Level ID
        public int EmoteID;                     // ID of Emote in Level sequence
        public EEmote EmoteEmoji;               // Emote to imitate
        public EEmote EmoteFer;                 // Emote with highest probability
        public Probabilities FerProbabilities;  // Class containing all probabilities (seperated into columns when logging)
        public string UserID;                   // User ID

        public Texture2D ImageTexture;          // Webcam Image
    }

    [Serializable]
    public struct LevelStruct
    {
        public string LevelName;
        public ELevelMode LevelMode;

        public float MovementSpeed; // 0.2f
        public float SpawnInterval; // 3f
    
        public int Count;
    
        [Header("Predefined Level Settings")]
        public List<EEmote> Emotes;
        public int EmoteRepeatAmount;
    
        public int[] EmoteArray;
    }
}