using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class EventSystemExtension : MonoBehaviour
    {
        private GameObject _currentSelection;
        private EventSystem _eventSystem;

        private void Start()
        {
            _eventSystem = EventSystem.current;
        }

        private void Update()
        {
            GameObject currentSelection = _eventSystem.currentSelectedGameObject;
            if (!currentSelection)
                _eventSystem.SetSelectedGameObject(_currentSelection);
            else
                _currentSelection = currentSelection;
        }
    }
}
