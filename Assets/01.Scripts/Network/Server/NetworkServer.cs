using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    public NetworkManager _NetworkManager;
    
    // 클라이언트 아이디로 Auth 아이디 알아내는 것
    private Dictionary<ulong, string> _clientToAuthDictionary
        = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> _authToUserDictionary
        = new Dictionary<string, UserData>();

    public NetworkServer(NetworkManager manager)
    {
        _NetworkManager = manager;
        _NetworkManager.ConnectionApprovalCallback
            += HandleApprovalCheck;

        _NetworkManager.OnServerStarted += HandleSererStart;
    }

    private void HandleSererStart()
    {
        _NetworkManager.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    // 접속한 순서대로 0 1 2 3
    // authID 문자열 형태
    private void HandleClientDisconnect(ulong clientID)
    {
        if (_clientToAuthDictionary.TryGetValue(clientID, out string authID))
        {
            _clientToAuthDictionary.Remove(clientID);
            _authToUserDictionary.Remove(authID);
        }
    }

    private void HandleApprovalCheck
        (NetworkManager.ConnectionApprovalRequest request, 
            NetworkManager.ConnectionApprovalResponse response)
    {
        string json = Encoding.UTF8.GetString(request.Payload);
        UserData data = JsonUtility.FromJson<UserData>(json);
        
        _clientToAuthDictionary[request.ClientNetworkId] = data.userAuthID;
        _authToUserDictionary[data.userAuthID] = data;

        response.CreatePlayerObject = false;
        response.Approved = true;
        
        HostSingleton.Instance.StartCoroutine(
            CreatePanelWithDelay(0.5f, request.ClientNetworkId, data.username));
    }
    
    private IEnumerator CreatePanelWithDelay(float time, ulong clientID, string username)
    {
        yield return new WaitForSeconds(time);
        GameManager.Instance.CreateUIPanel(clientID, username);
    }

    public void Dispose()
    {
        if (_NetworkManager == null) return;
        
        _NetworkManager.ConnectionApprovalCallback -= HandleApprovalCheck;
        _NetworkManager.OnServerStarted -= HandleSererStart;
        _NetworkManager.OnClientDisconnectCallback -= HandleClientDisconnect;

        if (_NetworkManager.IsListening)
        {
            _NetworkManager.Shutdown();
        }
    }
}
