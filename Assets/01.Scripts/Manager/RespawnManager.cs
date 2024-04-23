using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnManager : NetworkBehaviour
{
    [SerializeField] private float _keepCoinRatio = 0.8f;
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        PlayerController[] players =
            FindObjectsByType<PlayerController>(FindObjectsSortMode.None);

        foreach (PlayerController player in players)
        {
            HandlePlayerSpawn(player);
        }
        
        PlayerController.OnPlayerSpawn += HandlePlayerSpawn;
        PlayerController.OnPlayerDespawn += HandlePlayerDespawn;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        PlayerController.OnPlayerSpawn -= HandlePlayerSpawn;
        PlayerController.OnPlayerDespawn -= HandlePlayerDespawn;
    }
    
    private void HandlePlayerSpawn(PlayerController player)
    {
        player.HealthCompo.OnDieEvent += () =>
        {
            ulong clientID = player.OwnerClientId;
            Color color = player.tankColor.Value;

            int remainCoin = Mathf.FloorToInt(
                player.CoinCompo.totalCoin.Value * _keepCoinRatio);
            
            Destroy(player.gameObject);
            
            GameManager.Instance.SpawnTank(clientID, color, remainCoin, 10f);
        };
    }
    
    private void HandlePlayerDespawn(PlayerController player)
    {
    }
}
