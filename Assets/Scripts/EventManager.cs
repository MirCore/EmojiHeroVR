using System.Collections;
using System.Collections.Generic;
using Enums;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void EmoteEnteredArea(EEmote emote);
    public static event EmoteEnteredArea OnEmoteEnteredArea;
    public static void InvokeEmoteEnteredArea(EEmote emote)
    {
        OnEmoteEnteredArea?.Invoke(emote);
    }
    
    public delegate void EmotionDetected(EEmote emote);
    public static event EmotionDetected OnEmotionDetected;
    public static void InvokeEmotionDetected(EEmote emote)
    {
        OnEmotionDetected?.Invoke(emote);
    }
    
    public delegate void EmoteExitedArea();
    public static event EmoteExitedArea OnEmoteExitedArea;
    public static void InvokeEmoteExitedArea()
    {
        OnEmoteExitedArea?.Invoke();
    }
}
