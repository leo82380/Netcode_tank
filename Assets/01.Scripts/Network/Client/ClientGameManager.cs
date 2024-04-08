using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager
{
    private JoinAllocation _allocation;
    private string _playerName;
    public string PlayerName => _playerName;
    
    public async Task<bool> InitAsync()
    {
        // UGS 인증파트
        // WebRequest
        await UnityServices.InitializeAsync(); // 초기화
        
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
}
