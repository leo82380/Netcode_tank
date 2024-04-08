using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private Button _enterBtn;

    private RectTransform _rectTrm;
    public RectTransform Rect => _rectTrm;
    
    private Lobby _lobby;

    public void SetRoomTemplate(Lobby lobby)
    {
        _titleText.text = lobby.Name;
        _countText.text = $"{lobby.Players.Count} / {lobby.MaxPlayers}";
        _lobby = lobby;
        
        _enterBtn.onClick.AddListener(HandleEnterBtnClick);
    }

    private async void HandleEnterBtnClick()
    {
        LoaderUI.Instance.Show(true);
        try
        {
            // 내가 가지고 있는 로비 정보로 서버에서 불러옴
            Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(_lobby.Id);
            
            string joinCode = lobby.Data["JoinCode"].Value;
            await ClientSingleton.Instance.GameManager.StartClientWithJoinCode(joinCode);
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError(ex);
        }
    }

    private void Awake()
    {
        _rectTrm = GetComponent<RectTransform>();
    }
}
