using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton _clientPrefab;
    [SerializeField] private HostSingleton _hostPrefab;
    
    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        bool isDedicate = SystemInfo.graphicsDeviceType == 
                          UnityEngine.Rendering.GraphicsDeviceType.Null;
        LunchInMode(isDedicate);
    }

    private async void LunchInMode(bool isDedicateServer)
    {
        if (isDedicateServer)
        {
            // 
        }
        else
        {
            HostSingleton hostSingleton = Instantiate(_hostPrefab, transform);
            hostSingleton.CreateHost();
            
            ClientSingleton clientSingleton = Instantiate(_clientPrefab, transform);
            await clientSingleton.CreateClient();
            
            ClientSingleton.Instance.GameManager.GotoMenuScene();
        }
    }
}
