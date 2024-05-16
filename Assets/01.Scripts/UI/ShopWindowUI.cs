using System.Linq;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ShopWindowUI : NetworkBehaviour
{
    [SerializeField] private Button _closeBtn;
    
    private CanvasGroup _canvasGroup;
    public bool IsOpen => _canvasGroup.interactable;

    [HideInInspector] public PlayerController customer;

    private void Awake()
    {
        _closeBtn.onClick.AddListener(() => ActiveWindow(false));
        _canvasGroup = GetComponent<CanvasGroup>();
        
        GetComponentsInChildren<UpgradeShopItem>().ToList()
            .ForEach(shop => shop.Initialize(this));
    }

    public void ActiveWindow(bool isActive)
    {
        _canvasGroup.interactable = isActive;
        _canvasGroup.blocksRaycasts = isActive;
        
        float alpha = isActive ? 1f : 0f;
        _canvasGroup.DOFade(alpha, 0.4f);
    }

    public void SetCustomer(PlayerController player)
    {
        customer = player;
    }
}
