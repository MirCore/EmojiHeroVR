using System;
using Enums;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private Transform EmojiEndPosition;
    [field: SerializeField] public TextureMapping[] TextureMappings { get; private set; }
    [field: SerializeField] public float Speed { get; private set; }

    private void Awake()
    {
        // Create Singleton of this Class
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public Vector3 GetEndPosition()
    {
        return EmojiEndPosition.position;
    }
}

[Serializable]
public class TextureMapping
{
    public EEmote EEmote;
    public Texture Texture;
}
