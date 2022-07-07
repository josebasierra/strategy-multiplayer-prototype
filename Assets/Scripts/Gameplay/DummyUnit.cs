using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Allows the client to visualize and spawn a Unit in the map.
/// </summary>
public class DummyUnit : NetworkBehaviour
{
    [Header("Config")]
    [SerializeField] GameObject _prefabUnitToSpawn;
    [SerializeField] int _goldCost = 150;
    [SerializeField] Material _invalidStateMat; //Material that will be shown to indicate that the unit cannot be spawned.

    [Header("Inner refs")]
    [SerializeField] Renderer _renderer;
    [SerializeField] Collider _collider;

    GoldSource _goldSource;

    Material _initialMat;
    Vector3 _initialPosition;

    GameObject _instantiatedUnit;

    public int GoldCost => _goldCost;

    void Start()
    {
        _goldSource = GameManager.Instance.FindComponentOwnedByTeam<GoldSource>(GetComponent<Team>().Id);

        _initialMat = _renderer.sharedMaterial;
        _initialPosition = transform.position;
    }

    public void UpdatePosition()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 500f, GameConstants.GroundLayerMask))
        {
            transform.position = hit.point;
        }
        else
        {
            transform.position = ray.origin + ray.direction.normalized * 15f;
        }
    }

    public void ResetPosition()
    {
        transform.position = _initialPosition;
    }

    public void UpdateVisuals(bool showValidState)
    {
        if (!showValidState && _renderer.sharedMaterial != _invalidStateMat)
        {
            _renderer.sharedMaterial = _invalidStateMat;
        }
        else if (showValidState && _renderer.sharedMaterial != _initialMat)
        {
            _renderer.sharedMaterial = _initialMat;
        }
    }

    public bool CanSpawn(SpawnArea spawnArea)
    {
        return spawnArea.Contains(transform.position) && !CheckIntersection() && _goldSource.GoldCount.Value >= _goldCost;
    }

    public void Spawn()
    {
        SpawnServerRpc(transform.position);
        this.gameObject.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnServerRpc(Vector3 spawnPosition)
    {
        _goldSource.AddGold(-_goldCost);

        GameObject unitGameobject = Instantiate(_prefabUnitToSpawn, spawnPosition, transform.rotation);
        unitGameobject.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);

        _instantiatedUnit = unitGameobject;
        _instantiatedUnit.GetComponent<Damageable>().OnDeath.AddListener(OnUnitDeath); 

        SetDummyActiveClientRpc(false);
    }

    void OnUnitDeath()
    {
        _instantiatedUnit.GetComponent<Damageable>().OnDeath.RemoveListener(OnUnitDeath);
        SetDummyActiveClientRpc(true);
    }

    [ClientRpc]
    void SetDummyActiveClientRpc(bool value)
    {
        this.gameObject.SetActive(value);
    }

    // Check if dummyUnit intersects with some gameplay entities or environment (default layer) objects.
    bool CheckIntersection()
    {
        Vector3 colliderExtents = _collider.bounds.extents;
        Vector3 intersectionBoxExtents = new Vector3(colliderExtents.x, 5f, colliderExtents.z);

        _collider.enabled = false;

        bool intersectsWithObject = Physics.CheckBox(
            transform.position,
            intersectionBoxExtents,
            Quaternion.identity,
            GameConstants.GameplayEntityLayerMask | 1); // defaultLayerMask = 1

        _collider.enabled = true;

        return intersectsWithObject;
    }
}