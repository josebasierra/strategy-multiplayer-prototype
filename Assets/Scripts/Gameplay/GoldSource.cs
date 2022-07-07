using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GoldSource : NetworkBehaviour
{
    [SerializeField] NetworkVariable<float> _goldCount;
    [SerializeField] float _maxGoldCount;
    [SerializeField] float _incrementPerSecond;

    public NetworkVariable<float> GoldCount => _goldCount;
    public float MaxGoldCount => _maxGoldCount;

    public void AddGold(int value)
    {
        if (!IsServer) return;

        _goldCount.Value = Mathf.Clamp(_goldCount.Value + value, 0, _maxGoldCount);
    }

    void Update()
    {
        if (!IsServer) return;

        if (GameManager.Instance.GameStatus.Value != GameManager.GameStatusType.Playing) return;

        _goldCount.Value = Mathf.Clamp(_goldCount.Value + _incrementPerSecond * Time.deltaTime, 0, _maxGoldCount);
    }
}
