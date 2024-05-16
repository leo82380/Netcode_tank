using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;
    [SerializeField] private ShopWindowUI _shopWindow;

    private void Awake()
    {
        Instance = this;
    }
    
    public void OpenShop(PlayerController customer)
    {
        if (_shopWindow.IsOpen) return;
        
        _shopWindow.SetCustomer(customer);
        _shopWindow.ActiveWindow(true);
    }
    
    public void CloseShop()
    {
        _shopWindow.SetCustomer(null);
        _shopWindow.ActiveWindow(false);
    }
}
