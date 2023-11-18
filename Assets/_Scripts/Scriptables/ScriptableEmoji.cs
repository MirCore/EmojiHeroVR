using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace Scriptables
{
    public class ScriptableEmoji : ScriptableObject
    {
        public EEmote EEmote;
        public Texture Texture;
        public List<Texture> Textures;
    }
}