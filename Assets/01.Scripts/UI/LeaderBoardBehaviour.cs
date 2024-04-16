using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LeaderBoardBehaviour : NetworkBehaviour
{
    [SerializeField] private RankData _rankDataPrefab;
    [SerializeField] private RectTransform _contentRect; // 컨텐츠가 추가될 렉트

    private int _displayCount = 5; // 4개만 보여주고 자기자신도 보여주는

    private NetworkList<LeaderBoardEntityState> _leaderBoards;

    private void Awake()
    {
        _leaderBoards = new NetworkList<LeaderBoardEntityState>();
    }
    
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            PlayerController[] players = 
                FindObjectsOfType<PlayerController>();

            foreach (PlayerController p in players)
            {
                HandlePlayerSpawn(p);
            }
            PlayerController.OnPlayerSpawn += HandlePlayerSpawn;
            PlayerController.OnPlayerDespawn += HandlePlayerDespawn;
        }
    }


    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            PlayerController.OnPlayerSpawn -= HandlePlayerSpawn;
            PlayerController.OnPlayerDespawn -= HandlePlayerDespawn;
        }
    }
    
    private void HandlePlayerDespawn(PlayerController obj)
    {
        
    }
    
    private void HandlePlayerSpawn(PlayerController playerController)
    {
        
    }
}
