using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] float _attackRange;
    [SerializeField] int _damage;

    [Header("Inner refs")]
    [SerializeField] NavMovement _movement;
    [SerializeField] Animator _animator;

    Damageable _damageableTarget = null;

    bool _stopAttackingOnAnimationEnd = false;
    bool _isAttacking = false;

    public bool IsAttacking => _isAttacking;

    readonly int _isAttackingHash = Animator.StringToHash("isAttacking");

    void Update()
    {
        if (_damageableTarget == null && _isAttacking) // stop attack if enemy target becomes null
        {
            StopAttack();
        }
        else if (_damageableTarget != null && _isAttacking) // look at target while attacking
        {
            transform.forward = (_damageableTarget.transform.position - transform.position).normalized;
        }
    }

    public bool IsInAttackRange(Damageable damageable)
    {
        Collider damageableCollider = damageable.GetComponent<Collider>();
        return Vector3.Distance(damageableCollider.ClosestPoint(transform.position), transform.position) <= _attackRange;
    }

    public void StartAttack(Damageable target)
    {
        if (_isAttacking) return;

        _damageableTarget = target;
        _animator.SetBool(_isAttackingHash, true);
        _isAttacking = true;

        _movement.Stop();  //interrupt movement on attack start
    }

    public void StopAttack(bool waitCurrentAttackToEnd = false)
    {
        if (waitCurrentAttackToEnd) 
        { 
            _stopAttackingOnAnimationEnd = true;
            return;
        }

        _damageableTarget = null;
        _animator.SetBool(_isAttackingHash, false);
        _isAttacking = false;
    }

    #region Animation Events

    public void OnAttackAnimationHit()
    {
        if (_damageableTarget == null) return;

        _damageableTarget.TakeDamage(_damage);
    }

    public void OnAttackAnimationEnd()
    {
        if (_stopAttackingOnAnimationEnd)
        {
            StopAttack();
            _stopAttackingOnAnimationEnd = false;
        }
    }

    #endregion
}
