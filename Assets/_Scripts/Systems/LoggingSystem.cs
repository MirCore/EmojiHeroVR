using System;
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

        private readonly Dictionary<string, byte[]> _currentImages = new();


        private void Start()
        {
            // Create a unique log file name based on the current date and time
            _fileName = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + "_EmojiHero-Log.csv";
            
            string userID = EditorUI.EditorUI.Instance.UserID;
            
            // Combine the specified path with the application's data path
            _dirPath = Path.Combine(Application.dataPath + RelativePath, userID);
            
            CreateCsvHeaders();
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
            string imageFilename = SaveFiles.SaveImageFile(_dirPath, timestamp, _currentImages[timestamp]);
            _currentImages.Remove(timestamp);
            
            // Prepare the data to be logged
            string[] data =
            {
                EditorUI.EditorUI.Instance.UserID,
                timestamp, // Timestamp
                GameManager.Instance.Level.name, // Level name
                GameManager.Instance.GetLevelEmojiProgress().ToString(), // Number of emoji
                emote.ToString(), // Emote in ActionArea
                imageFilename, // Name of image file(s)
                maxEmote.ToString(),
                response[maxEmote].ToString("F2"),
                string.Join(";", response.Select(kv => $"{kv.Key}: {kv.Value:F2}")) // FER response
            };
            
            // Append the data as a line to the log CSV file
            SaveFiles.AppendLineToCsv(_dirPath, _fileName, data);
        }

        public void SetImageData(byte[] bytes, string timestamp)
        {
            _currentImages.Add(timestamp, bytes);
        }
    }
}
