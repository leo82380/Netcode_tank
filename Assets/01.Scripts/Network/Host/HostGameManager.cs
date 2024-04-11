using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager : IDisposable
{
    private Allocation _allocation;
    private const int _maxConnections = 8;
    private string _joinCode;
    private string _lobbyId; // 내가 게임 만들고, 로비를 만들건데 그 아이디
    
    public NetworkServer NetServer { get; private set; }

    private void MakeNetworkserver()
    {
        NetServer = new NetworkServer(NetworkManager.Singleton);
    }
    
    public string JoinCode => _joinCode;

    public async Task StartHostAsync()
    {
        try
        {
            _allocation = await Relay.Instance.CreateAllocationAsync(_maxConnections);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

        try
        {
            _joinCode = await Relay.Instance.GetJoinCodeAsync(_allocation.AllocationId);
            Debug.Log($"Join Code: {_joinCode}");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }
        
        // 서버 켜기
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        
        // tcp, udp, tls, dtls
        RelayServerData relayData = new RelayServerData(_allocation, "dtls");
        transport.SetRelayServerData(relayData);

        // 로비 정보 생성
        try
        {
            CreateLobbyOptions lobbyOption = new CreateLobbyOptions();
            lobbyOption.IsPrivate = false;
            lobbyOption.Data = new Dictionary<string, DataObject>()
            {
                {
                    "JoinCode",
                    new DataObject(DataObject.VisibilityOptions.Member, _joinCode)
                }
            };
            
            string playerName = ClientSingleton.Instance.GameManager.PlayerName;

            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync
                ($"{playerName}'s Lobby", _maxConnections, lobbyOption);

            _lobbyId = lobby.Id; // 로비 아이디 저장

            HostSingleton.Instance.StartCoroutine(HeartBeatLobby(15));
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError(ex);
            return;
        }
        
        MakeNetworkserver();
        
        ClientSingleton.Instance.GameManager.SetPayloadData();
        
        if (NetworkManager.Singleton.StartHost())
        {
            NetworkManager.Singleton.SceneManager.LoadScene(
                SceneNames.GameScene, LoadSceneMode.Single);
        }
    }
    
    private IEnumerator HeartBeatLobby(float waitTime)
    {
        var timer = new WaitForSecondsRealtime(waitTime);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(_lobbyId);
            yield return timer;
        }
    }

    public bool StartHostLocalNetwork()
    {
        MakeNetworkserver();
        ClientSingleton.Instance.GameManager.SetPayloadData();
        if (NetworkManager.Singleton.StartHost())
        {
            NetworkManager.Singleton.SceneManager.LoadScene(
                SceneNames.GameScene, LoadSceneMode.Single);
            return true;
        }
        else
        {
            NetworkManager.Singleton.Shutdown();
            return false;
        }
    }

    public void Dispose()
    {
        Shutdown();
    }

    private async Task Shutdown()
    {
        HostSingleton.Instance.StopAllCoroutines();
        
        if(!string.IsNullOrEmpty(_lobbyId))
        {
            try
            {
                await Lobbies.Instance.DeleteLobbyAsync(_lobbyId);
            }
            catch (LobbyServiceException ex)
            {
                Debug.LogError(ex);
            }
        }
        
        _lobbyId = string.Empty;
        NetServer?.Dispose();
    }
}