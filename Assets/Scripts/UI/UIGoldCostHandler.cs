using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGoldCostHandler : MonoBehaviour
{
    [SerializeField] GameObject _goldCostDisplayPrefab;
    [SerializeField] Vector3 _screenPositionOffset = new Vector3(0, 5f, 0);

    Canvas _mainCanvas;
    UIGoldCost _goldCostDisplay;

    void Start()
    {
        _mainCanvas = FindObjectOfType<Canvas>();
        _goldCostDisplay = Instantiate(_goldCostDisplayPrefab, _mainCanvas.transform).GetComponent<UIGoldCost>();

        _goldCostDisplay.SetCost(GetComponentInParent<DummyUnit>().GoldCost);
        _goldCostDisplay.SetScreenPosition(Camera.main.WorldToScreenPoint(transform.position) + _screenPositionOffset);
    }

    void OnEnable()
    {
        if (_goldCostDisplay != null) _goldCostDisplay.gameObject.SetActive(true);

    }
    void OnDisable()
    {
        if (_goldCostDisplay != null) _goldCostDisplay.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (_goldCostDisplay != null) Destroy(_goldCostDisplay.gameObject);
    }

    void Update()
    {
        _goldCostDisplay.SetScreenPosition(Camera.main.WorldToScreenPoint(transform.position) + _screenPositionOffset);
    }
}
