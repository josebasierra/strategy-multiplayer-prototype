using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandInputController : MonoBehaviour
{
    public void Tick(IReadOnlyCollection<Commandable> commandables)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetKey(KeyCode.A) && Input.GetKeyDown(KeyCode.Mouse1))
        {
            // Aggresive Movement to selected point

            if (Physics.Raycast(ray, out var hit, GameConstants.GroundLayerMask))
            {
                foreach (var commandable in commandables)
                {
                    commandable.AggresiveMove(hit.point);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            var hits = Physics.RaycastAll(ray, GameConstants.GameplayEntityLayerMask); // raycastALL in case there's more than one unit overlapping (e.g Ally, Enemy,...)
            var enemyDamageable = FindEnemyDamageable(hits);

            if (enemyDamageable == null)
            {
                // Non-Aggresive Movement to selected point

                if (Physics.Raycast(ray, out var hit, GameConstants.GroundLayerMask))
                {
                    foreach (var commandable in commandables)
                    {
                        commandable.Move(hit.point);
                    }
                }
            }
            else
            {
                // Focus/Chase selected enemy damageable 

                foreach (var commandable in commandables)
                {
                    commandable.FocusTarget(enemyDamageable.transform);
                }
            }
        }
    }

    Damageable FindEnemyDamageable(RaycastHit[] hits)
    {
        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent(out Damageable damageable))
            {
                var myTeam = GetComponent<Team>();
                var damageableTeam = damageable.GetComponent<Team>();

                if (myTeam.Id != damageableTeam.Id)
                    return damageable;
            }
        }
        return null;
    }
}
