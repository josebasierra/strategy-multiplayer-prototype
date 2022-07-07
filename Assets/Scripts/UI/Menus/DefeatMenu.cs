using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DefeatMenu : MonoBehaviour
{
    [SerializeField] GameObject _hostView, _clientView;

    GameObject _activeView;
    Text _playerDefeatedText;

    public void Init(bool showHostView)
    {
        _hostView.SetActive(showHostView);
        _clientView.SetActive(!showHostView);

        if (showHostView) _activeView = _hostView;
        else _activeView = _clientView;

        _playerDefeatedText = _activeView.transform.Find("PlayerDefeatedText").GetComponent<Text>();
    }

    public void SetDefeatedTeam(TeamId defeatedTeam)
    {
        _playerDefeatedText.text = defeatedTeam != TeamId.Team0 ?
            "Team 0 victory! \n Team 1 defeated..." : "Team 1 victory! \n Team 0 defeated...";
    }
}
