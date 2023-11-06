using System;
using UnityEngine;

namespace Systems
{
    public class FaceExpressionLogger : MonoBehaviour
    {
        private const OVRPermissionsRequester.Permission FaceTrackingPermission =
            OVRPermissionsRequester.Permission.FaceTracking;
        
        private OVRPlugin.FaceState _currentFaceState;
        private bool _validExpressions;

        private void Start()
        {
            if (!OVRPermissionsRequester.IsPermissionGranted(FaceTrackingPermission))
                Debug.LogWarning($"[{nameof(OVRFaceExpressions)}] Failed to start face tracking.");
            if (!OVRPlugin.StartFaceTracking()) 
                Debug.LogWarning($"[{nameof(OVRFaceExpressions)}] Failed to start face tracking.");
        }

        public string GetFaceExpressionsAsJson()
        {
            if (!_validExpressions)
                return ""; 
                    
            string faceData = JsonUtility.ToJson(_currentFaceState);

            return faceData;
        }

        private void Update()
        {
            _validExpressions = OVRPlugin.GetFaceState(OVRPlugin.Step.Render, -1, ref _currentFaceState) && _currentFaceState.Status.IsValid;
        }

        private void CheckValidity()
        {
            if (!_validExpressions)
                throw new InvalidOperationException($"Face expressions are not valid at this time. Use {nameof(_validExpressions)} to check for validity.");
        }
    }
}