using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class BountyCoin : Coin
{
    public override int Collect()
    {
        if (!IsServer)
        {
            SetVisible(false);
            return 0;
        }
        
        if (_alreadyCollected) return 0;
        _alreadyCollected = true;
        
        Destroy(gameObject);
        return _coinValue;
    }

    public void SetScaleAndVisible(float coinScale)
    {
        isActive.Value = true;

        CoinSpawnClientRPC(coinScale);
    }

    [ClientRpc]
    private void CoinSpawnClientRPC(float coinScale)
    {
        Vector3 destination = transform.position;
        transform.position = destination + new Vector3(0, 3f, 0);
        transform.localScale = Vector3.one * coinScale;
        SetVisible(true);

        transform.DOMove(destination, 0.6f).SetEase(Ease.OutBounce).OnComplete
        (() =>
        {

        });
    }
}
