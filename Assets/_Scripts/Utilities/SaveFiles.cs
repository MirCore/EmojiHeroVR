using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Utilities
{
    public abstract class SaveFiles
    {
        /// <summary>
        /// Save binary data (byte array) to a file asynchronously
        /// </summary>
        public static void SaveFile(string path, string fileName, byte[] bytes)
        {
            SaveFileInternal(path, fileName, async filePath => await WriteFile(bytes, filePath));
        }

        /// <summary>
        /// Save text data (base64-encoded string) to a file asynchronously
        /// </summary>
        public static void SaveFile(string path, string fileName, string base64)
        {
            SaveFileInternal(path, fileName, async filePath => await WriteFile(base64, filePath));
        }

        /// <summary>
        /// Internal method to handle logic for saving files
        /// </summary>
        private static async void SaveFileInternal(string path, string fileName, Func<string, Task> writeFileAction)
        {
            // Combine the specified path with the application's data path
            string dirPath = Path.Combine(Application.dataPath + path);
            
            // Create the directory if it doesn't exist
            if(!Directory.Exists(dirPath)) 
                Directory.CreateDirectory(dirPath);

            try
            {
                string filePath = Path.Combine(dirPath, fileName);
                await writeFileAction(filePath);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving file {fileName}: {e}");
            }
        }

        /// <summary>
        /// Asynchronously write text to a file
        /// </summary>
        private static async Task WriteFile(string content, string filePath)
        {
            await File.WriteAllTextAsync(filePath, content);
        }

        /// <summary>
        /// Asynchronously write binary data to a file
        /// </summary>
        private static async Task WriteFile(byte[] bytes, string filePath)
        {
            await File.WriteAllBytesAsync(filePath, bytes);
        }
        
    }
}