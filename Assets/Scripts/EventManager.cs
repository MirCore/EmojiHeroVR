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
}
