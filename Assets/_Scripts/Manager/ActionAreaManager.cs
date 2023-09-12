using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using Manager;
using UnityEngine;

public class ActionAreaManager : MonoBehaviour
{
    private Material _material;
    private int _originShaderID;
    private int _bottomColorShaderID;
    [SerializeField] private float ShaderRampDuration = 1;
    [SerializeField] private float LossColorRampDuration = 1;
    [SerializeField] private Color LossColor = Color.red;
    [SerializeField] private Color ResetColor = Color.yellow;
    [SerializeField] private float ResetColorRampDuration = 0.5f;
    [SerializeField] private float LowGradientPosition = -0.7f;
    [SerializeField] private float HighGradientPosition = -0.3f;

    private void Start()
    {
        _material = GetComponent<Renderer>().material;
        _originShaderID = Shader.PropertyToID("_Origin");
        _bottomColorShaderID = Shader.PropertyToID("_BottomColor");
        _material.SetFloat(_originShaderID, -0.3f);
        _material.SetColor(_bottomColorShaderID, ResetColor);
    }
    
    private void Awake()
    {
        EventManager.OnEmoteEnteredArea += OnEmoteEnteredAreaCallback;
        EventManager.OnEmoteExitedArea += OnEmoteExitedAreaCallback;
    }

    private void OnDestroy()
    {
        EventManager.OnEmoteEnteredArea -= OnEmoteEnteredAreaCallback;
        EventManager.OnEmoteExitedArea -= OnEmoteExitedAreaCallback;
    }

    private void OnEmoteEnteredAreaCallback(EEmote emote)
    {
        StartCoroutine(RampShader(HighGradientPosition));
        StartCoroutine(RampColor(LossColor, LossColorRampDuration));
    }

    private void OnEmoteExitedAreaCallback()
    {
        StopAllCoroutines();
        StartCoroutine(RampShader(LowGradientPosition));
        StartCoroutine(RampColor(ResetColor, ResetColorRampDuration));
    }

    private IEnumerator RampShader(float end)
    {
        float time = 0;
        float start = _material.GetFloat(_originShaderID);
        while (time < ShaderRampDuration)
        {
            _material.SetFloat(_originShaderID, Mathf.SmoothStep(start, end, time / ShaderRampDuration));
            time += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator RampColor(Color end, float duration)
    {
        float time = 0;
        Color start = _material.GetColor(_bottomColorShaderID);
        while (time < duration)
        {
            _material.SetColor(_bottomColorShaderID, Color.Lerp(start, end, time / duration));
            time += Time.deltaTime;
            yield return null;
        }
    }
}
