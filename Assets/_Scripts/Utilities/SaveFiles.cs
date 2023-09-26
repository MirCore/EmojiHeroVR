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
#if UNITY_EDITOR
            SaveFileInternal(path, fileName, async filePath => await WriteFile(bytes, filePath));
#endif
        }

        /// <summary>
        /// Save text data (base64-encoded string) to a file asynchronously
        /// </summary>
        public static void SaveFile(string path, string fileName, string base64)
        {
#if UNITY_EDITOR
            SaveFileInternal(path, fileName, async filePath => await WriteFile(base64, filePath));
#endif
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
                Console.WriteLine($"Error saving file {fileName}: {e}");
                throw;
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