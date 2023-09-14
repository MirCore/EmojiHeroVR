using System.Collections.Generic;
using System.Linq;
using Enums;
using Scriptables;
using UnityEngine;
using Utilities;

namespace Systems
{
    public class ResourceSystem : Singleton<ResourceSystem>
    {
        protected override void Awake()
        {
            base.Awake();
            AssembleEmojiResources();
            AssembleLevelResources();
        }
        
        #region Emoji ScriptableObjects

        private List<ScriptableEmoji> _emojis;
        private Dictionary<EEmote, ScriptableEmoji> _emojiDict;
    
        private void AssembleEmojiResources()
        {
            _emojis = Resources.LoadAll<ScriptableEmoji>("Emojis").ToList();
            _emojiDict = _emojis.ToDictionary(r => r.EEmote, r => r);
        }

        public ScriptableEmoji GetEmoji(EEmote t) => _emojiDict[t];
        
        #endregion
        
        
        #region Level ScriptableObjects

        private List<ScriptableLevel> _levels;
    
        private void AssembleLevelResources()
        {
            _levels = Resources.LoadAll<ScriptableLevel>("Levels").ToList();
        }

        public List<ScriptableLevel> GetLevels => _levels;
        
        #endregion
    }
}
