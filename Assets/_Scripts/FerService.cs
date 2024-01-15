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
            Probabilities = new List<Probabilities>()
        };

        Profiler.BeginSample("ConvertColor32ToTexture2D");
        // Convert the captured image to base64 format.
        Texture2D texture2D = WebcamManager.ConvertColor32ToTexture2D(image);
        Profiler.EndSample();

        Profiler.BeginSample("DetectFace");
        // Send the image for FER processing.
        ferData.DetectedFaces = FaceDetection.Instance.DetectFaces(texture2D, 0.3f);
        Profiler.EndSample();

        FilterResults(ferData);
        
        Profiler.BeginSample("DetectEmotion");
        if (ferData.FilteredFaces.Count > 0)
        {
            foreach (Texture2D tempTexture in ferData.FilteredFaces.Select(face => CreateTempTexture(texture2D, face)))
            {
                ferData.Probabilities.Add(EmotionRecognition.Instance.DetectEmotion(tempTexture));
            }
        }

        Profiler.EndSample();

        return ferData;
    }

    private void FilterResults(FaceExpressionData ferData)
    {
        List<DetectedFace> filteredFaces = new();
        
        foreach (DetectedFace face in ferData.DetectedFaces.Where(face => !filteredFaces.Any(otherFace => AreFacesOverlapping(face, otherFace))))
        {
            filteredFaces.Add(face);
        }

        ferData.FilteredFaces = filteredFaces;
    }

    private static bool AreFacesOverlapping(DetectedFace face1, DetectedFace face2)
    {
        // Check if one rectangle is on left side of other
        if (face1.X > face2.X + face2.Width || face2.X > face1.X + face1.Width)
            return false;
        // Check if one rectangle is under the other
        if (face1.Y > face2.Y + face2.Height || face2.Y > face1.Y + face1.Height)
            return false;
        
        return true;
    }
    


    private static Texture2D CreateTempTexture(Texture2D texture2D, DetectedFace face)
    {
        // Create a temporary texture to hold the cropped image
        Texture2D tempTexture = new(face.Width, face.Height);
        Color[] pixels = texture2D.GetPixels(face.X, face.Y, face.Width, face.Height);
        tempTexture.SetPixels(pixels);
        tempTexture.Apply();
        return tempTexture;
    }
}