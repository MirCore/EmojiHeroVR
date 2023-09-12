using System;
using UnityEngine;

namespace Manager
{
    public class UIManager : MonoBehaviour
    {
        
        public void OnStartButtonPressed()
        {
            GameManager.Instance.OnStartButtonPressed();
        }
    }
}

public enum UIType
{
    Default,
    Start,
    Stop,
    Pause
} 