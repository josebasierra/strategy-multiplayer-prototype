using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UIHealthBarHandler : MonoBehaviour
{
    [SerializeField] GameObject _healthBarPrefabAsAlly;
    [SerializeField] GameObject _healthBarPrefabAsEnemy;
    [SerializeField] Vector3 _screenPositionOffset = new Vector3(0, -25f, 0);

    UIHealthBar _healthBar;
    Canvas _mainCanvas;
    Damageable _damageable;

    void Start()
    {
        _mainCanvas = FindObjectOfType<Canvas>();
        _damageable = GetComponentInParent<Damageable>();

        if (GameManager.Instance.GetLocalClientTeamId() == _damageable.GetComponent<Team>().Id)
        {
            _healthBar = Instantiate(_healthBarPrefabAsAlly, _mainCanvas.transform).GetComponent<UIHealthBar>();
        }
        else
        {
            _healthBar = Instantiate(_healthBarPrefabAsEnemy, _mainCanvas.transform).GetComponent<UIHealthBar>();
        }

        _healthBar.SetHealthValue(_damageable.CurrentHealth / _damageable.MaxHealth);
        _healthBar.SetScreenPosition(Camera.main.WorldToScreenPoint(transform.position) + _screenPositionOffset);
    }

    void OnDestroy()
    {
        if (_healthBar != null)
            Destroy(_healthBar.gameObject);
    }

    void Update()
    {
        _healthBar.SetHealthValue(_damageable.CurrentHealth / _damageable.MaxHealth);
        _healthBar.SetScreenPosition(Camera.main.WorldToScreenPoint(transform.position) + _screenPositionOffset);
    }
}
