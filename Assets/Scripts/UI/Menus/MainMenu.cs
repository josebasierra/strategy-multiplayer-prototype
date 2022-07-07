using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject _mainView;
    [SerializeField] GameObject _hostGameView;
    [SerializeField] GameObject _joinGameView;

    [SerializeField] InputField _inputFieldHostIp;
    [SerializeField] InputField _inputFieldJoinIp;

    void Start()
    {
        _mainView.SetActive(true);
        _hostGameView.SetActive(false);
        _joinGameView.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _mainView.SetActive(true);
            _hostGameView.SetActive(false);
            _joinGameView.SetActive(false);
        }
    }

    public void HostGame()
    {
        RoomManager.Instance.Setup(_inputFieldHostIp.text);
    }

    public void JoinGame()
    {
        RoomManager.Instance.Join(_inputFieldJoinIp.text);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
