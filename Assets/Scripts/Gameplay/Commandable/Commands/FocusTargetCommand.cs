using UnityEngine;

/// <summary>
/// Focus enemy target. Chase it and attack it if inside attack range.
/// </summary>
public class FocusTargetCommand : ICommand
{
    Commandable _commandable;
    Transform _target;

    NavMovement _commandableMovement;
    MeleeAttack _commandableAttack;

    public FocusTargetCommand(Commandable commandable, Transform target)
    {
        _commandable = commandable;
        _target = target;

        _commandable.TryGetComponent(out _commandableMovement);
        _commandable.TryGetComponent(out _commandableAttack);
    }

    public void Tick()
    {
        if (_target != null && _target.TryGetComponent(out Damageable damageable))
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
                    _commandableMovement.MoveTo(_target.position);
                }
            }
        }

        // transition condition
        if (_target == null)
        {
            Interrupt();
            _commandable.StayIdleServerRpc();
        }
    }

    public void Interrupt()
    {
        _commandableMovement.Stop();
        _commandableAttack.StopAttack();
    }
}
