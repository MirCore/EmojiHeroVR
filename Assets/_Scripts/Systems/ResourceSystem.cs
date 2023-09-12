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
        private List<ScriptableEmoji> Emojis { get; set; }
        private Dictionary<EEmote, ScriptableEmoji> _emojiDict;
    
        protected override void Awake()
        {
            base.Awake();
            AssembleResources();
        }

        private void AssembleResources()
        {
            Emojis = Resources.LoadAll<ScriptableEmoji>("Emojis").ToList();
            _emojiDict = Emojis.ToDictionary(r => r.EEmote, r => r);
        }

        public ScriptableEmoji GetEmoji(EEmote t) => _emojiDict[t];
    }
}
