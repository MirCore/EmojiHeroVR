using System;
using System.Collections;
using System.IO;
using UnityEngine;
using Utilities;

namespace Systems
{
    public abstract class SaveImage
    {
        private static IEnumerator WriteImages(Texture2D texture)
        {
            yield return null; // Wait for a second to let all systems finish.

            // Construct the path for saving the image
            string path = Path.Combine(Application.dataPath + "/../SaveFiles/");

            try
            {
                // Encode the image to PNG format.
                byte[] bytes = texture.EncodeToPNG();

                // Add webcam index to filename if its not the main webcam (index > 0)
                string filename = $"{DateTime.Now}.png";

                // Save the image file.
                SaveFiles.SaveImageFile(path, filename, bytes);
            }
            catch (Exception ex)
            {
                Debug.LogError(
                    $"Failed to save image. Exception: {ex}");
            }
            
        }

        public static void WriteImage(Texture2D texture)
        {
            // Construct the path for saving the image
            string path = Path.Combine(Application.dataPath + "/../SaveFiles/");

            try
            {
                // Encode the image to PNG format.
                byte[] bytes = texture.EncodeToPNG();

                string filename = $"face.png";

                // Save the image file.
                SaveFiles.SaveImageFile(path, filename, bytes);
            }
            catch (Exception ex)
            {
                Debug.LogError(
                    $"Failed to save image. Exception: {ex}");
            }
        }
    }
}
