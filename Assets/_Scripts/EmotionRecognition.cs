using Unity.Sentis;
using UnityEngine;
using UnityEngine.Profiling;
using Utilities;

public class EmotionRecognition : Singleton<EmotionRecognition>
{
    [SerializeField] private ModelAsset OnnxModel;

    private Worker _engine;

    private static readonly BackendType BackendType = BackendType.GPUCompute;

    private const int ImageWidth = 260;

    private Tensor _inputTensor;


    private void Start()
    {
        Model model = ModelLoader.Load(OnnxModel);

        _engine = new Worker(model, BackendType);
    }

    private Probabilities ExecuteModel(Texture drawableTexture)
    {
        _inputTensor?.Dispose();

        _inputTensor = TextureConverter.ToTensor(drawableTexture, ImageWidth, ImageWidth, 3);

        _engine.Schedule(_inputTensor);
        
        Tensor<float> resultOutput = _engine.PeekOutput() as Tensor<float>;

        var result = resultOutput.ReadbackAndClone();
        
        // Assuming that the model outputs one set of probabilities for one image
        // and that the output tensor shape is [1, number_of_emotions]
        int numEmotions = result.shape[1]; 

        Probabilities ferProbabilities = new();
        for (int i = 0; i < numEmotions; i++)
        {
            float probability = result[0, i]; // Access the probability for each emotion

            // Assign the probability to the corresponding field in the Probabilities struct
            switch (i)
            {
                case 0:
                    ferProbabilities.anger = probability;
                    break;
                case 1:
                    ferProbabilities.disgust = probability;
                    break;
                case 2:
                    ferProbabilities.fear = probability;
                    break;
                case 3:
                    ferProbabilities.happiness = probability;
                    break;
                case 4:
                    ferProbabilities.neutral = probability;
                    break;
                case 5:
                    ferProbabilities.sadness = probability;
                    break;
                case 6:
                    ferProbabilities.surprise = probability;
                    break;
            }
        }

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
