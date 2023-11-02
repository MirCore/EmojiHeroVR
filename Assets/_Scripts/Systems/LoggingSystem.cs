using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Manager;
using UnityEngine;
using Utilities;

namespace Systems
{
    /// <summary>
    /// A system for handling logging of game events and images.
    /// </summary>
    public class LoggingSystem : Singleton<LoggingSystem>
    {
        private const string FileName = "labels.csv"; // The name of the log file
        private string _dirPath; // The full directory path where the log file will be stored

        private readonly List<LogData> _logDataList = new(); // A list to store log data temporarily.

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
            // Get the user ID from the EditorUI instance.
            string userID = EditorUI.EditorUI.Instance.UserID;
            
            // Set the directory path where log files and images will be stored.
            _dirPath = Path.Combine(Application.dataPath + "/../SaveFiles/", userID);
            
            // Create the CSV headers for the log file.
            CreateCsvHeaders();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void OnLevelFinishedCallback()
        {
            // Write log data to the CSV file when a level is finished.
            WriteLog();
            
            // Start coroutine to write all images post play.
            StartCoroutine(WriteImages());
        }

        private IEnumerator WriteImages()
        {
            yield return new WaitForSecondsRealtime(1); // Wait for a second to let all systems finish.
            
            // Continue until all log data is processed.
            while (_logDataList.Any())
            {
                // Get the first log data from the list.
                LogData logData = _logDataList.FirstOrDefault();
                
                // Construct the path for saving the image
                string path = Path.Combine(_dirPath, logData.LevelID, logData.EmoteEmoji.ToString());

                for (int i = 0; i < logData.ImageTextures.Count; i++)
                {
                    try
                    {
                        // Encode the image to PNG format.
                        byte[] bytes = logData.ImageTextures[i].EncodeToPNG();

                        // Add webcam index to filename if its not the main webcam (index > 0)
                        string filename = i == 0 ? $"{logData.Timestamp}.png" : $"{logData.Timestamp}-{i}.png";

                        // Save the image file.
                        SaveFiles.SaveImageFile(path, filename, logData, bytes);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to save image for logData with LevelID: {logData.LevelID}. Exception: {ex}");
                    }
                }
                
                // Remove the processed log data from the list.
                _logDataList.Remove(logData);
                
                yield return null; // Wait for the next frame.
            }
        }

        private void WriteLog()
        {
            // Write each log data to the CSV file.
            foreach (LogData logData in _logDataList)
            {
                try
                {
                    WriteLogLine(logData);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to write log for logData with LevelID: {logData.LevelID}. Exception: {ex}");
                }
            }
        }

        /// <summary>
        /// Writes a single line of log data to the CSV file.
        /// </summary>
        /// <param name="logData">The log data to be written.</param>
        private void WriteLogLine(LogData logData)
        {
            // Prepare the data as an array of strings.
            string[] data =
            {
                logData.Timestamp,
                logData.LevelID,
                logData.EmoteID.ToString(),
                logData.EmoteEmoji.ToString(),
                logData.EmoteFer.ToString(),
                logData.FerProbabilities.anger.ToString("F2"),
                logData.FerProbabilities.disgust.ToString("F2"),
                logData.FerProbabilities.fear.ToString("F2"),
                logData.FerProbabilities.happiness.ToString("F2"),
                logData.FerProbabilities.neutral.ToString("F2"),
                logData.FerProbabilities.sadness.ToString("F2"),
                logData.FerProbabilities.surprise.ToString("F2"),
                logData.UserID,
                logData.FaceExpressions,
            };
            
            // Append the data as a line to the log CSV file
            SaveFiles.AppendLineToCsv(_dirPath, FileName, data);
        }

        /// <summary>
        /// Creates and writes the header line for the CSV log file.
        /// </summary>
        private void CreateCsvHeaders()
        {
            // Define the header line.
            string[] data =
            {
                "Timestamp (Unix)",
                "Level ID",
                "Emote ID",
                "Emote todo",
                "Emote FER",
                "Anger",
                "Disgust",
                "Fear",
                "Happiness",
                "Neutral",
                "Sadness",
                "Surprise",
                "User ID",
                "OVR Face Expressions"
            };
            
            // Write the header line to the CSV file.
            SaveFiles.AppendLineToCsv(_dirPath, FileName, data);
        }
        
        /// <summary>
        /// Adds log data to the list for future processing.
        /// </summary>
        /// <param name="logData">The log data to be added.</param>
        public void AddToLogDataList(LogData logData)
        {
            _logDataList.Add(logData);
        }
        
        public static string GetUnixTimestamp()
        {
            return ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds().ToString();
        }
    }
}
