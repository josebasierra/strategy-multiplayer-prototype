using UnityEngine;

public class SpawnInputController : MonoBehaviour
{
    [SerializeField] SpawnArea _spawnArea;

    bool _isDragging = false;
    DummyUnit _currentDummyUnit = null;

    public bool IsDragging => _isDragging;

    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !_isDragging)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 500f) && hit.collider.TryGetComponent(out _currentDummyUnit))
            {
                if (_currentDummyUnit.GetComponent<Team>().Id != GetComponent<Team>().Id)
                {
                    _currentDummyUnit = null;
                    return;
                }

                _isDragging = true;
                OnDragEnter();
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0) && _isDragging)
        {
            _isDragging = false;
            OnDragExit();
            _currentDummyUnit = null;
        }

        if (_isDragging) OnDrag();
    }

    void OnDragEnter()
    {
        _spawnArea.SetAreaDisplay(true);
    }

    void OnDrag()
    {
        _currentDummyUnit.UpdatePosition();

        bool canSpawn = _currentDummyUnit.CanSpawn(_spawnArea);
        _currentDummyUnit.UpdateVisuals(canSpawn);
    }

    void OnDragExit()
    {
        if (_currentDummyUnit.CanSpawn(_spawnArea))
        {
            _currentDummyUnit.Spawn();
        }
        _currentDummyUnit.ResetPosition();
        _currentDummyUnit.UpdateVisuals(true);

        _spawnArea.SetAreaDisplay(false);
    }
}


