using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleSelect : MonoBehaviour
    {
        [SerializeField] private bool IsOn = true;
        [SerializeField] private bool Select = true;
        private void OnEnable()
        {
            Toggle toggle = GetComponent<Toggle>();
            if (Select)
                toggle.Select();
            toggle.isOn = IsOn;
        }
    }
}