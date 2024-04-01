using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    private static HostSingleton _instance;
    
    public HostSingleton Instance
    {
        get
        {
            if (_instance != null) return _instance;
            
            _instance = FindObjectOfType<HostSingleton>();
            if (_instance == null)
            {
                Debug.LogError("No Host Singleton");
            }
            return _instance;
        }
    }
    
    public HostGameManager GameManager { get; set; }
    
    public void CreateHost()
    {
        GameManager = new HostGameManager();
    }
}
