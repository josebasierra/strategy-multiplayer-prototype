using System;
using Unity.Netcode;
using UnityEngine;

public class Commandable : NetworkBehaviour
{
    [SerializeField] bool _commandableByPlayer = true;
    [SerializeField] CommandViewer _commandViewer;

    public static event Action<Commandable> OnCommandableSpawn, OnCommandableDespawn;

    ICommand _currentCommand;

    public bool CommandableByPlayer => _commandableByPlayer;

#if UNITY_EDITOR
    public string command;
#endif

    public override void OnNetworkSpawn()
    {
        if (_currentCommand == null) _currentCommand = new IdleCommand(this);
        OnCommandableSpawn?.Invoke(this);
    }

    public override void OnNetworkDespawn()
    {
        OnCommandableDespawn?.Invoke(this);
    }

    void Update()
    {
        if (IsHost)
        {
#if UNITY_EDITOR
            command = _currentCommand?.GetType().ToString();
#endif
            _currentCommand.Tick();
        }
    }

    #region Client

    public void StayIdle()
    {
        _commandViewer.ViewIdleCommand();
        StayIdleServerRpc();
    }

    public void Move(Vector3 destination)
    {
        _commandViewer.ViewMoveCommand(destination);
        MoveServerRpc(destination);
    }

    public void AggresiveMove(Vector3 destination)
    {
        _commandViewer.ViewAggresiveMoveCommand(destination);
        AggresiveMoveServerRpc(destination);
    }

    public void FocusTarget(Transform target)
    {
        _commandViewer.ViewFocusTargetCommand(target);
        FocusTargetServerRpc(target.gameObject);
    }

    [ClientRpc]
    void DisplayIdleCommandClientRpc()
    {
        _commandViewer.ViewIdleCommand();
    }

    #endregion

    #region Server

    [ServerRpc(RequireOwnership = false)]
    public void StayIdleServerRpc()
    {
        _currentCommand?.Interrupt();
        _currentCommand = new IdleCommand(this);

        DisplayIdleCommandClientRpc(); 
    }

    [ServerRpc(RequireOwnership = false)]
    public void MoveServerRpc(Vector3 destination)
    {
        _currentCommand?.Interrupt();
        _currentCommand = new MoveCommand(this, destination);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AggresiveMoveServerRpc(Vector3 destination)
    {
        _currentCommand?.Interrupt();
        _currentCommand = new AggresiveMoveCommand(this, destination);
    }

    [ServerRpc(RequireOwnership = false)]
    public void FocusTargetServerRpc(NetworkObjectReference objectRef)
    {
         NetworkObject networkObject = objectRef;

        _currentCommand?.Interrupt();
        _currentCommand = new FocusTargetCommand(this, networkObject.transform);
    }

    #endregion
}
