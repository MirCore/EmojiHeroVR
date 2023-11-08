using System;
using System.IO;
using System.Threading;

namespace Utilities
{
    public abstract class SaveFiles
    {
        public static void SaveImageFile(string dirPath, string filename, byte[] bytes)
        {
            // Start a new thread for file saving to avoid blocking the main thread
            Thread saveFile = new(() =>
            {
                // Save the captured image to a file
                SaveFileInternal(dirPath, filename, bytes);
            });
            saveFile.Start();
        }

        public static void WriteFile(string dirPath, string filename, string data)
        {
            SaveFileInternal(dirPath, filename, data);
        }
        
        /// <summary>
        /// Internal method to handle logic for saving byte[] files
        /// </summary>
        private static async void SaveFileInternal(string path, string fileName, byte[] bytes)
        { 
            // Create the directory if it doesn't exist
            if(!Directory.Exists(path)) 
                Directory.CreateDirectory(path);

            try
            {
                string filePath = Path.Combine(path, fileName);
                await File.WriteAllBytesAsync(filePath, bytes);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving file {fileName}: {e}");
                throw;
            }
        }
        
        /// <summary>
        /// Internal method to handle logic for saving string files
        /// </summary>
        private static async void SaveFileInternal(string path, string fileName, string data)
        { 
            // Create the directory if it doesn't exist
            if(!Directory.Exists(path)) 
                Directory.CreateDirectory(path);

            try
            {
                string filePath = Path.Combine(path, fileName);
                await File.WriteAllTextAsync(filePath, data);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving file {fileName}: {e}");
                throw;
            }
        }

        /// <summary>
        /// Appends a line of data to a CSV file.
        /// </summary>
        /// <param name="dirPath">The directory path where the CSV file is located.</param>
        /// <param name="fileName">The name of the CSV file (including the extension).</param>
        /// <param name="data">An array of data to append as a CSV line.</param>
        public static void AppendLineToCsv(string dirPath, string fileName, string[] data)
        {
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
        }

    }
}