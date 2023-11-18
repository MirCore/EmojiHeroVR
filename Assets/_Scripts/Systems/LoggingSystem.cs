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
        private const string FaceExpressionCsvFileName = "faceexpressions.csv"; // The name of the log file
        private string _dirPathWithUserID; // The full directory path where the log file will be stored

        private readonly List<LogData> _logDataList = new(); // A list to store log data temporarily.
        private readonly List<Snapshot> _snapshots = new(); // A list to store log data temporarily.
        private readonly List<FaceExpression> _faceExpressions = new();

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

            // Write FaceExpressions to a CSV file.
            WriteFaceExpressions();

            // Start coroutine to write all images post play.
            StartCoroutine(WriteImages());
        }

        private IEnumerator WriteImages()
        {
            yield return new WaitForSecondsRealtime(1); // Wait for a second to let all systems finish.

            Texture2D texture = new(WebcamManager.WebcamWidth, WebcamManager.WebcamHeight);

            // Continue until all snapshots is processed.
            while (_snapshots.Any())
            {
                // Get the first snapshot from the list.
                Snapshot snapshot = _snapshots.FirstOrDefault();

                if (snapshot != null && (LogTrainingLevel || snapshot.LevelMode != ELevelMode.Training))
                {
                    // Construct the path for saving the image
                    string path = Path.Combine(_dirPathWithUserID, snapshot.LevelID,
                        $"{snapshot.Emoji.EmoteID}-{snapshot.Emoji.Emote}");

                    for (int i = 0; i < snapshot.ImageTextures.Count; i++)
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
                            Debug.LogError(
                                $"Failed to save image for logData with LevelID: {snapshot.LevelID}. Exception: {ex}");
                        }
                }

                // Remove the processed snapshot from the list.
                _snapshots.Remove(snapshot);

                yield return null; // Wait for the next frame.
            }
        }

        private void WriteLog()
        {
            string logDataCsvString = _logDataList.Aggregate("", (current, logData) => current + $"{GetLogDataString(logData)}\n");

            try
            {
                // Append the data as a line to the log CSV file
                SaveFiles.AppendLineToCsv(_dirPathWithUserID, CsvFileName, logDataCsvString);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to write log for faceExpression. Exception: {ex}");
            }

            _logDataList.Clear();
        }

        private void WriteFaceExpressions()
        {
            string faceExpressionCsvString = _faceExpressions.Aggregate("",
                (current, faceExpression) => current + $"{GetFaceExpressionString(faceExpression)}\n");

            // Write to the CSV file.
            try
            {
                // Append the data as a line to the log CSV file
                SaveFiles.AppendLineToCsv(_dirPathWithUserID, FaceExpressionCsvFileName, faceExpressionCsvString);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to write log for faceExpression. Exception: {ex}");
            }

            _faceExpressions.Clear();
        }

        private static string GetFaceExpressionString(FaceExpression faceExpression)
        {
            // Prepare the data as an array of strings.
            string[] data =
            {
                faceExpression.Timestamp,
                faceExpression.LevelID,
                faceExpression.Emoji.EmoteID.ToString(),
                faceExpression.Emoji.Emote.ToString(),
                faceExpression.FaceExpressionJson
            };

            // Concatenate the data array into a CSV line using semicolons as separators
            return string.Join(";", data);
        }

        /// <summary>
        /// returns a single line of log data for the CSV file.
        /// </summary>
        /// <param name="logData">The log data to be written.</param>
        private string GetLogDataString(LogData logData)
        {
            // Prepare the data as an array of strings.
            string[] data =
            {
                logData.Timestamp,
                logData.LevelID,
                logData.Emoji.EmoteID.ToString(),
                logData.Emoji.Emote.ToString(),
                logData.EmoteFer.ToString(),
                logData.FerProbabilities.anger.ToString("F2"),
                logData.FerProbabilities.disgust.ToString("F2"),
                logData.FerProbabilities.fear.ToString("F2"),
                logData.FerProbabilities.happiness.ToString("F2"),
                logData.FerProbabilities.neutral.ToString("F2"),
                logData.FerProbabilities.sadness.ToString("F2"),
                logData.FerProbabilities.surprise.ToString("F2"),
                logData.UserID,
                logData.FaceExpressions
            };

            // Concatenate the data array into a CSV line using semicolons as separators
            return string.Join(";", data);
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

        public void AddToFaceExpressionList(FaceExpression faceExpression)
        {
            _faceExpressions.Add(faceExpression);
        }

        public bool FinishedSaving()
        {
            return !_snapshots.Any();
        }
    }
}