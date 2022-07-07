using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    [SerializeField] float _detectionRange;

    Team _ownerTeam;

    Collider[] _collidersBuffer;
    List<Damageable> _enemyDamageablesInDetectionRange;

    void Start()
    {
        _ownerTeam = GetComponent<Team>();
        _collidersBuffer = new Collider[20];
        _enemyDamageablesInDetectionRange = new List<Damageable>();
    }

    public Damageable GetNearestEnemyDamageableInDetectionRange()
    {
        UpdateEnemyDamageablesInDetectionRange();

        if (_enemyDamageablesInDetectionRange.Count <= 0) return null;

        Damageable nearestDamageable = _enemyDamageablesInDetectionRange[0];
        float shortestDistance = Vector3.Distance(transform.position, nearestDamageable.transform.position);

        for (int i = 1; i < _enemyDamageablesInDetectionRange.Count; i++)
        {
            Vector3 position = _enemyDamageablesInDetectionRange[i].transform.position;
            float distance = Vector3.Distance(transform.position, position);

            if (distance < shortestDistance)
            {
                nearestDamageable = _enemyDamageablesInDetectionRange[i];
                shortestDistance = distance;
            }
        }

        return nearestDamageable;
    }

    // TODO: Keep track of damageable instances in some manager instead of using OverlapSphere?
    // Overlap sphere will scale better with more units (is probably between O(logn) and O(1)), while iterating over entities
    // in manager would have linear cost O(n)
    void UpdateEnemyDamageablesInDetectionRange()
    {
        _enemyDamageablesInDetectionRange.Clear();

        // reset colliders buffer
        for (int i = 0; i < _collidersBuffer.Length; i++)
        {
            _collidersBuffer[i] = null;
        }

        //obtain colliders in detection range
        int collidersCount = Physics.OverlapSphereNonAlloc(transform.position, _detectionRange, _collidersBuffer, GameConstants.GameplayEntityLayerMask);

        // obtain enemy damageables
        for (int i = 0; i < collidersCount; i++)
        {
            var collider = _collidersBuffer[i];
            if (collider.TryGetComponent(out Damageable damageable) && IsVisible(damageable))
            {
                if (collider.GetComponent<Team>().Id != _ownerTeam.Id)
                {
                    _enemyDamageablesInDetectionRange.Add(damageable);
                }
            }
        }
    }

    bool IsVisible(Damageable damageable)
    {
        Vector3 v = damageable.transform.position - transform.position;
        Ray ray = new Ray(transform.position, v.normalized);

        if (Physics.Raycast(ray, v.magnitude, GameConstants.DefaultLayerMask)) return false;

        return true;
    }
}
