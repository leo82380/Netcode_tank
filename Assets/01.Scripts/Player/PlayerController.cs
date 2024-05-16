using System;
using Cinemachine;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public static event Action<PlayerController> OnPlayerSpawn;
    public static event Action<PlayerController> OnPlayerDespawn;
    
    
    [Header("Reference")] 
    [SerializeField] private CinemachineVirtualCamera _followCam;
    [SerializeField] private TextMeshPro _nameText;
    [SerializeField] private SpriteRenderer minimapIcon;
    
    [Header("Setting Values")] 
    [SerializeField] private int _ownerCamPriority;
    // 탱크 인덱스
    public NetworkVariable<Color> tankColor;
    
    public PlayerVisual VisualCompo { get; private set; }
    public PlayerMovement MovementCompo { get; private set; }
    public Health HealthCompo { get; private set; }
    public CoinCollector CoinCompo { get; private set; }
    public ProjectileLauncher LauncherCompo { get; private set; }
    
    public NetworkVariable<FixedString32Bytes> playerName;

    public ShopNPC shopNpc;
    private PlayerInput _playerInput;

    private void Awake()
    {
        tankColor = new NetworkVariable<Color>();
        playerName = new NetworkVariable<FixedString32Bytes>();
        
        VisualCompo = GetComponent<PlayerVisual>();
        MovementCompo = GetComponent<PlayerMovement>();
        HealthCompo = GetComponent<Health>();
        CoinCompo = GetComponent<CoinCollector>();
        _playerInput = GetComponent<PlayerInput>();
        LauncherCompo = GetComponent<ProjectileLauncher>();
    }

    public override void OnNetworkSpawn()
    {
        tankColor.OnValueChanged += HandleColorChanged;
        
        playerName.OnValueChanged += HandlePlayerNameChanged;
        if (IsOwner)
        {
            _followCam.Priority = _ownerCamPriority;
            minimapIcon.color = Color.blue;
            _playerInput.OnShopKeyEvent += HandleShopKeyEvent;
        }

        if (IsServer)
        {
            UserData data = 
                HostSingleton.Instance.GameManager.NetServer.GetUserDataByClientID(OwnerClientId);
            playerName.Value = data.username;
        }
        OnPlayerSpawn?.Invoke(this);
        HandlePlayerNameChanged(string.Empty, playerName.Value);
    }

    private void HandlePlayerNameChanged(FixedString32Bytes previousvalue, FixedString32Bytes newvalue)
    {
        _nameText.text = newvalue.ToString();
    }


    public override void OnNetworkDespawn()
    {
        tankColor.OnValueChanged -= HandleColorChanged;
        playerName.OnValueChanged -= HandlePlayerNameChanged;
        
        if (IsOwner)
        {
            _playerInput.OnShopKeyEvent -= HandleShopKeyEvent;
        }
        
        OnPlayerDespawn?.Invoke(this);
    }

    private void HandleShopKeyEvent()
    {
        if (shopNpc == null) return;
        
        shopNpc.OpenShop(this);
    }

    private void HandleColorChanged(Color previousvalue, Color newvalue)
    {
        VisualCompo.SetTintColor(newvalue);
    }

    #region Only Server excution area
    public void SetTankData(Color color, int coin)
    {
        tankColor.Value = color;
    }
    #endregion

    [ClientRpc]
    public void AddDamageToLauncherClientRpc(int upgradeValue)
    {
        LauncherCompo.damage += upgradeValue;
    }

    [ClientRpc]
    public void AddHPClientRpc(int upgradeValue)
    {
        HealthCompo.maxHealth += upgradeValue;
    }
}
