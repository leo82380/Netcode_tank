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
    
    public void Awake()
    {
        Instance = this;
        _tankSelectPanel = _selectPanelTrm.parent.GetComponent<TankSelectPanel>();
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
            SpawnTank(clientID, color);
        }
    }

    public async void SpawnTank(ulong clientID, Color color, float delay = 0)
    {
        if (delay > 0)
        {
            await Task.Delay(Mathf.CeilToInt(delay * 1000));
        }
        
        Vector3 position = TankSpawnPoint.GetRandomSpawnPos();
        
        PlayerController tank = Instantiate(_playerPrefab, position, Quaternion.identity);
        tank.NetworkObject.SpawnAsPlayerObject(clientID); // 이 클라이언트 아이디가 주인이 됨
        tank.SetTankColor(color);
    }
}
