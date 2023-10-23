using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Utilities
{
    public abstract class MathHelper
    {
        public static IEnumerator SLerp(float from, float to, float duration, Material material, int nameID)
        {
            float t = 0;
            while (t < duration)
            {
                float result = Mathf.SmoothStep(from, to, t / duration);
                t += Time.deltaTime;
                material.SetFloat(nameID, result);

                yield return new WaitForEndOfFrame();
            }

            material.SetFloat(nameID, to);
        }
        
        public static async Task SLerpAsync(float from, float to, float duration, Material material, int nameID)
        {
            float t = 0;
            while (t < duration)
            {
                try
                {
                    float result = Mathf.SmoothStep(from, to, t / duration);
                    t += Time.deltaTime;
                    material.SetFloat(nameID, result);

                    await Task.Yield();
                }
                catch (Exception e)
                {
                    return;
                }
            }

            // ReSharper disable once Unity.PerformanceCriticalCodeNullComparison
            if (material != null)
                material.SetFloat(nameID, to);
        }
        
        public static IEnumerator SLerpTimeScale(float from, float to, float duration)
        {
            float t = 0;
            while (t < duration)
            {
                float result = Mathf.SmoothStep(from, to, t / duration);
                t += Time.unscaledDeltaTime;
                Time.timeScale = result;
                yield return new WaitForEndOfFrame();
            }

            Time.timeScale = to;
        }
    }
}