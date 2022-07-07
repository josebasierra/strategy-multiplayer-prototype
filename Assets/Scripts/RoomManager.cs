using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : NetworkBehaviour
{
    [SerializeField] int _maxPlayersCount = 2;

    const ushort _defaultPort = 7777;

    NetworkVariable<int> _connectedPlayersCount = new NetworkVariable<int>();
    public int ConnectedPlayersCount => _connectedPlayersCount.Value;

    public static RoomManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    void OnDestroy()
    {
        if (NetworkManager.Singleton == null) return;

        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }

    public void Setup(string ip, ushort port = _defaultPort)
    {
        StartCoroutine(StartSetup(ip, port));
    }

    public void Join(string ip, ushort port = _defaultPort)
    {
        StartCoroutine(StartJoining(ip, port));
    }

    public void Leave()
    {
        StartCoroutine(StartLeaving());
    }

    IEnumerator StartSetup(string ip, ushort port)
    {
        NetworkManager.Singleton.Shutdown();
        while (NetworkManager.Singleton.ShutdownInProgress) yield return null;

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

        var utp = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        utp.SetConnectionData(ip, port);

        try
        {
            NetworkManager.Singleton.StartHost();
            _connectedPlayersCount.Value = 1; // Host does not trigger OnClientConnected callback, although Host is Server+Client...
            NetworkManager.Singleton.SceneManager.LoadScene("3_CombatScene", LoadSceneMode.Single);
        }
        catch (Exception e)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
            Debug.Log(e);
        }
    }

    IEnumerator StartJoining(string ip, ushort port)
    {
        NetworkManager.Singleton.Shutdown();
        while (NetworkManager.Singleton.ShutdownInProgress) yield return null;

        //TODO: Use password ?
        NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(" ");

        var utp = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        utp.SetConnectionData(ip, port);

        NetworkManager.Singleton.StartClient();
    }

    IEnumerator StartLeaving()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        }

        NetworkManager.Singleton.Shutdown();
        while (NetworkManager.Singleton.ShutdownInProgress) yield return null;

        SceneManager.LoadScene("2_MenuScene");
    }

    void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
    {
        //string clientInputPassword = Encoding.ASCII.GetString(connectionData);
        bool approveConnection;

        if (_connectedPlayersCount.Value >= _maxPlayersCount)
        {
            approveConnection = false;
        }
        else
        {
            approveConnection = true;
        }

        callback(false, null, approveConnection, null, null);
    }

    void HandleClientConnected(ulong clientId)
    {
        if (IsHost)
        {
            _connectedPlayersCount.Value++;
            Debug.Log("Client connected");
        }
    }

    void HandleClientDisconnect(ulong clientId)
    {
        if (IsHost)
        {
            _connectedPlayersCount.Value--;
            Debug.Log("Client disconnected");
        }
        if (clientId == NetworkManager.Singleton.LocalClientId || clientId == NetworkManager.ServerClientId)
        {
            SceneManager.LoadScene("2_MenuScene", LoadSceneMode.Single);
        }
    }
}

