using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnManager : NetworkBehaviour
{
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
            
            Destroy(player.gameObject);
            
            GameManager.Instance.SpawnTank(clientID, color, 10f);
        };
    }
    
    private void HandlePlayerDespawn(PlayerController player)
    {
    }
}
