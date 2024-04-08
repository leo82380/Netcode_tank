using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UGSConnectPannel : MonoBehaviour
{
    [SerializeField] private Button _relayHostBtn;
    [SerializeField] private Button _enterLobbyBtn;

    public UnityEvent OpenLobbyEvent;
    private void Awake()
    {
        _relayHostBtn.onClick.AddListener(HandleRelayHostClick);
        _enterLobbyBtn.onClick.AddListener(HandleLobbyClick);
    }

    private void HandleLobbyClick()
    {
        OpenLobbyEvent?.Invoke();
    }

    private async void HandleRelayHostClick()
    {
        await HostSingleton.Instance.GameManager.StartHostAsync();
    }
}
