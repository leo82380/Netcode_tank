using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton _instance;
    
    public static ClientSingleton Instance
    {
        get
        {
            if (_instance != null) return _instance;
            
            _instance = FindObjectOfType<ClientSingleton>();
            if (_instance == null)
            {
                Debug.LogError("No Client Singleton");
            }
            return _instance;
        }
    }
    
    public ClientGameManager GameManager { get; set; }

    public async Task<bool> CreateClient()
    {
        GameManager = new ClientGameManager();
        return await GameManager.InitAsync();
    }
}
