using System;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    public NetworkVariable<int> currentHealth;
    public int maxHealth;
    
    public event Action OnDieEvent;
    public event Action OnHealthChangedEvent;

    private bool _isDead;

    private void Awake()
    {
        currentHealth = new NetworkVariable<int>(maxHealth);
    }

    public override void OnNetworkSpawn()
    {
        // NetworkVariable은 서버만 건드릴 수 있음. 클라는 받기만
        if (IsClient)
        {
            currentHealth.OnValueChanged += HandleHealthValueChanged;
        }
        
        if (IsServer == false) return;
        currentHealth.Value = maxHealth; // 처음 시작 시 최대체력
    }
    
    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            currentHealth.OnValueChanged -= HandleHealthValueChanged;
        }
    }
    
    public float GetNormalizedHealth()
    {
        return (float)currentHealth.Value / maxHealth;
    }

    private void HandleHealthValueChanged(int previousvalue, int newvalue)
    {
        OnHealthChangedEvent?.Invoke();
    }

    // 서버만 실행
    private void ModifyHealth(int value)
    {
        if (_isDead) return;
        
        currentHealth.Value = Mathf.Clamp(currentHealth.Value + value, 0, maxHealth);

        if (currentHealth.Value == 0)
        {
            OnDieEvent?.Invoke();
            _isDead = true;
        }
    }

    public void TakeDamage(int damageValue)
    {
        ModifyHealth(-damageValue);
    }
    
    public void RestoreHealth(int restoreValue)
    {
        ModifyHealth(restoreValue);
    }
}
