using System.Collections.Generic;
using System.Linq;
using Enums;
using Manager;
using Systems;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public class WebcamPreview : MonoBehaviour
    {
        [SerializeField] private RawImage WebcamTexture;
        [SerializeField] private GameObject Emoji;
        private readonly List<MeshRenderer> _emojis = new();

        [SerializeField] private float EmojiScaleFactor = 10f;

        [SerializeField] private bool EnableBoundingBoxes;

        private float _rectWidth;
        private float _rectHeight;
        private float _emojiScaleFactor;

        private readonly int _sprite = Shader.PropertyToID("_Sprite");
        [SerializeField] private float ScoreThreshold = 0.3f;
        [SerializeField] private float SizeThreshold = 0.1f;

        private void OnEnable()
        {
            CalculateDimensions();
        }

        private void CalculateDimensions()
        {
            Rect rect = WebcamTexture.rectTransform.rect;
            _rectHeight = rect.height;
            _rectWidth = _rectHeight * WebcamManager.GetCameraRatio();
            WebcamTexture.rectTransform.sizeDelta = new Vector2(_rectWidth, _rectHeight);
            _emojiScaleFactor = EmojiScaleFactor * _rectWidth;
        }

        private void Update()
        {
            Color32[] image = WebcamManager.TakeSnapshot();
            List<DetectedFace> detectedFaces = FerService.GetEmotions(image, ScoreThreshold, SizeThreshold, -1);
        
            if (!detectedFaces.Any())
                return;
        
            if (EnableBoundingBoxes)
                DrawBoundingBoxes.Instance.DrawBoundingBox(detectedFaces);
        
            foreach (DetectedFace face in detectedFaces)
            {
                if (face == null)
                    continue;
                if (face.Emote != EEmote.None)
                    EventManager.InvokeEmotionDetected(face);
            }
            
            detectedFaces = detectedFaces.OrderBy(face => face.RelativeX).ToList();
            PositionEmojis(detectedFaces);
        }

        private void PositionEmojis(IReadOnlyList<DetectedFace> detectedFaces)
        {
            for (int i = 0; i < detectedFaces.Count; i++)
            {
                if (_emojis.Count <= i)
                    SpawnEmoji();
                float scale = _emojiScaleFactor * detectedFaces[i].RelativeWidth;
                _emojis[i].transform.localScale = (_emojis[i].transform.localScale + new Vector3(scale, scale, scale)) / 2;
                _emojis[i].transform.localPosition = (_emojis[i].transform.localPosition + new Vector3(
                    (detectedFaces[i].RelativeX + detectedFaces[i].RelativeWidth / 2) * _rectWidth - _rectWidth / 2,
                    (detectedFaces[i].RelativeY + detectedFaces[i].RelativeHeight / 2) * _rectHeight - _rectHeight / 2)) / 2;
            
            
                Texture texture = ResourceSystem.EmojiScriptables[detectedFaces[i].Emote].Textures[0];
            
                _emojis[i].material.SetTexture(_sprite, texture);
                _emojis[i].gameObject.SetActive(true);
            }

            for (int i = detectedFaces.Count; i < _emojis.Count; i++)
            {
                _emojis[i].gameObject.SetActive(false);
            }
        }

        private void SpawnEmoji()
        {
            GameObject emoji = Instantiate(Emoji, WebcamTexture.transform);
            _emojis.Add(emoji.GetComponentInChildren<MeshRenderer>());
        }
    }
}