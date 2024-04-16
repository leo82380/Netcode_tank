using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TankSelectPanel : NetworkBehaviour
{
    [SerializeField] private Button _startBtn;
    [SerializeField] private List<TankSelectUI> _selectUIList;

    private void Awake()
    {
        _selectUIList = new List<TankSelectUI>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            _startBtn.onClick.AddListener(HandleGameStart);
        }
        else
        {
            _startBtn.gameObject.SetActive(false);
        }
    }

    private void HandleGameStart()
    {
        //Debug.Log("Game Start");
        GameManager.Instance.StartGame(_selectUIList);
        GameStartClientRpc();
    }

    [ClientRpc]
    private void GameStartClientRpc()
    {
        gameObject.SetActive(false);
    }

    public void AddSelectUI(TankSelectUI ui)
    {
        _selectUIList.Add(ui);
        ui.OnDisconnectEvent += HandleDisconnected;
        ui.OnReadyChangeEvent += HandleReadyChange;

        HandleReadyChange(false);
    }
    
    private void HandleDisconnected(TankSelectUI ui)
    {
        ui.OnDisconnectEvent -= HandleDisconnected;
        _selectUIList.Remove(ui);
    }
    
    private void HandleReadyChange(bool obj)
    {
        bool allReady = 
            _selectUIList.Count > 0 &&
            _selectUIList.Any(x => x.isReady.Value == false) == false;
        
        _startBtn.interactable = allReady;
    }
}
