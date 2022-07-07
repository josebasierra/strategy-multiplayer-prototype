using UnityEngine;

/// <summary>
/// Move to destination but chase and attack enemies inside detection range. 
/// Once enemies defeated or outside detection range again, continue to commanded destination.
/// </summary>
public class AggresiveMoveCommand : ICommand
{
    Commandable _commandable;
    Vector3 _destination;

    NavMovement _commandableMovement;
    MeleeAttack _commandableAttack;
    EnemyDetector _commandableDetector;

    Damageable _detectedDamagable;

    public AggresiveMoveCommand(Commandable commandable, Vector3 destination)
    {
        _commandable = commandable;
        _destination = destination;

        _commandable.TryGetComponent(out _commandableMovement);
        _commandable.TryGetComponent(out _commandableAttack);
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
            _commandableMovement.MoveTo(_destination);
        }

        // transition condition
        if (_commandableMovement.HasReached(_destination))
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
