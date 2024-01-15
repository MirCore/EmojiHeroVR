using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Sentis;
using UnityEngine;
using Utilities;

public class FaceDetection : Singleton<FaceDetection>
{
    [SerializeField] private ModelAsset OnnxModel;

    private IWorker _engine;

    private static readonly BackendType BackendType = BackendType.GPUCompute;


    private TensorFloat _inputTensor;

    private Ops _ops;

    private void Start()
    {
        Model model = ModelLoader.Load(OnnxModel);

        _engine = WorkerFactory.CreateWorker(BackendType, model);

        _ops = WorkerFactory.CreateOps(BackendType, null);
    }


    private Texture2D ExecuteModel(Texture2D texture)
    {
        _inputTensor?.Dispose();

        _inputTensor = TextureConverter.ToTensor(texture, 640, 480, 3);

        _engine.Execute(_inputTensor);
        
        // model has multiple output, so to know which output to get we need to specify which one we are referring to
        TensorFloat scores = _engine.PeekOutput("scores") as TensorFloat;
        TensorFloat boxes = _engine.PeekOutput("boxes") as TensorFloat;
        scores.MakeReadable();
        boxes.MakeReadable();
        
        int numDetections = boxes.shape[1]; // Assuming second dimension of the model is the number of detections
        
        float highestScore = 0;
        float[] highestScoreBox = new float[4];

        for (int i = 0; i < numDetections; i++)
        {
            float score = scores[0, i, 1]; // Modify this based on how scores are laid out
            
            if (!(score > highestScore))
                continue;
            
            highestScore = score;
            for (int j = 0; j < 4; j++)
            {
                highestScoreBox[j] = boxes[0, i, j]; // Extract each coordinate of the bounding box
            }
        }

        if (!(highestScore > 0.3f)) 
            return null;
        
        // Convert the bounding box to pixel coordinates
        int x = Mathf.FloorToInt(highestScoreBox[0] * texture.width);
        // Flip the y coordinate
        int y = Mathf.FloorToInt((1 - highestScoreBox[3]) * texture.height);
        int width = Mathf.FloorToInt(highestScoreBox[2] * texture.width) - x;
        // Calculate the height based on the flipped y
        int height = Mathf.FloorToInt((1 - highestScoreBox[1]) * texture.height) - y;

        // Ensure that the coordinates and dimensions are within the texture bounds
        x = Mathf.Clamp(x, 0, texture.width);
        y = Mathf.Clamp(y, 0, texture.height);
        width = Mathf.Clamp(width, 0, texture.width - x);
        height = Mathf.Clamp(height, 0, texture.height - y);
        

        // Create a temporary texture to hold the cropped image
        Texture2D tempTexture = new(width, height);
        Color[] pixels = texture.GetPixels(x, y, width, height);
        tempTexture.SetPixels(pixels);
        tempTexture.Apply();

        return tempTexture;
    }
    
    private List<DetectedFace> ExecuteModel(Texture2D texture, float scoreThreshold)
    {
        _inputTensor?.Dispose();

        _inputTensor = TextureConverter.ToTensor(texture, 640, 480, 3);

        _engine.Execute(_inputTensor);
        
        // model has multiple output, so to know which output to get we need to specify which one we are referring to
        TensorFloat scores = _engine.PeekOutput("scores") as TensorFloat;
        TensorFloat boxes = _engine.PeekOutput("boxes") as TensorFloat;
        scores.MakeReadable();
        boxes.MakeReadable();
        
        int numDetections = boxes.shape[1]; // Assuming second dimension of the model is the number of detections
        
        List<DetectedFace> detectedFaces = new();

        for (int i = 0; i < numDetections; i++)
        {
            float score = scores[0, i, 1];

            if (score < scoreThreshold)
                continue;
            
            DetectedFace df = new()
            {
                Score = score,
            };

            float[] box = new float[4];
            for (int j = 0; j < 4; j++)
            {
                box[j] = boxes[0, i, j]; 
            }
            
            ConvertDetectedFaceCoordinates(df, box, texture.width, texture.height); // Extract each coordinate of the bounding box
            
            detectedFaces.Add(df);
        }
        
        // Sort the list by score in descending order
        detectedFaces = detectedFaces.OrderByDescending(face => face.Score).ToList();


        return detectedFaces;
    }
    
    private static void ConvertDetectedFaceCoordinates(DetectedFace df, IReadOnlyList<float> box, int imageWidth, int imageHeight)
    {
        df.RelativeX = Mathf.Clamp(box[0], 0, 1);
        df.RelativeY = Mathf.Clamp(1 - box[3], 0, 1); // Flip the y coordinate
        df.RelativeWidth = Mathf.Clamp(box[2] - df.RelativeX, 0, 1);
        df.RelativeHeight = Mathf.Clamp((1 - box[1]) - df.RelativeY, 0, 1); // Flip the y coordinate
        
        // Convert the bounding box to pixel coordinates
        int x = (int)(df.RelativeX * imageWidth);
        int y = (int)(df.RelativeY * imageHeight);
        int width = (int)(df.RelativeWidth * imageWidth);
        int height = (int)(df.RelativeHeight * imageHeight);

        // Ensure that the coordinates and dimensions are within the texture bounds
        df.X = Mathf.Clamp(x, 0, imageWidth);
        df.Y = Mathf.Clamp(y, 0, imageHeight);
        df.Width = Mathf.Clamp(width, 0, imageWidth - x);
        df.Height = Mathf.Clamp(height, 0, imageHeight - y);
    }
    
    public List<DetectedFace> DetectFaces(Texture2D image, float scoreThreshold)
    {
        List<DetectedFace> detectedFaces = ExecuteModel(image, scoreThreshold);

        return detectedFaces;
    }


    public Texture2D DetectFace(Texture2D image, FerHandler ferHandler)
    {
        Texture2D result = ExecuteModel(image);

        if (result != null)
            return result;
        
        ferHandler.ProcessFerError(new Exception("FaceDetection Error"));
        return null;
    }
   
    // Clean up all our resources at the end of the session so we don't leave anything on the GPU or in memory:
    private void OnDestroy()
    {
        _inputTensor?.Dispose();
        _engine?.Dispose();
        _ops?.Dispose();
    }
}