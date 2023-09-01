using System;
using System.Linq;
using Enums;
using UnityEngine;
using Random = UnityEngine.Random;

public class EmojiManager : MonoBehaviour
{
    private Vector3 _endPosition;
    [SerializeField] private EEmote Emote;
    [SerializeField] private Renderer EmojiRenderer;


    private void OnEnable()
    {
        Emote = (EEmote)Random.Range(0, Enum.GetValues(typeof(EEmote)).Length);
        UpdateTextures();
        _endPosition = GameManager.Instance.GetEndPosition();
        _endPosition += new Vector3(transform.position.x, 0, 0);
    }

    private void Update()
    {
        float step = GameManager.Instance.Speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _endPosition, step);

        if (Vector3.Distance(transform.position, _endPosition) < 0.001f)
        {
            gameObject.SetActive(false);
        }
    }

    private void UpdateTextures()
    {
        Texture texture = (from textureMapping in GameManager.Instance.TextureMappings where textureMapping.EEmote == Emote select textureMapping.Texture).FirstOrDefault();
        EmojiRenderer.material.mainTexture = texture;
        EmojiRenderer.material.SetTexture(314, texture);
    }
}
