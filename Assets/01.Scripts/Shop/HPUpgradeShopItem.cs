using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HPUpgradeShopItem : UpgradeShopItem
{
    [SerializeField] private int _increaseValue = 5;
    protected override void PurchaseProcess()
    {
        UpgradeServerRpc(_parent.customer.OwnerClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpgradeServerRpc(ulong customerOwnerClientId)
    {
        PlayerController player = GameManager.Instance.GetPlayerByClientID(customerOwnerClientId);
        
        if (player.CoinCompo.totalCoin.Value < _upgradeCost) return;
        
        player.CoinCompo.totalCoin.Value -= _upgradeCost;
        player.AddHPClientRpc(_increaseValue);
    }
}
