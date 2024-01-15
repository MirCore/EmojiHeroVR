using System;
using System.Collections.Generic;
using System.Linq;
using Manager;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class WebcamPreview : MonoBehaviour
{
    private FerService _ferService;

    [SerializeField] private RawImage WebcamTexture;
    [SerializeField] private GameObject Emoji;

    private void Start()
    {
        _ferService = new FerService();
    }

    private void Update()
    {
        Color32[] image = WebcamManager.TakeSnapshot();
        FaceExpressionData ferData = _ferService.AnalyzeImage(image);
        
        if (!ferData.FilteredFaces.Any())
            return;
        
        DetectedFace detectedFace = ferData.FilteredFaces.First();
        DrawBoundingBoxes.Instance.DrawBoundingBox(ferData);
        Emoji.transform.localPosition = new Vector3(detectedFace.X, detectedFace.Y);
    }
}