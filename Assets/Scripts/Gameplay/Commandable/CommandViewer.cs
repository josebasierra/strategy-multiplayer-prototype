using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CommandViewer : MonoBehaviour
{
    [SerializeField] LineRenderer _lineRenderer;

    [SerializeField] Gradient _moveLineGradient;
    [SerializeField] Gradient _aggresiveMoveLineGradient;
    [SerializeField] Gradient _focusTargetLineGradient;

    Transform _target;
    SelectionDisplay _targetSelectionDisplay;

    void Start()
    {
        if (GetComponentInParent<Team>().Id == GameManager.Instance.GetLocalClientTeamId())
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }

        Debug.Assert(_lineRenderer.positionCount == 2);
        _lineRenderer.enabled = false;
    }

    void Update()
    {
        if (_lineRenderer.enabled)
        {
            _lineRenderer.SetPosition(0, transform.position);
            if (_target != null)
            {
                _lineRenderer.SetPosition(1, _target.position);
            }
        }
    }

    void OnDestroy()
    {
        TryUnassignTarget();
    }

    public void ViewIdleCommand()
    {
        TryUnassignTarget();
        _lineRenderer.enabled = false;
    }

    public void ViewAggresiveMoveCommand(Vector3 destination)
    {
        TryUnassignTarget();
        SetupLineRenderer(destination, _aggresiveMoveLineGradient);
    }

    public void ViewMoveCommand(Vector3 destination)
    {
        TryUnassignTarget();
        SetupLineRenderer(destination, _moveLineGradient);
    }

    public void ViewFocusTargetCommand(Transform target)
    {
        if (_target == target) return;

        TryUnassignTarget();
        AssignTarget(target);
        SetupLineRenderer(target.position, _focusTargetLineGradient);
    }

    void SetupLineRenderer(Vector3 endPoint, Gradient colorGradient)
    {
        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(1, endPoint);
        _lineRenderer.colorGradient = colorGradient;
    }

    void AssignTarget(Transform target)
    {
        _target = target;
        _targetSelectionDisplay = target.GetComponent<SelectionDisplay>();
        _targetSelectionDisplay.AddTargeter();
    }

    void TryUnassignTarget()
    {
        if (_target != null && _targetSelectionDisplay != null)
        {
            _targetSelectionDisplay.RemoveTargeter();
        }
        _target = null;
        _targetSelectionDisplay = null;
    }
}
