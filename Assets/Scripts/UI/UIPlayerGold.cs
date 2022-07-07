using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerGold : MonoBehaviour
{
    [SerializeField] Text _text;
    GoldSource _goldSource;

    void Start()
    {
        // Get reference to the local player GoldSource
        TeamId localPlayerTeamId = GameManager.Instance.GetLocalClientTeamId();
        _goldSource = GameManager.Instance.FindComponentOwnedByTeam<GoldSource>(localPlayerTeamId);

        SetPlayerGoldData(_goldSource.GoldCount.Value, _goldSource.MaxGoldCount);

        _goldSource.GoldCount.OnValueChanged += OnGoldCountChanged;
    }

    void OnDestroy()
    {
        _goldSource.GoldCount.OnValueChanged -= OnGoldCountChanged;
    }

    void OnGoldCountChanged(float oldValue, float newValue)
    {
        SetPlayerGoldData(newValue,_goldSource.MaxGoldCount);
    }

    void SetPlayerGoldData(float value, float maxValue)
    {
        _text.text = Mathf.RoundToInt(value) + "/" + Mathf.RoundToInt(maxValue);
    }
}
