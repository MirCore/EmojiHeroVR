using System.Collections.Generic;
using System.Linq;
using Manager;
using UnityEngine;
using UnityEngine.Profiling;
using Utilities;

public class FerService
{
    public FaceExpressionData AnalyzeImage(Color32[] image)
    {
        FaceExpressionData ferData = new()
        {
            DetectedFaces = new List<DetectedFace>(),
            Probabilities = new List<Probabilities>()
        };

        Profiler.BeginSample("GetImage");
        // Convert the captured image to base64 format.
        Texture2D texture2D = WebcamManager.GetImage(image);
        Profiler.EndSample();

        Profiler.BeginSample("DetectFace");
        // Send the image for FER processing.
        FaceDetection.Instance.DetectFaces(texture2D, ferData, 0.3f);
        Profiler.EndSample();

        Profiler.BeginSample("DetectEmotion");
        if (ferData.DetectedFaces.Any())
        {
            foreach (Texture2D tempTexture in ferData.DetectedFaces.Select(face => CreateTempTexture(texture2D, face)))
            {
                ferData.Probabilities.Add(EmotionRecognition.Instance.DetectEmotion(tempTexture));
            }
        }

        Profiler.EndSample();

        return ferData;
    }

    private static Texture2D CreateTempTexture(Texture2D texture2D, DetectedFace face)
    {
        // Create a temporary texture to hold the cropped image
        Texture2D tempTexture = new(face.width, face.height);
        Color[] pixels = texture2D.GetPixels(face.x, face.y, face.width, face.height);
        tempTexture.SetPixels(pixels);
        tempTexture.Apply();
        return tempTexture;
    }
}