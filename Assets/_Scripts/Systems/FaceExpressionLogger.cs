using System;
using UnityEngine;

namespace Systems
{
    public class FaceExpressionLogger : MonoBehaviour
    {
#if OVR_IMPLEMENTED
        private const OVRPermissionsRequester.Permission FaceTrackingPermission = OVRPermissionsRequester.Permission.FaceTracking;
        
        private OVRPlugin.FaceState _currentFaceState;
#endif
        private bool _validExpressions;

        private void Start()
        {
#if OVR_IMPLEMENTED
            if (!OVRPermissionsRequester.IsPermissionGranted(FaceTrackingPermission))
                Debug.LogWarning($"[{nameof(OVRFaceExpressions)}] Failed to start face tracking.");
            if (!OVRPlugin.StartFaceTracking()) 
                Debug.LogWarning($"[{nameof(OVRFaceExpressions)}] Failed to start face tracking.");
#endif
        }

        public string GetFaceExpressionsAsJson()
        {
#if OVR_IMPLEMENTED      
            if (!_validExpressions)
                return faceData = JsonUtility.ToJson(_currentFaceState);
#endif
            
            return ""; 
        }

        private void Update()
        {
#if OVR_IMPLEMENTED
            _validExpressions = OVRPlugin.GetFaceState(OVRPlugin.Step.Render, -1, ref _currentFaceState) && _currentFaceState.Status.IsValid;
#endif
        }

        private void CheckValidity()
        {
            if (!_validExpressions)
                throw new InvalidOperationException($"Face expressions are not valid at this time. Use {nameof(_validExpressions)} to check for validity.");
        }
    }
}