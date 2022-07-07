using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnArea : NetworkBehaviour
{
    BoxCollider _boxCollider;
    Vector3 _min, _max;

    public Vector3 Min => _min;
    public Vector3 Max => _max;

    void Start()
    {
        gameObject.SetActive(false);

        if (!TryGetComponent(out _boxCollider))
        {
            Debug.LogWarning("Spawn area needs a box collider");
        }

        Vector3 point1 = transform.TransformPoint(_boxCollider.center - _boxCollider.size / 2f);
        Vector3 point2 = transform.TransformPoint(_boxCollider.center + _boxCollider.size / 2f);

        _min = Vector3.Min(point1, point2);
        _max = Vector3.Max(point1, point2);
    }

    public void SetAreaDisplay(bool value)
    {
        // TODO: Play some effect or animation to smooth out activation..

        gameObject.SetActive(value);
    }

    public bool Contains(Vector3 point)
    {
        return point.x >= _min.x && point.y >= _min.y && point.z >= _min.z && 
            point.x <= _max.x && point.y <= _max.y && point.z <= _max.z;
    }
}