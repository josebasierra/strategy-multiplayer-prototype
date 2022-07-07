using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXSpawner : MonoBehaviour
{
    [SerializeField] GameObject _FXPrefab;
    [SerializeField] float _duration;

    public void Spawn()
    {
        var effect = Instantiate(_FXPrefab, transform.position, Quaternion.identity);
        Destroy(effect, _duration);
    }
}
