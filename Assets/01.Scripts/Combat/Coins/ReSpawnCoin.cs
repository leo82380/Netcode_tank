using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReSpawnCoin : Coin
{

    public event Action<ReSpawnCoin> OnCollected;

    private Vector3 _prevPos;

    public override int Collect()
    {
        if (_alreadyCollected) return 0;

        if(!IsServer)
        {
            SetVisible(false);
            return 0;
        }

        _alreadyCollected = true;
        OnCollected?.Invoke(this);
        isActive.Value = false; //코인을 꺼준다.

        return _coinValue;
    }

    [ContextMenu("Reset Coin")]
    //서버만 실행하는 함수. 클라는 이거 안실행해
    public void ResetCoin()
    {
        _alreadyCollected = false;
        isActive.Value = true; //네트워크 변수를 true로 설정해주는거지
        SetVisible(true);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _prevPos = transform.position;

        if(IsClient)
            isActive.OnValueChanged += HandleActiveValueChanged;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if(IsClient)
            isActive.OnValueChanged -= HandleActiveValueChanged;
    }

    private void HandleActiveValueChanged(bool previousValue, bool newValue)
    {
        SetVisible(newValue);
    }
}