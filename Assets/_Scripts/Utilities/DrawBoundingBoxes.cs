// Based of https://github.com/doughtmw/BoundingBoxUtils-Unity

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public class DrawBoundingBoxes : Singleton<DrawBoundingBoxes>
    {
        private Material _material;
        private Texture2D _texture;
        private Color[] _fillPixels;

        [SerializeField] private RawImage RawImage;

        private void Start()
        {
            // Create a new texture instance with same size as the canvas.
            Rect rect = RawImage.rectTransform.rect;
            _texture = new Texture2D((int)rect.width, (int)rect.height);

            // Set the texture to transparent (with helper method)
            TransparentTexture(_texture);

            // Apply and set main material texture;
            _texture.Apply();
            RawImage.texture = _texture;
        }

        public void DrawBoundingBox(List<DetectedFace> detectedFaces)
        {
            // Draw bounding box at specified coordinates.
            
            // reset Texture
            _texture.SetPixels(_fillPixels);

            foreach (DetectedFace face in detectedFaces)
            {
                DrawBoundingBoxesOnCanvas(face);
            }

            // Apply and set main material texture;
            _texture.Apply();
        }

        private void DrawBoundingBoxesOnCanvas(DetectedFace face)
        {
            // Check the bounds of bounding box
            // Give buffer for line drawing to prevent wrap around
            int x1 = (int)Mathf.Clamp(face.RelativeX * _texture.width, 3, _texture.width - 3);
            int y1 = (int)Mathf.Clamp(face.RelativeY * _texture.height, 3, _texture.height - 3);
            int x2 = (int)Mathf.Clamp(face.RelativeWidth * _texture.width + x1 , 3, _texture.width - 3);
            int y2 = (int)Mathf.Clamp(face.RelativeHeight * _texture.height + y1 , 3, _texture.height - 3);

            //Debug.LogFormat("x1: {0}, y1: {1}, x2: {2}, y2: {3}", x1, y1, x2, y2);

            // Plot on texture
            Vector2 topLeft = new (x1, y1);
            Vector2 bottomRight = new (x2, y2);
            _texture = Box(
                _texture,
                topLeft,
                bottomRight,
                Color.red);
        }

        // Draw a box given two vec2d points.
        // Top left corner, bottom right corner
        private static Texture2D Box(
            Texture2D tex,
            Vector2 tl,
            Vector2 br,
            Color color)
        {
            // Draw line connecting top left and top right
            var tr = new Vector2(br.x, tl.y);
            tex = Line(tex, tl, tr, color);

            // Draw line connecting top left and bottom left
            var bl = new Vector2(tl.x, br.y);
            tex = Line(tex, tl, bl, color);

            // Draw line connecting bottom left and bottom right
            tex = Line(tex, bl, br, color);

            // Draw line connecting bottom right and top right
            tex = Line(tex, br, tr, color);

            return tex;
        }

        // Draw line between two vec2d points.
        // https://answers.unity.com/questions/244417/create-line-on-a-texture.html
        private static Texture2D Line(
            Texture2D tex,
            Vector2 p1,
            Vector2 p2,
            Color color)
        {
            Vector2 t = p1;
            float frac = 1 / Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.y - p1.y, 2));
            float ctr = 0;

            while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y)
            {
                t = Vector2.Lerp(p1, p2, ctr);
                ctr += frac;
                tex.SetPixel((int)t.x - 2, (int)t.y - 2, color);
                tex.SetPixel((int)t.x - 1, (int)t.y - 1, color);
                tex.SetPixel((int)t.x, (int)t.y, color);
                tex.SetPixel((int)t.x + 1, (int)t.y + 1, color);
                tex.SetPixel((int)t.x + 2, (int)t.y + 2, color);
            }

            return tex;
        }

        // Set texture as transparent to initialize canvas for drawing.
        private void TransparentTexture(Texture2D tex)
        {
            Color fillColor = Color.clear;
            _fillPixels = new Color[tex.width * tex.height];

            for (int i = 0; i < _fillPixels.Length; i++)
            {
                _fillPixels[i] = fillColor;
            }
        }
    }
}