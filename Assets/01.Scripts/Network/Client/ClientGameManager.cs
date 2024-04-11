using System;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager : IDisposable
{
    private JoinAllocation _allocation;
    private string _playerName;
    public string PlayerName => _playerName;
    
    public NetworkClient NetClient { get; private set; }
    
    public async Task<bool> InitAsync()
    {
        // UGS 인증파트
        // WebRequest
        await UnityServices.InitializeAsync(); // 초기화
        
        NetClient = new NetworkClient(NetworkManager.Singleton);
        
        AuthState authState = await UGSAuthWrapper.DoAuth(); // 인증 5회 진행
        
        if (authState == AuthState.Authenticated)
        {
            return true;
        }

        return false;
    }

    public void GotoMenuScene()
    {
        SceneManager.LoadScene(SceneNames.MenuScene);
    }

    public async Task StartClientWithJoinCode(string joinCode)
    {
        try
        {
            _allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            var relayServerData = new RelayServerData(_allocation, "dtls");
            transport.SetRelayServerData(relayServerData);
            
            SetPayloadData();
            
            NetworkManager.Singleton.StartClient();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }
    }
    
    public bool StartClientLocalNetwork()
    {
        SetPayloadData();
        if (NetworkManager.Singleton.StartClient() == false)
        {
            NetworkManager.Singleton.Shutdown();
            return false;
        }
        return true;
    }

    public void SetPlayerName(string playerName)
    {
        _playerName = playerName;
    }

    // 요청헤더에 사용자 정보 저장
    public void SetPayloadData()
    {
        UserData userData = new UserData()
        {
            username = _playerName,
            userAuthID = AuthenticationService.Instance.PlayerId
        };
        string json = JsonUtility.ToJson(userData);
        byte[] payload = Encoding.UTF8.GetBytes(json);
        
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payload;
    }

    public void Dispose()
    {
        NetClient?.Dispose();
    }
    
    public void Disconnect()
    {
        NetClient?.Dispose();
    }
}
