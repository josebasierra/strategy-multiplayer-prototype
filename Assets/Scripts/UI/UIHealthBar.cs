using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    [SerializeField] RectTransform _rectTransform;
    [SerializeField] Slider _slider;
    [SerializeField] Image _fillImage;

    public void SetColor(Color color)
    {
        _fillImage.color = color;
    }

    public void SetHealthValue(float value)
    {
        _slider.value = value;
    }

    public void SetScreenPosition(Vector3 position)
    {
        _rectTransform.position = position;
    }
}
