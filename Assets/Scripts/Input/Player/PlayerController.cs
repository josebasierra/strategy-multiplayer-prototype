using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] SelectionInputController _selectionInputController;
    [SerializeField] SpawnInputController _spawnInputController;
    [SerializeField] CommandInputController _commandInputController;

    void Update()
    {
        _spawnInputController.Tick();
        if (!_spawnInputController.IsDragging)
        {
            _selectionInputController.Tick();
            _commandInputController.Tick(_selectionInputController.Selection);
        }
        else
        {
            _selectionInputController.ResetSelection();
        }
    }
}
