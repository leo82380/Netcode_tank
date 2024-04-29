using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuPanel : MonoBehaviour
{
    [SerializeField] private Button _closeBtn, _exitBtn;
    private CanvasGroup _canvasGroup;

    private bool _isOpen = false;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _exitBtn.onClick.AddListener(HandleExitGame);
        _closeBtn.onClick.AddListener(HandleCloseWindow);
    }

    private void HandleCloseWindow()
    {
        _isOpen = !_isOpen;
        OpenWindow(_isOpen);
    }

    private void HandleExitGame()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.GameManager.Shutdown();
        }
        
        ClientSingleton.Instance.GameManager.Disconnect();
    }

    private void Update()
    {
        CheckSystemInput();
    }

    private void CheckSystemInput()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            _isOpen = !_isOpen;
            OpenWindow(_isOpen);
        }
    }

    private void OpenWindow(bool isOpen)
    {
        float fadeValue = isOpen ? 1f : 0;

        _canvasGroup.DOFade(fadeValue, 0.5f);
        _canvasGroup.blocksRaycasts = isOpen;
        _canvasGroup.interactable = isOpen;
    }
}