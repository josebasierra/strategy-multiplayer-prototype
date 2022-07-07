using System.Collections;
using UnityEngine;

public abstract class BaseAttack : MonoBehaviour
{
    public abstract bool IsInAttackRange(Damageable damageable);

    public abstract bool StartAttack(Damageable damageable);

    public abstract void StopAttack(bool waitCurrentAttackToEnd = false);
}
