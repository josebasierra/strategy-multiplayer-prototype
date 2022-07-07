using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public enum GameStatusType { WaitingToStart, Playing, Completed }

    [SerializeField] StartMenu _startMenu;
    [SerializeField] DefeatMenu _defeatMenu;

    [SerializeField] PlayerController _player0Controller;
    [SerializeField] PlayerController _player1Controller;
    [SerializeField] DummyBot _bot;

    PlayerController _localPlayerController;

    public static GameManager Instance;
    public NetworkVariable<GameStatusType> GameStatus = new NetworkVariable<GameStatusType>(value: GameStatusType.WaitingToStart);
    public bool IsPlaying => GameStatus.Value == GameStatusType.Playing;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        InitLocalPlayerController();
        InitBot();
        InitUI();
    }

    void Update()
    {
        // TODO: Subscribe to GameStatus.OnValueChanged instead
        if (GameStatus.Value == GameStatusType.WaitingToStart)
        {
            _startMenu.gameObject.SetActive(true);
        }
        if (GameStatus.Value == GameStatusType.Playing)
        {
            _startMenu.gameObject.SetActive(false);
            _localPlayerController.enabled = true;
            if (RoomManager.Instance.ConnectedPlayersCount <= 1)
            {
                _bot.enabled = true;
            }
        }
        if (GameStatus.Value == GameStatusType.Completed)
        {
            _defeatMenu.gameObject.SetActive(true);
            _localPlayerController.enabled = false;
            _bot.enabled = false;
        }
    }

    void InitLocalPlayerController()
    {
        _player0Controller.gameObject.SetActive(false);
        _player1Controller.gameObject.SetActive(false);

        if (IsHost)
        {
            _localPlayerController = _player0Controller;
        }
        else
        {
            _localPlayerController = _player1Controller;
        }

        _localPlayerController.gameObject.SetActive(true); // Camera is a child of the player object, always keep one active.
        _localPlayerController.enabled = false;
    }

    void InitBot()
    {
        _bot.gameObject.SetActive(true);
        _bot.enabled = false;
    }

    void InitUI()
    {
        _startMenu.gameObject.SetActive(false);
        _defeatMenu.gameObject.SetActive(false);

        _startMenu.Init(IsHost);
        _defeatMenu.Init(IsHost);
    }

    public void StartGame()
    {
        if (!IsHost) return;

        GameStatus.Value = GameStatusType.Playing;
    }

    public void CompleteGame(TeamId defeatedTeamId)
    {
        if (!IsHost) return;
        if (!IsPlaying) return;

        GameStatus.Value = GameStatusType.Completed;

        SetDefeatedTeamIdClientRpc(defeatedTeamId);
    }

    public void CompleteGame(int defeatedTeamId)
    {
        CompleteGame((TeamId)defeatedTeamId);
    }

    public void RestartGame()
    {
        if (!IsHost) return;

        NetworkManager.Singleton.SceneManager.LoadScene("3_CombatScene", LoadSceneMode.Single);
    }

    public void ReturnToMainMenu()
    {
        RoomManager.Instance.Leave();
    }

    public TeamId GetLocalClientTeamId()
    {
        if (NetworkManager.Singleton.LocalClientId == 0) return TeamId.Team0;
        else return TeamId.Team1;
    }
    
    public T FindComponentOwnedByTeam<T>(TeamId teamId, bool includeInactive = false) where T: MonoBehaviour
    {
        var components = FindObjectsOfType<T>(includeInactive);
        foreach (var component in components)
        {
            if (component.TryGetComponent(out Team team) && team.Id == teamId)
            {
                return component;
            }
        }
        return null;
    }

    [ClientRpc]
    void SetDefeatedTeamIdClientRpc(TeamId defeatedTeamId)
    {
        _defeatMenu.SetDefeatedTeam(defeatedTeamId);
    }
}

