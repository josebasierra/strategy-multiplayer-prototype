using System;
using Unity.Netcode;
using UnityEngine;

public class SelectionDisplay : NetworkBehaviour
{
    [SerializeField] SpriteRenderer _selectionImage;
    [SerializeField] SpriteRenderer _previewImage;

    [SerializeField] Color _selectedColor;
    [SerializeField] Color _enemySelectedColor;

    [SerializeField] Color _previewColor;
    [SerializeField] Color _enemyPreviewColor;

    int _targetersCount = 0;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _selectionImage.enabled = false;
        _previewImage.enabled = false;

        bool isAlly = GetComponent<Team>().Id == GameManager.Instance.GetLocalClientTeamId();

        _selectionImage.color = isAlly ? _selectedColor : _enemySelectedColor;
        _previewImage.color = isAlly ? _previewColor : _enemyPreviewColor;
    }

    public void SetSelect(bool value)
    {
        if (value)
        {
            _selectionImage.enabled = true;
            
        }
        else
        {
            _selectionImage.enabled = false;
        }
    }

    public void SetPreview(bool value)
    {
        if (value)
        {
            _previewImage.enabled = true;
            
        }
        else
        {
            _previewImage.enabled = false;
        }
    }

    // Methods to keep track of units targeting/selecting, while at least one unit targeting this display, show selectionImage. 
    public void AddTargeter()
    {
        _targetersCount++;
        SetSelect(true);
    }

    public void RemoveTargeter()
    {
        _targetersCount = Mathf.Max(0, _targetersCount - 1);
        if (_targetersCount <= 0) SetSelect(false);
    }
}