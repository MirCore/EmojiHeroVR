using System;
using Enums;
using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "New Level")]
    public class ScriptableLevel : ScriptableObject
    {
        [field:SerializeField] public string Name { get; private set; }
        [field:SerializeField] public ELevelMode LevelMode { get; private set; }
        [field:SerializeField] public float EmojiMovementSpeed { get; private set; }
        [field:SerializeField] public float EmojiSpawnInterval { get; private set; }
        [field:SerializeField] public int Count { get; private set; }
        [field:SerializeField] public float Time { get; private set; }

        private void OnEnable()
        {
            
        }
    }
}