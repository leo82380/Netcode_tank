using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private PlayerController _playerPrefab;
    [SerializeField] private TankSelectUI _selectUIPrefab;
    [SerializeField] private RectTransform _selectPanelTrm;
    
    private TankSelectPanel _tankSelectPanel;
    private Dictionary<ulong, PlayerController> _playerDictionary;
    
    public void Awake()
    {
        Instance = this;
        _tankSelectPanel = _selectPanelTrm.parent.GetComponent<TankSelectPanel>();
        _playerDictionary = new Dictionary<ulong, PlayerController>();
    }
    
    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            PlayerController.OnPlayerSpawn += HandlePlayerSpawn;
            PlayerController.OnPlayerDespawn += HandlePlayerDespawn;
        }
    }
    
    private void HandlePlayerSpawn(PlayerController controller)
    {
        _playerDictionary.Add(controller.OwnerClientId, controller);
    }

    private void HandlePlayerDespawn(PlayerController controller)
    {
        if (_playerDictionary.ContainsKey(controller.OwnerClientId))
        {
            _playerDictionary.Remove(controller.OwnerClientId);
        }
    }

    public PlayerController GetPlayerByClientID(ulong clientID)
    {
        if (_playerDictionary.TryGetValue(clientID, out PlayerController controller))
        {
            return controller;
        }

        return null;
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            PlayerController.OnPlayerSpawn -= HandlePlayerSpawn;
            PlayerController.OnPlayerDespawn -= HandlePlayerDespawn;
        }
    }


    public void CreateUIPanel(ulong clientID, string username)
    {
        TankSelectUI ui = Instantiate(_selectUIPrefab, _selectPanelTrm);
        ui.NetworkObject.SpawnAsPlayerObject(clientID);
        ui.transform.SetParent(_selectPanelTrm);
        ui.transform.localScale = Vector3.one;
        
        _tankSelectPanel.AddSelectUI(ui);
        
        ui.SetTankName(username);
    }

    public void StartGame(List<TankSelectUI> tankUIList)
    {
        foreach (TankSelectUI ui in tankUIList)
        {
            ulong clientID = ui.OwnerClientId;
            Color color = ui.selectedColor.Value;
            SpawnTank(clientID, color, 0);
        }
    }

    public async void SpawnTank(ulong clientID, Color color,int coin, float delay = 0)
    {
        if (delay > 0)
        {
            await Task.Delay(Mathf.CeilToInt(delay * 1000));
        }
        
        Vector3 position = TankSpawnPoint.GetRandomSpawnPos();
        
        PlayerController tank = Instantiate(_playerPrefab, position, Quaternion.identity);
        tank.NetworkObject.SpawnAsPlayerObject(clientID); // 이 클라이언트 아이디가 주인이 됨
        tank.SetTankData(color, coin);
        
    }
}
