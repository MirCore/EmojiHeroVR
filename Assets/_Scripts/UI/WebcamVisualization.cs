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
        [SerializeField] private GameObject WebcamOverlay;
        [SerializeField] private List<GameObject> WebcamOverlays = new();
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
            
            while (GameManager.Instance.PlayerCount > WebcamOverlays.Count)
            {
                WebcamOverlays.Add(Instantiate(WebcamOverlay, WebcamTexture.transform));
            }

            while (GameManager.Instance.PlayerCount < WebcamOverlays.Count)
            {
                GameObject overlay = WebcamOverlays.LastOrDefault();
                WebcamOverlays.Remove(overlay);
                Destroy(overlay);
            }
                
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
                if (_emojis.Count <= i)
                    SpawnEmoji();
                if (detectedFaces[i] == null)
                {
                    _emojis[i].gameObject.SetActive(false);
                    continue;
                }
                
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
            GameObject emoji = Instantiate(Emoji, transform);
            _emojis.Add(emoji.GetComponentInChildren<RectTransform>());
        }
    }
}