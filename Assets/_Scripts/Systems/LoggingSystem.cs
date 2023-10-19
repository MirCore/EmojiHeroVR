using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Enums;
using Manager;
using UnityEngine;
using Utilities;

namespace Systems
{
    public class LoggingSystem : Singleton<LoggingSystem>
    {
        // Log Setup:
        // userID   timestamp    level    # of emoji    emote in ActionArea    name of image file(s)    FER response?  

        private const string RelativePath = "/../SaveFiles/"; // Relative path for the log file
        private string _fileName; // The name of the log file
        private string _dirPath; // The full directory path where the log file will be stored

        private List<LogData> _logDataList = new();

        private void OnEnable()
        {
            EventManager.OnLevelFinished += OnLevelFinishedCallback;
        }
        private void OnDisable()
        {
            EventManager.OnLevelFinished -= OnLevelFinishedCallback;
        }

        private void Start()
        {
            // Create a unique log file name based on the current date and time
            _fileName = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + "_EmojiHero-Log.csv";
            
            string userID = EditorUI.EditorUI.Instance.UserID;
            
            // Combine the specified path with the application's data path
            _dirPath = Path.Combine(Application.dataPath + RelativePath, userID);
            
            CreateCsvHeaders();
        }

        private void OnLevelFinishedCallback()
        {
            StartCoroutine(WriteImages());
        }

        private IEnumerator WriteImages()
        {
            while (_logDataList.Any())
            {
                LogData logData = _logDataList.FirstOrDefault();
                byte[] bytes = logData.ImageTexture.EncodeToPNG();
                yield return null;
                SaveFiles.SaveImageFile(_dirPath, logData, bytes);
                _logDataList.Remove(logData);
                yield return null;
            }
        }

        /// <summary>
        /// Creates the header row for the CSV file.
        /// </summary>
        private void CreateCsvHeaders()
        {
            string[] data =
            {
                "User ID",
                "Timestamp (Unix)",
                "Level Name",
                "Number of Emoji",
                "Emote in ActionArea",
                "Name of image file",
                "Emote with highest probability",
                "Highest probability",
                "Full FER response"
            };
            
            // Append the header row to the log CSV file
            SaveFiles.AppendLineToCsv(_dirPath, _fileName, data);
        }

        /// <summary>
        /// Writes a log entry to the CSV file.
        /// </summary>
        public void WriteLog(EEmote maxEmote, Dictionary<EEmote, float>  response, string timestamp, EEmote emote)
        {
            // Prepare the data to be logged
            string[] data =
            {
                EditorUI.EditorUI.Instance.UserID,
                timestamp, // Timestamp
                GameManager.Instance.Level.name, // Level name
                GameManager.Instance.GetLevelEmojiProgress().ToString(), // Number of emoji
                emote.ToString(), // Emote in ActionArea
                "imageFilename", // Name of image file(s)
                maxEmote.ToString(),
                response[maxEmote].ToString("F2"),
                string.Join(";", response.Select(kv => $"{kv.Key}: {kv.Value:F2}")) // FER response
            };
            
            // Append the data as a line to the log CSV file
            SaveFiles.AppendLineToCsv(_dirPath, _fileName, data);
        }

        public void AddToLogDataList(LogData logData)
        {
            _logDataList.Add(logData);
        }
    }
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
