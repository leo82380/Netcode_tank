using System;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private ParticleSystem _explosionEffect;
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
        for (int i = 0; i < 4; i++)
        {
            Vector2 pos = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle;
            Instantiate(_explosionEffect, pos, Quaternion.identity);
        }
        
        if (IsClient)
        {
            currentHealth.OnValueChanged -= HandleHealthValueChanged;
        }
    }
    
    public float GetNormalizedHealth()
    {
        return (float)currentHealth.Value / maxHealth;
    }

    private void HandleHealthValueChanged(int previousValue, int newValue)
    {
        OnHealthChangedEvent?.Invoke();
        
        int delta = newValue - previousValue;
        int value = Mathf.Abs(delta);
        
        if (value == maxHealth) return;
        
        Color textColor = delta < 0 ? Color.red : Color.green;
        TextManager.Instance.PopUpText(value.ToString(), transform.position, textColor);
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
        if (MapManager.Instance.IsInSafetyZone(transform.position))
        {
            SetInvincibleClientRpc("무적", Color.white);
            return;
        }
        ModifyHealth(-damageValue);
    }
    
    public void RestoreHealth(int restoreValue)
    {
        ModifyHealth(restoreValue);
    }
    
    [ClientRpc]
    public void SetInvincibleClientRpc(string text, Color color)
    {
        TextManager.Instance.PopUpText(text, transform.position, color);
    }
}
