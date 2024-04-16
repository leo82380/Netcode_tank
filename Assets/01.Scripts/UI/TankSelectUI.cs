using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class TankSelectUI : NetworkBehaviour
{
    public event Action<bool> OnReadyChangeEvent;
    public event Action<TankSelectUI> OnDisconnectEvent;
    
    [SerializeField] private Image _tankImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Button[] _colorButtons;
    [SerializeField] private Button _readyBtn;
    [SerializeField] private Image _statusImage;

    
    [SerializeField] private Sprite _readySprite, _notReadySprite, _readyBtnSprite, _notReadyBtnSprite;

    public NetworkVariable<bool> isReady;
    public NetworkVariable<Color> selectedColor;

    private TextMeshProUGUI _readyBtnText;
    private NetworkVariable<FixedString32Bytes> playerName;

    private void Awake()
    {
        _readyBtnText = _readyBtn.GetComponentInChildren<TextMeshProUGUI>();

        playerName = new NetworkVariable<FixedString32Bytes>();

        isReady = new NetworkVariable<bool>();
        selectedColor = new NetworkVariable<Color>();
    }

    private void HandlePlayerNameChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        Debug.Log(newValue.ToString());
        _nameText.text = newValue.ToString();
    }

    public override void OnNetworkSpawn()
    {
        playerName.OnValueChanged += HandlePlayerNameChanged;
        isReady.OnValueChanged += HandleIsReadyChanged;
        selectedColor.OnValueChanged += HandleSelectedColorChanged;
        
        // 처음 시작 시 서버가 가지고 있던 기본 값으로 핸들링
        HandlePlayerNameChanged(string.Empty, playerName.Value);  //이거 추가
        HandleIsReadyChanged(false, isReady.Value);
        HandleSelectedColorChanged(Color.white, selectedColor.Value);

        if (IsServer)
        {
            isReady.Value = false;
            selectedColor.Value = Color.red;
        }

        if (IsOwner == false) return;
        _readyBtn.onClick.AddListener(HandleReadyBtnClick);

        foreach(Button button in _colorButtons)
        {
            button.onClick.AddListener(() =>
            {
                SetTankColor(button.image.color);
            });
        }
        
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnDisconnectEvent?.Invoke(this); // 이여석이 종료되었다 알려줌
        }
        isReady.OnValueChanged -= HandleIsReadyChanged;
        selectedColor.OnValueChanged -= HandleSelectedColorChanged;

        if (IsOwner == false) return;

        _readyBtn.onClick.RemoveListener(HandleReadyBtnClick);
        foreach (Button button in _colorButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    private void HandleSelectedColorChanged(Color previousValue, Color newValue)
    {
        _tankImage.color = newValue;
    }

    private void HandleIsReadyChanged(bool previousValue, bool newValue)
    {
        if (newValue)
        {
            _statusImage.sprite = _readySprite;
            _readyBtn.image.sprite = _readyBtnSprite;
            _readyBtnText.text = "준비 완료";
        }
        else
        {
            _statusImage.sprite = _notReadySprite;
            _readyBtn.image.sprite = _notReadyBtnSprite;
            _readyBtnText.text = "준비";
        }
    }


    #region Only Owner execution area
    private void HandleReadyBtnClick()
    {        
        SetReadyClaimToServerRpc(!isReady.Value);
    }

    private void SetTankColor(Color color)
    {
        SetColorClaimToServerRpc(color);
    }
    #endregion



    #region Only server execution area
    [ServerRpc]
    public void SetColorClaimToServerRpc(Color color)
    {
        selectedColor.Value = color;
    }

    public void SetTankName(string name)
    {
        playerName.Value = name;
    }

    [ServerRpc]
    public void SetReadyClaimToServerRpc(bool value)
    {
        isReady.Value = value;
        OnReadyChangeEvent?.Invoke(value);
    }
    #endregion


    #region Only Client execution area
    
    #endregion

}