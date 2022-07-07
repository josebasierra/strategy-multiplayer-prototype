using UnityEngine;

/// <summary>
/// Do nothing unless an enemy appears inside detection range, then chase and attack him.
/// </summary>
public class IdleCommand : ICommand
{
    Commandable _commandable;

    MeleeAttack _commandableAttack;
    NavMovement _commandableMovement;
    EnemyDetector _commandableDetector;

    public IdleCommand(Commandable commandable)
    {
        _commandable = commandable;

        _commandable.TryGetComponent(out _commandableAttack);
        _commandable.TryGetComponent(out _commandableMovement);
        _commandable.TryGetComponent(out _commandableDetector);
    }

    public void Tick()
    {
        var damageable = _commandableDetector.GetNearestEnemyDamageableInDetectionRange();

        if (damageable != null)
        {
            if (_commandableAttack.IsInAttackRange(damageable))
            {
                _commandableAttack.StartAttack(damageable);
            }
            else
            {
                if (_commandableAttack.IsAttacking)
                {
                    _commandableAttack.StopAttack(waitCurrentAttackToEnd: true);
                }
                else
                {
                    _commandableMovement.MoveTo(damageable.transform.position);
                }
            }
        }
        else
        {
            _commandableAttack.StopAttack();
            _commandableMovement.Stop();
        }
    }

    public void Interrupt()
    {
        _commandableAttack.StopAttack();
        _commandableMovement.Stop();
    }
}
