using System;
using UnityEngine;

namespace Systems
{
    public class FaceExpressionLogger : MonoBehaviour
    {
#if !OVRPLUGIN_UNSUPPORTED_PLATFORM
        private OVRPlugin.FaceState _currentFaceState;
#endif
        
        private bool _validExpressions;

        private void Start()
        {
#if !OVRPLUGIN_UNSUPPORTED_PLATFORM
            if (!OVRPlugin.StartFaceTracking()) 
                Debug.LogWarning($"[{nameof(OVRFaceExpressions)}] Failed to start face tracking.");
            if (!OVRPlugin.StartEyeTracking()) 
                Debug.LogWarning($"[{nameof(OVRFaceExpressions)}] Failed to start eye tracking.");
#endif
        }

        public string GetFaceExpressionsAsJson()
        {
#if !OVRPLUGIN_UNSUPPORTED_PLATFORM      
            if (_validExpressions)
                return JsonUtility.ToJson(_currentFaceState);
            Debug.LogWarning($"Face expression not valid.");
#endif
            return ""; 
        }

        private void Update()
        {
#if !OVRPLUGIN_UNSUPPORTED_PLATFORM
            _validExpressions = OVRPlugin.GetFaceState(OVRPlugin.Step.Render, -1, ref _currentFaceState) && _currentFaceState.Status.IsValid;
#endif
        }
    }
}