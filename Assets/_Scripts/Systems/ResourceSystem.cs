using System.Collections.Generic;
using System.Linq;
using Enums;
using Scriptables;
using UnityEngine;

namespace Systems
{
    /// <summary>
    /// Manages and provides access to game resources emojis.
    /// </summary>
    public class ResourceSystem
    {
        /// <summary>
        /// Initializes the ResourceSystem instance and assembles resources for emojis.
        /// </summary>
        public ResourceSystem()
        {
            AssembleEmojiResources();
            AssembleEmojiListResources();
        }
        

        /// <summary>
        /// Gets a dictionary of emoji textures indexed by their emote type.
        /// </summary>
        public static Dictionary<EEmote, Texture> EmojiTextures { get; private set; }
    
        /// <summary>
        /// Loads all ScriptableEmoji assets from the "Emojis" resources folder, and stores them in a list and a dictionary for quick access.
        /// </summary>
        private static void AssembleEmojiResources()
        {
            EmojiTextures = Resources.LoadAll<ScriptableEmoji>("Emojis")
                .ToDictionary(emoji => emoji.EEmote, emoji => emoji.Texture);
        }

        /// <summary>
        /// Gets a dictionary of emoji textures indexed by their emote type.
        /// </summary>
        public static Dictionary<EEmote, List<Texture>> EmojiTexturesList { get; private set; }
    
        /// <summary>
        /// Loads all ScriptableEmoji assets from the "Emojis" resources folder, and stores them in a list and a dictionary for quick access.
        /// </summary>
        private static void AssembleEmojiListResources()
        {
            EmojiTexturesList = Resources.LoadAll<ScriptableEmoji>("Emojis")
                .ToDictionary(emoji => emoji.EEmote, emoji => emoji.Textures);
        }
    }
}
