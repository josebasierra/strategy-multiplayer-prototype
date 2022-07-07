using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyBot : MonoBehaviour
{
    [SerializeField] List<DummyUnit> _dummyUnits;
    [SerializeField] SpawnArea _spawnArea;
    [SerializeField] GoldSource _goldSource;
    [SerializeField] Transform _commandablesDestination;

    void OnEnable()
    {
        Commandable.OnCommandableSpawn += OnCommandableSpawn;
    }

    void OnDisable()
    {
        Commandable.OnCommandableSpawn -= OnCommandableSpawn;
    }

    void Update()
    {
        if (!TryGetAvailableDummyToSpawn(out DummyUnit dummyUnitToSpawn)) return;

        dummyUnitToSpawn.transform.position = GenerateSpawnPosition();

        if (dummyUnitToSpawn.CanSpawn(_spawnArea))
        {
            dummyUnitToSpawn.Spawn();
        }
        dummyUnitToSpawn.ResetPosition();
    }

    bool TryGetAvailableDummyToSpawn(out DummyUnit dummyToSpawn)
    {
        foreach(var dummyUnit in _dummyUnits)
        {
            if (dummyUnit != null && dummyUnit.gameObject.activeSelf && dummyUnit.GoldCost <= _goldSource.GoldCount.Value)
            {
                dummyToSpawn = dummyUnit;
                return true;
            }
        }

        dummyToSpawn = null;
        return false;
    }

    Vector3 GenerateSpawnPosition()
    {
        float xPosition = Random.Range(_spawnArea.Min.x, _spawnArea.Max.x);
        float zPosition = Random.Range(_spawnArea.Min.z, _spawnArea.Max.z);

        return new Vector3(xPosition, 0.3f, zPosition);
    }

    void OnCommandableSpawn(Commandable commandable)
    {
        if (commandable.GetComponent<Team>().Id != GetComponent<Team>().Id || !commandable.CommandableByPlayer) return;

        commandable.AggresiveMove(_commandablesDestination.position);
    }
}
