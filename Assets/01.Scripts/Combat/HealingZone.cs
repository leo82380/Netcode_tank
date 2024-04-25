using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealingZone : NetworkBehaviour
{
    [Header("Reference")]
    [SerializeField] private Transform _healPowerBarTrm;

    [Header("Settings")]
    [SerializeField] private int _maxHealPower = 15; //15틱 회복가능
    [SerializeField] private float _cooldown = 60f, _healTickRate = 1f;
    [SerializeField] private int _coinPerTick = 10, _healPerTick = 10;

    [SerializeField]
    [ColorUsage(true, true)] private Color _normalColor, _chargeColor;

    #region Only server variable
    private List<PlayerController> _playerInZone = new List<PlayerController>();
    #endregion

    private NetworkVariable<bool> _isInCharge;
    private NetworkVariable<int> _healPower;

    private readonly int _colorHash = Shader.PropertyToID("_EmissionColor");

    private float _remainCooldown;
    private float _tickTimer;

    private Material _material;

    private void Awake()
    {
        _healPower = new NetworkVariable<int>();
        _isInCharge = new NetworkVariable<bool>();

        _material = _healPowerBarTrm.GetComponentInChildren<SpriteRenderer>().material;
    }

    public override void OnNetworkSpawn()
    {
        if(IsClient)
        {
            _healPower.OnValueChanged += HandleHealPowerChanged;
            _isInCharge.OnValueChanged += HandleChargeModeChanged;
            HandleHealPowerChanged(0, _healPower.Value);
            HandleChargeModeChanged(false, _isInCharge.Value);
        }
        
        if(IsServer)
        {
            _healPower.Value = _maxHealPower; //초기 시작시 고정하고 간다.
        }
    }


    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            _healPower.OnValueChanged -= HandleHealPowerChanged;
            _isInCharge.OnValueChanged -= HandleChargeModeChanged;
        }
    }

    private void HandleChargeModeChanged(bool previousValue, bool newValue)
    {
        Color color = newValue ? _chargeColor : _normalColor;
        _material.SetColor(_colorHash, color);
    }

    private void HandleHealPowerChanged(int previousValue, int newValue)
    {
        _healPowerBarTrm.localScale = new Vector3((float)newValue / _maxHealPower, 1, 1);
    }


    #region Only Server execution area

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;

        if(collision.attachedRigidbody.TryGetComponent<PlayerController>(out var player))
        {
            _playerInZone.Add(player);
            Debug.Log($"Enter : {player.playerName.Value}");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsServer) return;

        if (collision.attachedRigidbody.TryGetComponent<PlayerController>(out var player))
        {
            _playerInZone.Remove(player);
            Debug.Log($"Exit : {player.playerName.Value}");
        }
    }

    private void Update()
    {
        if (!IsServer) return;

        if(_remainCooldown > 0)  //서비스 준비중 
        {
            _remainCooldown -= Time.deltaTime;

            float percent = 1f - _remainCooldown / _cooldown;
            int value = Mathf.FloorToInt(_maxHealPower * percent);
            _healPower.Value = value;

            if(_remainCooldown < 0)
            {
                _healPower.Value = _maxHealPower;
                _isInCharge.Value = false;
            }
            else
            {
                return;
            }
        }

        //여기까지 왔다는 이야기는 아직 힐파워가 있고 쿨타임이 안돌아가고 있다.

        _tickTimer += Time.deltaTime;

        if(_tickTimer >= _healTickRate)  //힐 틱을 계산할 차례가 온거야.
        {
            foreach(PlayerController p in _playerInZone)
            {
                if (_healPower.Value <= 0) break;

                if (p.HealthCompo.currentHealth.Value == p.HealthCompo.maxHealth)
                    continue;

                if (p.CoinCompo.totalCoin.Value < _coinPerTick)
                    continue;

                p.CoinCompo.SpendCoin(_coinPerTick);
                p.HealthCompo.RestoreHealth(_healPerTick);

                _healPower.Value -= 1;
                if(_healPower.Value <= 0)
                {
                    _isInCharge.Value = true;
                    _remainCooldown = _cooldown;
                }
            }

            _tickTimer = _tickTimer % _healTickRate;
        }
        // 여기에도 뭔가 작업을 해야 정상적으로 틱이 처리된다.
    }

    #endregion
}