using Enums;
using Unity.Sentis;
using UnityEngine;
using UnityEngine.Profiling;
using Utilities;

public class EmotionRecognition : Singleton<EmotionRecognition>
{
    [SerializeField] private ModelAsset OnnxModel;

    private Worker _engine;

    [SerializeField] private int ImageWidth = 260;
    
    [SerializeField] private Texture Image;
    
    [SerializeField] private TensorLayout TensorLayout = TensorLayout.NCHW;
    
    [SerializeField] private EEmote[] Emotions = {EEmote.Anger, EEmote.Disgust, EEmote.Fear, EEmote.Happiness, EEmote.Neutral, EEmote.Sadness, EEmote.Surprise};

    private Tensor<float> _inputTensor;


    private void Start()
    {
        Model model = ModelLoader.Load(OnnxModel);

        _engine = new Worker(model, BackendType.GPUCompute);
    }

    private Probabilities ExecuteModel(Texture drawableTexture)
    {
        _inputTensor?.Dispose();

        if (Image)
            drawableTexture = Image;

        TextureTransform textureTransform = new TextureTransform().SetDimensions(ImageWidth, ImageWidth, 3).SetTensorLayout(TensorLayout);
        _inputTensor = TextureConverter.ToTensor(drawableTexture, textureTransform);
        
        _engine.Schedule(_inputTensor);
        
        Tensor<float> resultOutput = _engine.PeekOutput() as Tensor<float>;

        Tensor<float> result = resultOutput.ReadbackAndClone();
        
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
        
        resultOutput.Dispose(); // Dispose after you're done with it.

        return ferProbabilities;
    }
   
    // Clean up all our resources at the end of the session so we don't leave anything on the GPU or in memory:
    private void OnDestroy()
    {
        _inputTensor?.Dispose();
        _engine?.Dispose();
    }

    public Probabilities DetectEmotion(Texture2D face)
    {
        Profiler.BeginSample("DetectEmotion");
        
        Probabilities result = ExecuteModel(face);
        
        Profiler.EndSample();
        
        return result;
    }
}
