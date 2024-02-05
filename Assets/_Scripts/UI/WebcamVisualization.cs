using System.Collections.Generic;
using System.Linq;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class WebcamVisualization : MonoBehaviour
    {
        [SerializeField] private RawImage WebcamTexture;
        [SerializeField] private Image WebcamOverlay;
        [SerializeField] private GameObject Emoji;
        private readonly List<RectTransform> _emojis = new();

        [SerializeField] private float EmojiScaleFactor = 7f;


        private float _rectWidth;
        private float _rectHeight;
        private float _emojiScaleFactor;

        private readonly int _sprite = Shader.PropertyToID("_Sprite");

        private void OnEnable()
        {
            WebcamTexture.GetComponent<AspectRatioFitter>().aspectRatio = WebcamManager.GetCameraRatio();
            WebcamOverlay.GetComponent<AspectRatioFitter>().aspectRatio = WebcamManager.GetCameraRatio();
            CalculateDimensions();
        }

        private void CalculateDimensions()
        {
            Rect rect = WebcamTexture.rectTransform.rect;
            _rectHeight = rect.height;
            _rectWidth = rect.width;
            _emojiScaleFactor = EmojiScaleFactor * _rectWidth;
        }

        public void PositionEmojis(List<DetectedFace> detectedFaces)
        {
            if (_rectWidth == 0)
                CalculateDimensions();
            
            for (int i = 0; i < detectedFaces.Count; i++)
            {
                if (detectedFaces[i] == null)
                    continue;
                if (_emojis.Count <= i)
                    SpawnEmoji();
                
                RectTransform emoji = _emojis[i];
                float scale = _emojiScaleFactor * detectedFaces[i].RelativeWidth;
                emoji.sizeDelta = (emoji.sizeDelta + new Vector2(scale, scale)) / 2;
                emoji.anchoredPosition = (emoji.anchoredPosition + new Vector2(
                    (detectedFaces[i].RelativeX + detectedFaces[i].RelativeWidth / 2) * _rectWidth - _rectWidth / 2,
                    (detectedFaces[i].RelativeY + detectedFaces[i].RelativeHeight / 2) * _rectHeight - _rectHeight / 2)) / 2;
                
                emoji.gameObject.SetActive(true);
            }

            for (int i = detectedFaces.Count(); i < _emojis.Count; i++)
            {
                _emojis[i].gameObject.SetActive(false);
            }
        }

        private void SpawnEmoji()
        {
            GameObject emoji = Instantiate(Emoji, WebcamTexture.transform);
            _emojis.Add(emoji.GetComponentInChildren<RectTransform>());
        }
    }
}