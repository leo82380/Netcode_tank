using System;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public abstract class Coin : NetworkBehaviour
{
    protected SpriteRenderer _spriteRenderer;
    protected CircleCollider2D _collider2D;
    protected int _coinValue = 10;
    protected bool _alreadyCollected;
    
    protected readonly int _viewOffsetHash  = Shader.PropertyToID("_ViewOffset");

    public NetworkVariable<bool> isActive;

    protected Tween _viewTween = null;

    public abstract int Collect();

    protected virtual void Awake()
    {
        isActive = new NetworkVariable<bool>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider2D = GetComponent<CircleCollider2D>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            SetVisible(isActive.Value);
        }
    }
    
    public void SetVisible(bool value)
    {
        _collider2D.enabled = value;
        _spriteRenderer.enabled = value;

        if (value)
        {
            if (_viewTween != null && _viewTween.IsActive())
            {
                _viewTween.Kill();
            }
            
            Material mat = _spriteRenderer.material;
            mat.SetFloat(_viewOffsetHash, 0); //0부터
            float coinVisibleTime = 1.5f;
            
            _viewTween = mat.DOFloat(1.1f, _viewOffsetHash, coinVisibleTime);

            // _viewTween = DOTween.To(
            //     () => mat.GetFloat(_viewOffsetHash),
            //     value => mat.SetFloat(_viewOffsetHash, value),
            //     1.1f, coinVisibleTime);
        }
    }

    public void SetValue(int value)
    {
        _coinValue = value;
    }
}
