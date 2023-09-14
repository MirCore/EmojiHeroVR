using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Webcam : MonoBehaviour
{
    [SerializeField] private List<WebCamTexture> _webcam = new();
    
    [SerializeField] private List<string> WebcamName;
    [SerializeField] private List<RawImage> Image;

    private void Start()
    {
        foreach (string webcamName in WebcamName)
        {
            _webcam.Add(new WebCamTexture(webcamName));
        }
        
        for (int i = 0; i < Image.Count; i++)
        {
            
            Image[i].texture = _webcam[i];
            _webcam[i].Play();
        }
        
        
    }
}
