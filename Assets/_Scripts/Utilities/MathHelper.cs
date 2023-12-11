using System;
using System.Collections;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Provides mathematical helper functions, particularly for smooth interpolation.
    /// </summary>
    public abstract class MathHelper
    {
        /// <summary>
        /// Smoothly interpolates a material's float property over time.
        /// </summary>
        public static IEnumerator SmoothStepMaterial(float from, float to, float duration, Material material, int nameID)
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
        
        
        /// <summary>
        /// Smoothly interpolates the time scale over time.
        /// </summary>
        public static IEnumerator SmoothStepTimeScale(float from, float to, float duration)
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