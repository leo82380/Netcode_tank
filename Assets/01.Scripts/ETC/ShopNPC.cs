using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ShopNPC : NetworkBehaviour
{
    [SerializeField] private Sprite[] _iconSprites;
    [SerializeField] private SpriteRenderer _iconRenderer;
    private Coroutine _iconCoroutine;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody.TryGetComponent(out PlayerController player))
        {
            if (player.IsOwner)
            {
                player.shopNpc = this;
                _iconCoroutine = StartCoroutine(ShopIconCoroutine());
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.attachedRigidbody.TryGetComponent(out PlayerController player))
        {
            if (player.IsOwner && _iconCoroutine != null)
            {
                player.shopNpc = null;
                ShopManager.Instance.CloseShop();
                StopCoroutine(_iconCoroutine);
                _iconRenderer.sprite = _iconSprites[0];
            }
        }
    }

    private IEnumerator ShopIconCoroutine()
    {
        var ws = new WaitForSeconds(0.2f);
        int idx = 0;
        while (true)
        {
            _iconRenderer.sprite = _iconSprites[idx];
            yield return ws;
            idx = (idx + 1) % _iconSprites.Length;
        }
    }
    
    public void OpenShop(PlayerController customer)
    {
        ShopManager.Instance.OpenShop(customer);
    }
}
