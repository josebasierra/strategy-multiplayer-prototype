using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionInputController : MonoBehaviour
{
    enum SelectionMode { Default, Additive }; 

    [SerializeField] SelectionBox _selectionBox;

    List<Commandable> _possibleSelection; // commandables that can be selected (same team and not controlled by AI)
    HashSet<Commandable> _currentSelection;
    HashSet<Commandable> _confirmedSelection;

    bool _isSelecting = false;
    SelectionDisplay _currentPreviewUnderMouse = null;

    public bool IsSelecting => _isSelecting;
    public IReadOnlyCollection<Commandable> Selection => _confirmedSelection;

    void Awake()
    {
        _possibleSelection = new List<Commandable>();
        _currentSelection = new HashSet<Commandable>();
        _confirmedSelection = new HashSet<Commandable>();

        Commandable.OnCommandableSpawn += OnCommandableEnabled;
        Commandable.OnCommandableDespawn += OnCommandableDisabled;

        _selectionBox.SetActive(false);
    }

    void OnDestroy()
    {
        Commandable.OnCommandableSpawn -= OnCommandableEnabled;
        Commandable.OnCommandableDespawn -= OnCommandableDisabled;
    }

    void OnCommandableEnabled(Commandable commandable)
    {
        if (commandable.GetComponent<Team>().Id != GetComponent<Team>().Id || !commandable.CommandableByPlayer) return;

        _possibleSelection.Add(commandable);
    }

    void OnCommandableDisabled(Commandable commandable)
    {
        _possibleSelection.Remove(commandable);
        if (_confirmedSelection.Contains(commandable)) _confirmedSelection.Remove(commandable);
    }

    public void ResetSelection()
    {
        ClearCurrentSelection();
        ClearConfirmedSelection();

        _isSelecting = false;
        _selectionBox.SetActive(false);
    }

    public void Tick()
    {
        SelectionMode selectionMode = Input.GetKey(KeyCode.LeftShift) ? SelectionMode.Additive : SelectionMode.Default;

        if (Input.GetKey(KeyCode.Mouse0))
        {
            // clear previous state
            ClearCurrentSelection();
            if (selectionMode == SelectionMode.Default) //We keep confirmed selection if in Additive mode
            {
                ClearConfirmedSelection();
            }

            // Update selection with new box
            _isSelecting = true;
            _selectionBox.SetActive(true);
            UpdateSelectionBox();

            SelectInsideSelectionBox();
            PreviewCurrentSelection();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0) && _isSelecting)
        {
            // Confirm Selection
            _isSelecting = false;
            _selectionBox.SetActive(false);

            if (_currentSelection.Count == 0) //optional, just to improve input respone/feeling...
            {
                var commandable = GetCommandableByPlayerUnderMouse();
                if (commandable != null) _currentSelection.Add(commandable);
            }

            ConfirmAndViewSelection();
            ClearCurrentSelection();  
        }

        // Show feedback before selecting something
        PreviewUnderMouse();    
    }

    void UpdateSelectionBox()
    {
        Vector2 mousePosition = Input.mousePosition;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _selectionBox.SetStartPoint(mousePosition);
            _selectionBox.SetActive(true);
        }
        _selectionBox.SetEndPoint(mousePosition);
    }

    Commandable GetCommandableByPlayerUnderMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) return null;
        if (!hit.collider.TryGetComponent(out Commandable commandable) || !commandable.CommandableByPlayer) return null;
        if (!_possibleSelection.Contains(commandable)) return null;

        return commandable;
    }

    void SelectInsideSelectionBox()
    {
        foreach (var commandable in _possibleSelection)
        {
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(commandable.transform.position);
            if (_selectionBox.Contains(screenPosition))
            {
                _currentSelection.Add(commandable);
            }
        }
    }

    void PreviewCurrentSelection()
    {
        foreach (var commandable in _currentSelection)
        {
            commandable.GetComponent<SelectionDisplay>().SetPreview(true);
        }
    }

    void ConfirmAndViewSelection()
    {
        foreach (Commandable commandable in _currentSelection)
        {
            _confirmedSelection.Add(commandable);
            commandable.GetComponent<SelectionDisplay>().SetSelect(true);
        }
    }

    void ClearCurrentSelection()
    {
        foreach (Commandable commandable in _currentSelection)
        {
            commandable.GetComponent<SelectionDisplay>().SetPreview(false);
        }
        _currentSelection.Clear();
    }

    void ClearConfirmedSelection()
    {
        foreach (Commandable commandable in _confirmedSelection)
        {
            commandable.GetComponent<SelectionDisplay>().SetSelect(false);
        }
        _confirmedSelection.Clear();
    }

    void PreviewUnderMouse()
    {
        if (_currentPreviewUnderMouse != null)
        {
            _currentPreviewUnderMouse.SetPreview(false);
            _currentPreviewUnderMouse = null;
        }

        if (IsSelecting) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) return;
        if (!hit.collider.TryGetComponent(out SelectionDisplay selecDisplay)) return;
        if (selecDisplay.GetComponent<Team>().Id == GetComponent<Team>().Id) // Is from my team?
        {
            // we don't want to preview-select allied entities other heroes
            if (selecDisplay.TryGetComponent(out Commandable commandable) && commandable.CommandableByPlayer)
            {
                selecDisplay.SetPreview(true);
                _currentPreviewUnderMouse = selecDisplay;
            }
        }
        else if (selecDisplay.TryGetComponent(out Damageable _))
        {
            selecDisplay.SetPreview(true);
            _currentPreviewUnderMouse = selecDisplay;
        }
    }
}