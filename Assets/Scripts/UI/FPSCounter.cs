using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textFPS;
    float _elapsedTime = 0.5f;

    [Tooltip("Número de decimales. Utilización de un Start(), no se actualizará frame a frame.")]
    [SerializeField, Range(0, 5)] int _decimals = 0;
    string _decimalString;

    void Start()
    {
        _decimalString = "F" + _decimals;
    }

    void Update()
    {
        if (_textFPS.gameObject.activeSelf)
        {
            _elapsedTime += Time.unscaledDeltaTime;

            if (_elapsedTime >= 0.5f)
            {
                // Calcular FPS Promedio
                float FPSValue = 1f / Time.unscaledDeltaTime;

                // Actualizar el Texto de FPS
                _textFPS.text = "FPS " + FPSValue.ToString(_decimalString);
                
                _elapsedTime = 0f;
            }
        }
    }

    public void Show()
    {
        _textFPS.gameObject.SetActive(!_textFPS.gameObject.activeSelf);
    }
}