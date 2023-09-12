using Enums;
using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "New Level")]
    public class ScriptableLevel : ScriptableObject
    {
        public ELevelMode LevelMode;
        public int Count;
        public float Time;
    }
}