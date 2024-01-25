using System.Collections.Generic;
using System.Linq;
using Enums;
using Manager;
using UnityEngine;
using UnityEngine.Profiling;
using Utilities;

public static class FerService
{
    public static List<DetectedFace> AnalyzeImage(Color32[] image, float scoreThreshold, float sizeThreshold)
    {
        Profiler.BeginSample("ConvertColor32ToTexture2D");
        // Convert the captured image to base64 format.
        Texture2D texture2D = WebcamManager.ConvertColor32ToTexture2D(image);
        Profiler.EndSample();

        Profiler.BeginSample("DetectFace");
        // Send the image for FER processing.
        IEnumerable<DetectedFace> detectedFaces = FaceDetection.Instance.DetectFaces(texture2D, scoreThreshold);
        Profiler.EndSample();

        List<DetectedFace> filteredFaces = FilterResults(detectedFaces, sizeThreshold * texture2D.width);
        
        Profiler.BeginSample("DetectEmotion");
        if (filteredFaces.Count > 0)
        {
            foreach (DetectedFace face in filteredFaces)
            {
                Texture2D tempTexture = CreateTempTexture(texture2D, face);
                Probabilities probabilities = EmotionRecognition.Instance.DetectEmotion(tempTexture);
                face.Emote = GetEmoteWithHighestProbability(probabilities);
            }
        }

        Profiler.EndSample();

        return filteredFaces;
    }

    private static List<DetectedFace> FilterResults(IEnumerable<DetectedFace> detectedFaces, float sizeThreshold)
    {
        List<DetectedFace> filteredFaces = new();

        foreach (DetectedFace face in detectedFaces.Where(face => face.Width > sizeThreshold).Where(face => !filteredFaces.Any(otherFace => AreFacesOverlapping(face, otherFace))))
        {
            filteredFaces.Add(face);
        }

        return filteredFaces;
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
    
    /// <summary>
    /// Determines the emotion with the highest probability from the FER results.
    /// </summary>
    /// <param name="probabilities">The FER probabilities for each emotion.</param>
    /// <returns>The emotion with the highest probability.</returns>
    private static EEmote GetEmoteWithHighestProbability(Probabilities probabilities)
    {
        // Map each emotion to its probability.
        Dictionary<EEmote, float> result = new()
        {
            { EEmote.Anger, probabilities.anger },
            { EEmote.Disgust, probabilities.disgust },
            { EEmote.Fear, probabilities.fear },
            { EEmote.Happiness, probabilities.happiness },
            { EEmote.Neutral, probabilities.neutral },
            { EEmote.Sadness, probabilities.sadness },
            { EEmote.Surprise, probabilities.surprise }
        };

        // Return the emotion with the highest probability.
        return result.OrderByDescending(kv => kv.Value).First().Key;
    }
}

    
public class DetectedFace
{
    public float Score;

    public float RelativeX;
    public float RelativeY;
    public float RelativeWidth;
    public float RelativeHeight;
        
    public int X;
    public int Y;
    public int Width;
    public int Height;
    
    public EEmote Emote;
}