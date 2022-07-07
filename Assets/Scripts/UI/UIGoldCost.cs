using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGoldCost : MonoBehaviour
{
    [SerializeField] RectTransform _rectTransform;
    [SerializeField] Text _text;

    public void SetCost(int value)
    {
        _text.text = value.ToString();
    }

    public void SetScreenPosition(Vector3 position)
    {
        _rectTransform.position = position;
    }
}
