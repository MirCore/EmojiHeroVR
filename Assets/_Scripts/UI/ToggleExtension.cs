using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ToggleExtension : MonoBehaviour
    {
        private Toggle _toggle;
        private Color _defaultNormal;
        private Color _defaultHighlighted;
        private Color _defaultSelected;
        private void OnEnable()
        {
            _toggle = GetComponent<Toggle>();
            if (_defaultNormal == new Color())
                _defaultNormal = _toggle.colors.normalColor;
            if (_defaultHighlighted == new Color())
                _defaultHighlighted = _toggle.colors.highlightedColor;
            _defaultSelected = _toggle.colors.selectedColor;
            _toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private void OnToggleValueChanged(bool isOn)
        {
            ColorBlock cb = _toggle.colors;
            if (isOn)
            {
                cb.normalColor = _defaultSelected;
                cb.highlightedColor = _defaultSelected;
            }
            else
            {
                cb.normalColor = _defaultNormal;
                cb.highlightedColor = _defaultHighlighted;
            }
            _toggle.colors = cb;
        }
    }
}
