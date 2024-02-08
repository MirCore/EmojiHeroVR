using UnityEngine;
using Utilities;

namespace Scriptables
{
    public class ScriptableDebugData : ScriptableObject
    {
        public int PendingFer;     // Current number of active REST POST requests.
        public int TotalFer;    // Total number of REST POST requests made.
        public double CurrentFerFPS;    // Frames per second calculated based on the time between the last two REST POST requests.
        public double SnapshotFPS;    // Frames per second of the snapshots.
        public Probabilities Probabilities;
    }
}
