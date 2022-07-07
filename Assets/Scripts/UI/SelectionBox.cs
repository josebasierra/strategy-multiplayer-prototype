using UnityEngine;

public class SelectionBox : MonoBehaviour
{
    [Tooltip("Anchor must be located on the bottom left corner")]
    [SerializeField] RectTransform _rectTransform;

    Vector2 _startPoint, _endPoint;
    Vector2 _min, _max;

    public void Start()
    {
        if (_rectTransform == null) TryGetComponent(out _rectTransform);

        _startPoint = _rectTransform.anchoredPosition - (_rectTransform.sizeDelta / 2f);
        _endPoint = _rectTransform.anchoredPosition + (_rectTransform.sizeDelta / 2f);
        _min = _startPoint;
        _max = _endPoint;
    }

    public void SetStartPoint(Vector2 startPoint)
    {
        _startPoint = startPoint;
        UpdateRectTransform();
    }

    public void SetEndPoint(Vector2 endPoint)
    {
        _endPoint = endPoint;
        UpdateRectTransform();
    }

    public bool Contains(Vector2 point)
    {
        return point.x > _min.x && point.x < _max.x && point.y > _min.y && point.y < _max.y;
    }

    public void SetActive(bool value)
    {
        _rectTransform.gameObject.SetActive(value);
    }

    void UpdateRectTransform()
    {
        _min = Vector2.Min(_startPoint, _endPoint);
        _max = Vector2.Max(_startPoint, _endPoint);

        _rectTransform.anchoredPosition = (_min + _max) / 2f;
        _rectTransform.sizeDelta = (_max - _min);
    }
}
