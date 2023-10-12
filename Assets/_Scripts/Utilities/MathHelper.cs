using System.Collections;
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