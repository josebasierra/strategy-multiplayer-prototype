using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField] GameObject _hostView, _clientView;

    GameObject _activeView;
    GameObject _player0Text, _player1Text;

    public void Init(bool showHostView)
    {
        _hostView.SetActive(showHostView);
        _clientView.SetActive(!showHostView);

        if (showHostView) _activeView = _hostView;
        else _activeView = _clientView;

        _player0Text = _activeView.transform.Find("Player0Text").gameObject;
        _player1Text = _activeView.transform.Find("Player1Text").gameObject;

        _player0Text.SetActive(false);
        _player1Text.SetActive(false);
    }

    void Update()
    {
        if (RoomManager.Instance.ConnectedPlayersCount <= 1)
        {
            _player0Text.SetActive(true);
            _player1Text.SetActive(false);
        }
        else
        {
            _player0Text.SetActive(true);
            _player1Text.SetActive(true);
        }
    }
}
