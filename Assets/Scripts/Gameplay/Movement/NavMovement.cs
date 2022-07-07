using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMovement : BaseMovement
{
    [Header("Config")]
    [SerializeField] float speed = 3f;

    [Header("Inner refs")]
    [SerializeField] NavMeshAgent _navMeshAgent;
    [SerializeField] Animator _animator;

    readonly int _isMovingHash = Animator.StringToHash("isMoving");

    void Start()
    {
        _navMeshAgent.speed = speed;
    }

    public override void MoveTo(Vector3 destination)
    {
        if (destination == _navMeshAgent.destination) return;

        _navMeshAgent.isStopped = false;
        _navMeshAgent.destination = destination;

        _animator.SetBool(_isMovingHash, true);
    }

    public override void Stop()
    {
        _navMeshAgent.isStopped = true;

        _animator.SetBool(_isMovingHash, false);
    }

    public override bool HasReachedDestination()
    {
        return HasReached(_navMeshAgent.destination);
    }

    public override bool HasReached(Vector3 position)
    {
        Vector3 agentPosition = transform.position;

        position.y = 0;
        agentPosition.y = 0;

        float remainingDistance = Vector3.Distance(position, agentPosition);
        return remainingDistance < _navMeshAgent.stoppingDistance;

        // NOTE: NavMeshAgent.remainingDistance only returns correct distance when at the last segment of the path.
    }
}
