using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMovement : MonoBehaviour
{
    public abstract void MoveTo(Vector3 destination);

    public abstract void Stop();

    public abstract bool HasReachedDestination();

    public abstract bool HasReached(Vector3 position);
}
