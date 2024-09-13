using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class ScrollViewExtension : MonoBehaviour
    {
        private EventSystem _eventSystem;
        private ScrollRect _scrollRect;
        private GameObject _target;
        private RectTransform _scrollRectTransform;

        private void Start()
        {
            _scrollRect = GetComponent<ScrollRect>();
            _eventSystem = EventSystem.current;
            _scrollRectTransform = _scrollRect.GetComponent<RectTransform>();
        }

        private void Update()
        {
            GameObject obj = _eventSystem.currentSelectedGameObject;
            
            if (!obj)
                return;
            if (obj == _target)
                return;
            if (!obj.transform.IsChildOf(transform))
                return;
            
            _target = obj;
            SnapTo(_target.transform);
        }

        private void SnapTo(Transform target)
        {
            Canvas.ForceUpdateCanvases();
            
            Vector2 contentPos = (Vector2)_scrollRect.transform.InverseTransformPoint( _scrollRect.content.position );
            Vector2 childPos = (Vector2)_scrollRect.transform.InverseTransformPoint( target.position );

            Vector2 height = _scrollRectTransform.sizeDelta;
            
            Vector2 endPos = contentPos - childPos + target.GetComponent<RectTransform>().sizeDelta - height;
            
            // If no horizontal scroll, then don't change contentPos.x
            if( !_scrollRect.horizontal ) endPos.x = contentPos.x;
            // If no vertical scroll, then don't change contentPos.y
            if( !_scrollRect.vertical ) endPos.y = contentPos.y;
            _scrollRect.content.anchoredPosition = endPos;
        }
    }
}
