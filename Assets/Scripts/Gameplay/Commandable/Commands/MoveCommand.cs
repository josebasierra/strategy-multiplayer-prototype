using UnityEngine;

/// <summary>
/// Move to destination ignoring all threats
/// </summary>
public class MoveCommand : ICommand
{
    Commandable _commandable;
    Vector3 _destination;

    NavMovement _commandableMovement;

    public MoveCommand(Commandable commandable, Vector3 destination)
    {
        _commandable = commandable;
        _destination = destination;

        _commandable.TryGetComponent(out _commandableMovement);
    }

    public void Tick()
    {
        _commandableMovement.MoveTo(_destination);

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
    }
}
