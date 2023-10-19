using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Manager;
using Systems;
using UnityEngine;

namespace Utilities
{
    public abstract class SaveFiles
    {
        public static string GetUnixTimestamp()
        {
            return ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds().ToString();
        }

        public static string SaveImageFile(string dirPath, string timestamp, byte[] bytes)
        {
            string filename =
                $"{timestamp}-" +
                $"{EditorUI.EditorUI.Instance.UserID}-" +
                $"{GameManager.Instance.GetEmojiInActionArea().FirstOrDefault()}-" +
                $"{FerHandler.Instance.LastDetectedEmote}.jpg";
            
            // Start a new thread for file saving to avoid blocking the main thread
            Thread saveFile = new(() =>
            {
                // Save the captured image to a file
                SaveFile(dirPath, filename, bytes);
            });
            saveFile.Start();
            return filename;
        }
        
        /// <summary>
        /// Save binary data (byte array) to a file asynchronously
        /// </summary>
        private static void SaveFile(string path, string fileName, byte[] bytes)
        {
#if UNITY_EDITOR
            SaveFileInternal(path, fileName, async filePath => await WriteFile(bytes, filePath));
#endif
        }

        /// <summary>
        /// Internal method to handle logic for saving files
        /// </summary>
        private static async void SaveFileInternal(string path, string fileName, Func<string, Task> writeFileAction)
        { 
            // Create the directory if it doesn't exist
            if(!Directory.Exists(path)) 
                Directory.CreateDirectory(path);

            try
            {
                string filePath = Path.Combine(path, fileName);
                await writeFileAction(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving file {fileName}: {e}");
                throw;
            }
        }

        /// <summary>
        /// Asynchronously write binary data to a file
        /// </summary>
        private static async Task WriteFile(byte[] bytes, string filePath)
        {
            await File.WriteAllBytesAsync(filePath, bytes);
        }

        /// <summary>
        /// Appends a line of data to a CSV file.
        /// </summary>
        /// <param name="dirPath">The directory path where the CSV file is located.</param>
        /// <param name="fileName">The name of the CSV file (including the extension).</param>
        /// <param name="data">An array of data to append as a CSV line.</param>
        public static void AppendLineToCsv(string dirPath, string fileName, string[] data)
        {
#if UNITY_EDITOR
            // Combine the directory path and file name to get the full file path
            string filePath = Path.Combine(dirPath, fileName);
            
            // Create the directory if it doesn't exist
            if(!Directory.Exists(dirPath)) 
                Directory.CreateDirectory(dirPath);
            
            try
            {
                // Create or append to the CSV file
                using StreamWriter writer = new (filePath, true);
                
                // Concatenate the data array into a CSV line using semicolons as separators
                string csvLine = string.Join(";", data);
                    
                // Write the CSV line to the file
                writer.WriteLine(csvLine);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to append data to CSV file {fileName}: {e}");
                throw;
            }
#endif
        }
        
    }
}