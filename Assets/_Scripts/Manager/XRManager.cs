using Unity.XR.CoreUtils;
using UnityEngine;

namespace Manager
{
    public class XRManager : MonoBehaviour
    {
        [SerializeField] private XROrigin Origin;

        /// <summary>
        /// Recenters the XR view on the Arcade
        /// </summary>
        public void OnRecenterXRButtonPressed() => RecenterXR();
    
        /// <summary>
        /// Recenters the XR view on the Arcade
        /// </summary>
        private void RecenterXR()
        {
            Origin.MatchOriginUpCameraForward(Vector3.up, Vector3.forward);
            Origin.MoveCameraToWorldLocation(new Vector3(0, Origin.CameraInOriginSpaceHeight, 0));
        }
    }
}
