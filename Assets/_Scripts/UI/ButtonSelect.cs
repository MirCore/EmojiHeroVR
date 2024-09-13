using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonSelect : MonoBehaviour
    {
        private void OnEnable()
        {
            Button button = GetComponent<Button>();
            button.Select();
        }
    }
}
