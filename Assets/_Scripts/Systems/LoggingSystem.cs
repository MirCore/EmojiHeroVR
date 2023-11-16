using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using Enums;
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
        [SerializeField] internal bool LogTrainingLevel;
        
        private const string CsvFileName = "labels.csv"; // The name of the log file
        private string _dirPathWithUserID; // The full directory path where the log file will be stored

        private readonly List<LogData> _logDataList = new(); // A list to store log data temporarily.
        private readonly List<Snapshot> _snapshots = new(); // A list to store log data temporarily.

        private void OnEnable()
        {
            EventManager.OnLevelFinished += OnLevelFinishedCallback;
        }
        private void OnDisable()
        {
            EventManager.OnLevelFinished -= OnLevelFinishedCallback;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void OnLevelFinishedCallback()
        {
            if (_dirPathWithUserID == null)
            {
                // Get the user ID from the EditorUI instance.
                string userID = EditorUI.EditorUI.Instance.UserID;
                
                // Set the directory path where log files and images will be stored.
                _dirPathWithUserID = Path.Combine(Application.dataPath + "/../SaveFiles/", userID);

                // Create the CSV headers for the log file.
                CreateCsvHeaders();
            }
            
            // Write log data to the CSV file when a level is finished.
            WriteLog();
            
            // Start coroutine to write all images post play.
            StartCoroutine(WriteImages());
        }

        private IEnumerator WriteImages()
        {
            yield return new WaitForSecondsRealtime(1); // Wait for a second to let all systems finish.
            
            
            Texture2D texture = new (WebcamManager.WebcamWidth, WebcamManager.WebcamHeight);
            
            // Continue until all snapshots is processed.
            while (_snapshots.Any())
            {
                // Get the first snapshot from the list.
                Snapshot snapshot = _snapshots.FirstOrDefault();
                
                if (snapshot != null && (LogTrainingLevel || snapshot.LevelMode != ELevelMode.Training))
                {
                    // Construct the path for saving the image
                    string path = Path.Combine(_dirPathWithUserID, snapshot.LevelID, snapshot.EmoteEmoji.ToString());

                    for (int i = 0; i < snapshot.ImageTextures.Count; i++)
                    {
                        try
                        {
                            // Convert Pixels to an image
                            texture.SetPixels32(snapshot.ImageTextures[i]);
                            texture.Apply();

                            // Encode the image to PNG format.
                            byte[] bytes = texture.EncodeToPNG();

                            // Add webcam index to filename if its not the main webcam (index > 0)
                            string filename = i == 0 ? $"{snapshot.Timestamp}.png" : $"{snapshot.Timestamp}-{i}.png";

                            // Save the image file.
                            SaveFiles.SaveImageFile(path, filename, bytes);

                            EditorUI.EditorUI.Instance.UpdateImageProgress(_snapshots.Count);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Failed to save image for logData with LevelID: {snapshot.LevelID}. Exception: {ex}");
                        }
                    }
                }
                
                // Remove the processed snapshot from the list.
                _snapshots.Remove(snapshot);
                
                yield return null; // Wait for the next frame.
            }
        }

        private void WriteLog()
        {
            foreach (LogData logData in _logDataList)
            {
                // Write each log data to the CSV file.
                try
                {
                    WriteLogLine(logData);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to write log for logData with LevelID: {logData.LevelID}. Exception: {ex}");
                }

                // Write each FaceExpression to a json file.
                try
                {
                    if (logData.FaceExpressions != null)
                        WriteFaceExpressionJson(logData);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to write log for logData with LevelID: {logData.LevelID}. Exception: {ex}");
                }
            }

            _logDataList.Clear();
        }

        private void WriteFaceExpressionJson(LogData logData)
        {
            // Construct the path for saving the image
            string path = Path.Combine(_dirPathWithUserID, logData.LevelID, logData.EmoteEmoji.ToString());
            
            string filename = $"{logData.Timestamp}.json";
            
            // Write json file
            SaveFiles.WriteFile(path, filename, logData.FaceExpressions);
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
            SaveFiles.AppendLineToCsv(_dirPathWithUserID, CsvFileName, data);
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
            SaveFiles.AppendLineToCsv(_dirPathWithUserID, CsvFileName, data);
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

        public Snapshot LatestSnapshot
        {
            get => _snapshots.LastOrDefault();
            set
            {
                // replace the Snapshots in Training mode
                if (!LogTrainingLevel && value.LevelMode == ELevelMode.Training)
                    _snapshots.Clear();
                
                // Add Snapshot to List
                _snapshots.Add(value);
                EditorUI.EditorUI.Instance.UpdateImageBacklog(_snapshots.Count);
            }
        }
    }
}
