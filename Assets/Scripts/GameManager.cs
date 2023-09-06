using System;
using Enums;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [field: SerializeField] public Transform EmojiEndPosition { get; private set; }
    [field: SerializeField] public Transform ActiveAreaStartPosition { get; private set; }
    [field: SerializeField] public Transform ActiveAreaEndPosition { get; private set; }
    [field: SerializeField] public TextureMapping[] TextureMappings { get; private set; }
    [field: SerializeField] public float Speed { get; private set; }

    private REST _rest;

    private EEmote _emojiInActionArea;

    private void Awake()
    {
        // Create Singleton of this Class
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        EventManager.OnEmoteEnteredArea += OnEmoteEnteredAreaCallback;
        EventManager.OnEmoteExitedArea += OnEmoteExitedAreaCallback;

        _rest = new REST();
    }


    private void OnDestroy()
    {
        EventManager.OnEmoteEnteredArea -= OnEmoteEnteredAreaCallback;
        EventManager.OnEmoteExitedArea -= OnEmoteExitedAreaCallback;
    }

    private void OnEmoteEnteredAreaCallback(EEmote emote)
    {
        _emojiInActionArea = emote;
        SendRestImage();
    }

    private void SendRestImage()
    {
        //_rest.Post();
        _ = _rest.FakePost(Random.Range(0.1f,0.7f));
    }

    private void OnEmoteExitedAreaCallback()
    {
        _emojiInActionArea = EEmote.None;
    }

    public void ProcessRestResponse(Post response)
    {
        if (response.result)
        {
            Debug.Log(response.result.ToString());
            EventManager.InvokeEmotionDetected(_emojiInActionArea);
        }

        if (_emojiInActionArea != EEmote.None)
        {
            SendRestImage();
        }
    }
}

[Serializable]
public class TextureMapping
{
    public EEmote EEmote;
    public Texture Texture;
}
