using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UpgradeShopItem : MonoBehaviour
{
    [SerializeField] private Button _purchaseBtn;
    [SerializeField] protected int _upgradeCost = 100;
    [SerializeField] protected TextMeshProUGUI _purchaseText;

    protected ShopWindowUI _parent;

    protected virtual void Start()
    {
        _purchaseText.text = $"{_upgradeCost} G";
        _purchaseBtn.onClick.AddListener(HandlePurchaseClick);
    }

    public void Initialize(ShopWindowUI parent)
    {
        _parent = parent;
    }

    protected void HandlePurchaseClick()
    {
        if (_parent.customer == null) return;
        if(_parent.customer.CoinCompo.totalCoin.Value < _upgradeCost)
        {
            MessageSystem.Instance.ShowText("코인이 없네요", 0.8f);
            return;
        }

        PurchaseProcess();
    }

    protected abstract void PurchaseProcess();
}