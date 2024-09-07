using Enums;
using NUnit.Framework.Internal;
using Unity.Collections;
using Unity.Sentis;
using UnityEngine;
using UnityEngine.Profiling;
using Utilities;

public class EmotionRecognition : Singleton<EmotionRecognition>
{
    [SerializeField] private ModelAsset OnnxModel;

    private Worker _worker;

    [SerializeField] private int ImageWidth = 260;
    
    [SerializeField] private Texture Image;
    
    [SerializeField] private TensorLayout TensorLayout = TensorLayout.NCHW;
    
    [SerializeField] private ETextureMode TextureMode = ETextureMode.ZeroToOne;
    
    [SerializeField] private EEmote[] Emotions = {EEmote.Anger, EEmote.Disgust, EEmote.Fear, EEmote.Happiness, EEmote.Neutral, EEmote.Sadness, EEmote.Surprise};
    

    private void Start()
    {
        LoadAndConvertModel();
    }

    private void LoadAndConvertModel()
    {
        // Load the source model.
        Model model = ModelLoader.Load(OnnxModel);

        if (TextureMode == ETextureMode.ZeroToOne)
        {
            // Create worker to run the model.
            _worker = new Worker(model, BackendType.GPUCompute);
        }
        else
        {
            FunctionalGraph graph = new ();
            
            // Get the input functional tensor from the graph with input data type and shape matching that of the original model input.
            FunctionalTensor sRGB = graph.AddInput(model, 0);

            // Apply f(x) = x^(1/2.2) element-wise to transform from RGB to sRGB.
            //FunctionalTensor sRGB = Functional.Pow(rgb, Functional.Constant(1 / 2.2f));

            FunctionalTensor texture = TextureMode switch
            {
                // Apply f(x) = x * 255 element-wise to transform values from the range [0, 1] to the range [0, 255].
                ETextureMode.ZeroTo255 => sRGB * 255,
                // Apply f(x) = x * 2 - 1 element-wise to transform values from the range [0, 1] to the range [-1, 1].
                ETextureMode.MinusOneToOne => sRGB * 2 - 1,
                _ => null
            };

            // Apply the forward method of the source model to the transformed functional input and return the output.
            FunctionalTensor[] outputs = Functional.Forward(model, texture);
            
            // Compile the graph to return the final model.
            Model modifiedModel = graph.Compile(outputs);

            // Create worker to run the model.
            _worker = new Worker(modifiedModel, BackendType.GPUCompute);
        }
    }

    private Probabilities ExecuteModel(Texture drawableTexture)
    {
        if (Image)
            drawableTexture = Image;

        TextureTransform textureTransform = new TextureTransform().SetDimensions(ImageWidth, ImageWidth, 3).SetTensorLayout(TensorLayout);
        using Tensor<float> inputTensor = TextureConverter.ToTensor(drawableTexture, textureTransform);

        _worker.Schedule(inputTensor);
        
        Tensor<float> output = _worker.PeekOutput() as Tensor<float>;

        Tensor<float> result = output!.ReadbackAndClone();
        
        // Assuming that the model outputs one set of probabilities for one image
        // and that the output tensor shape is [1, number_of_emotions]
        int numEmotions = result.shape.length;

        //Debug.Log(result[0, 0] + " " + result[0, 1] + " " + result[0, 2] + " " + result[0, 3] + " " + result[0, 4] + " " + result[0, 5] + " " + result[0, 6]);

        Probabilities ferProbabilities = new();
        for (int i = 0; i < numEmotions; i++)
        {
            float probability = result[0, i]; // Access the probability for each emotion

            EEmote emotion = Emotions[i];
            
            // Assign the probability to the corresponding field in the Probabilities struct
            switch (emotion)
            {
                case EEmote.Anger:
                    ferProbabilities.anger = probability;
                    break;
                case EEmote.Disgust:
                    ferProbabilities.disgust = probability;
                    break;
                case EEmote.Fear:
                    ferProbabilities.fear = probability;
                    break;
                case EEmote.Happiness:
                    ferProbabilities.happiness = probability;
                    break;
                case EEmote.Neutral:
                    ferProbabilities.neutral = probability;
                    break;
                case EEmote.Sadness:
                    ferProbabilities.sadness = probability;
                    break;
                case EEmote.Surprise:
                    ferProbabilities.surprise = probability;
                    break;
            }
        }
        
        result.Dispose(); // Dispose after you're done with it.

        return ferProbabilities;
    }
   
    // Clean up all our resources at the end of the session so we don't leave anything on the GPU or in memory:
    private void OnDestroy()
    {
        _worker?.Dispose();
    }

    public Probabilities DetectEmotion(Texture2D face)
    {
        Profiler.BeginSample("DetectEmotion");
        
        Probabilities result = ExecuteModel(face);
        
        Profiler.EndSample();
        
        return result;
    }
}

internal enum ETextureMode
{
    ZeroToOne,      // 0 to 1
    MinusOneToOne,  // -1 to 1
    ZeroTo255       // 0 to 255
}
