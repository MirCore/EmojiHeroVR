using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Enums;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "New Level")]
    public class ScriptableLevel : ScriptableObject
    {
        public LevelStruct LevelStruct;
        public bool GenerateNewLevelFile = true;
        private string _filePath;
            
        private void OnEnable()
        {
            _filePath = Path.Combine(Application.dataPath, "LevelJson", name + ".json");
            if (GenerateNewLevelFile)
                GenerateNewLevelSaveFile();
            else
                LoadLevel();
        }

        private void OnValidate()
        {
            if (GenerateNewLevelFile)
                GenerateNewLevelSaveFile();
        }

        private void LoadLevel()
        {
            if (!File.Exists(_filePath))
            {
                Debug.Log("File not found: " + _filePath);
                GenerateNewLevelSaveFile();
                return;
            }
                

            string json = File.ReadAllText(_filePath);

            LevelStruct = JsonUtility.FromJson<LevelStruct>(json);
        }

        private void GenerateNewLevelSaveFile()
        {
            GenerateNewLevelFile = false;
            
            if (LevelStruct.LevelMode == ELevelMode.Predefined)
                LevelStruct.EmoteArray = GenerateRandomOrder(LevelStruct.Emotes.Count);
        
            File.WriteAllText(_filePath, JsonUtility.ToJson(LevelStruct));
        }

        private int[] GenerateRandomOrder(int count)
        {
            Random random = new ();
            
            // Use all emotes if ints is empty
            if (count == 0)
                count = 7;

            // Create a list with each number repeated amount times
            IEnumerable<int> repeatedNumbers = Enumerable.Range(0, count).SelectMany(i => Enumerable.Repeat(i, LevelStruct.EmoteRepeatAmount));
        
            // Shuffle the list to get a random order
            int[] randomNumbers = repeatedNumbers.OrderBy(x => random.Next()).ToArray();

            return randomNumbers;
        }
    }
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