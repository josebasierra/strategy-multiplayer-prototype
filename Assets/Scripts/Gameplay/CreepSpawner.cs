using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CreepSpawner : NetworkBehaviour
{
    [SerializeField] GameObject _creepPrefab;
    [SerializeField] List<Transform> _spawnSpots;
    [SerializeField] float _spawnCooldown;
    [SerializeField] Transform _creepsDestination;

    float _currentCooldown;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer) enabled = false;
    }

    void Update()
    {
        if (!GameManager.Instance.IsPlaying) return;

        _currentCooldown -= Time.deltaTime;

        if (_currentCooldown <= 0)
        {
            _currentCooldown = _spawnCooldown;
            SpawnCreeps();
        }
    }

    void SpawnCreeps()
    {
        foreach (var spot in _spawnSpots)
        {
            var creep = Instantiate(_creepPrefab, spot.position, Quaternion.identity);
            creep.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
            creep.GetComponent<Commandable>().AggresiveMove(_creepsDestination.position);
        }
    }
}
