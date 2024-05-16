using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AtkUpgradeShopItem : UpgradeShopItem
{
    [SerializeField] private int _upgradeValue = 5;
    protected override void PurchaseProcess()
    {
        UpgradeServerRpc(_parent.customer.OwnerClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpgradeServerRpc(ulong clientID)
    {
        PlayerController player = GameManager.Instance.GetPlayerByClientID(clientID);
        
        if (player.CoinCompo.totalCoin.Value < _upgradeCost) return;
        
        player.CoinCompo.totalCoin.Value -= _upgradeCost;
        player.AddDamageToLauncherClientRpc(_upgradeValue);
    }
}
