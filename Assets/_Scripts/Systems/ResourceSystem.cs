using System.Collections.Generic;
using System.Linq;
using Enums;
using Scriptables;
using UnityEngine;
using Utilities;

namespace Systems
{
    /// <summary>
    /// Manages and provides access to game resources emojis and levels.
    /// </summary>
    public class ResourceSystem : Singleton<ResourceSystem>
    {
        /// <summary>
        /// Initializes the ResourceSystem instance and assembles resources for emojis and levels.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            AssembleEmojiResources();
            AssembleLevelResources();
        }
        
        #region Emoji ScriptableObjects

        /// <summary>
        /// Gets a dictionary of emoji textures indexed by their emote type.
        /// </summary>
        public Dictionary<EEmote, Texture> EmojiTextures { get; private set; }
    
        /// <summary>
        /// Loads all ScriptableEmoji assets from the "Emojis" resources folder, and stores them in a list and a dictionary for quick access.
        /// </summary>
        private void AssembleEmojiResources() => EmojiTextures = Resources.LoadAll<ScriptableEmoji>("Emojis").ToDictionary(emoji => emoji.EEmote, emoji => emoji.Texture);
        
        #endregion
        
        
        #region Level ScriptableObjects

        /// <summary>
        /// Gets a list of all loaded levels.
        /// </summary>
        public List<ScriptableLevel> Levels { get; private set; }
        
        /// <summary>
        /// Loads all ScriptableLevel assets from the "Levels" resources folder, and stores them in a list.
        /// </summary>
        private void AssembleLevelResources() => Levels = Resources.LoadAll<ScriptableLevel>("Levels").ToList();

        #endregion
    }
}
