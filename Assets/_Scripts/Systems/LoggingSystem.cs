using System;
using System.IO;
using Manager;
using UnityEngine;
using Utilities;

namespace Systems
{
    public class LoggingSystem : Singleton<LoggingSystem>
    {
        // Log Setup:
        // timestamp    level    # of emoji    emote in ActionArea    name of image file(s)    FER response?  

        private const string RelativePath = "/../SaveFiles/"; // Relative path for the log file
        private string _fileName; // The name of the log file
        private string _dirPath; // The full directory path where the log file will be stored


        private void Start()
        {
            // Create a unique log file name based on the current date and time
            _fileName = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + "_EmojiHero-Log.csv";
            
            // Combine the specified path with the application's data path
            _dirPath = Path.Combine(Application.dataPath + RelativePath);

            CreateCsvHeaders();
        }
        
        /// <summary>
        /// Creates the header row for the CSV file.
        /// </summary>
        private void CreateCsvHeaders()
        {
            string[] data =
            {
                "Timestamp (Unix)",
                "Level Name",
                "Number of Emoji",
                "Emote in ActionArea",
                "Name of image file",
                "FER response"
            };
            
            // Append the header row to the log CSV file
            SaveFiles.AppendLineToCsv(_dirPath, _fileName, data);
        }

        /// <summary>
        /// Writes a log entry to the CSV file.
        /// </summary>
        public void WriteLog()
        {
            // Prepare the data to be logged
            string[] data =
            {
                ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds().ToString(), // Timestamp
                GameManager.Instance.Level.name, // Level name
                GameManager.Instance.LevelEmojiProgress.ToString(), // Number of emoji
                GameManager.Instance.EmojiInActionArea.ToString(), // Emote in ActionArea
                "[Name of image files(s)]", // Name of image file(s) TODO ADD IMAGE NAME
                "[FER response]" // FER response
            };
            
            // Append the data as a line to the log CSV file
            SaveFiles.AppendLineToCsv(_dirPath, _fileName, data);
        }
    }
}
