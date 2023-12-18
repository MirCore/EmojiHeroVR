using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enums;
using Manager;
using UnityEngine;
using UnityEngine.Profiling;
using Utilities;

/// <summary>
/// Handles the Facial Emotion Recognition (FER) processes, including capturing images, sending them for analysis, and processing the results.
/// </summary>
public class FerHandler : MonoBehaviour
{
    // Coroutine for facial emotion recognition
    private Coroutine _coroutine;

    private void Start()
    {
        EventManager.OnEmoteEnteredActionArea += EmoteEnteredActionAreaCallback;
    }

    private void OnDestroy()
    {
        EventManager.OnEmoteEnteredActionArea -= EmoteEnteredActionAreaCallback;
    }
    
    // Callback for when an emote enters the action area, triggers the facial emotion recognition
    private void EmoteEnteredActionAreaCallback(Emoji emoji) => SendRestImage();

    /// <summary>
    /// Initiates the sending of REST images for FER processing.
    /// </summary>
    private void SendRestImage()
    {
        _coroutine ??= StartCoroutine(DetectEmotion());
    }

    /// <summary>
    /// Captures a webcam frame, converts it to base64, and sends it for FER processing.
    /// </summary>
    private IEnumerator DetectEmotion()
    {
#if UNITY_EDITOR
        // Log a new FER request.
        EditorUIFerStats.Instance.LogNewRestRequest();
#endif
        
        Color32[] snapshot = WebcamManager.TakeSnapshots();
        
        Profiler.BeginSample("GetImage");
        // Convert the captured image to base64 format.
        Texture2D image = WebcamManager.GetImage(snapshot);
        Profiler.EndSample();
        yield return null;  // Wait until the next frame to reduce lag

        Profiler.BeginSample("DetectFace");
        // Send the image for FER processing.
        Texture2D face = FaceDetection.Instance.DetectFace(image, this);
        Profiler.EndSample();
        yield return null;  // Wait until the next frame to reduce lag
        
        Profiler.BeginSample("DetectEmotion");
        if (face != null)
        {
            EmotionRecognition.Instance.DetectEmotion(face, this);
        }
        Profiler.EndSample();
    }
    
    /// <summary>
    /// Processes the REST probabilities from the FER API.
    /// </summary>
    /// <param name="probabilities">The JSON probabilities from the FER service.</param>
    public void ProcessFerResponse(Probabilities probabilities)
    {
        // Determine the emotion with the highest probability.
        EEmote emoteFer = GetEmoteWithHighestProbability(probabilities);
        // Trigger an event for the detected emotion.
        EventManager.InvokeEmotionDetected(emoteFer);
        
        HandleFerCompletion(probabilities);
    }

    /// <summary>
    /// Handles errors that occur during the REST call for FER processing.
    /// </summary>
    /// <param name="error">The exception thrown during the REST call.</param>
    public void ProcessFerError(Exception error)
    {
        // Log the error message.
        Debug.LogWarning("FER Error: " + error.Message);
        
        HandleFerCompletion(new Probabilities());
    }

    private void HandleFerCompletion(Probabilities probabilities)
    {
        _coroutine = null;
        
#if UNITY_EDITOR
        // Update the UI with the FER results.
        EditorUIFerStats.Instance.LogRestResponse(probabilities);
#endif

        // If emojis are still in the action area, continue the FER process.
        if (GameManager.LevelProgress.EmojisAreInActionArea)
            StartCoroutine(SendRestImageNextFrame());
    }

    private IEnumerator SendRestImageNextFrame()
    {
        yield return null;  // Wait until the next frame to reduce lag
        SendRestImage();
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

