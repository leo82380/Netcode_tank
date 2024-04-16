using System.Text.RegularExpressions;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanelUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TMP_InputField _inputName;
    [SerializeField] private Button _btnLogin;

    private void Awake()
    {
        _btnLogin.interactable = false;
        _btnLogin.onClick.AddListener(HandleLoginBtnClick);
        _inputName.onValueChanged.AddListener(ValidateUsername);
    }

    private void ValidateUsername(string name)
    {
        // 영문 또는 숫자로 띄어쓰기 없이 3~5글자로 입력하면 성공
        Regex regex = new Regex("^[a-zA-Z0-9]{3,5}$");
        bool success = regex.IsMatch(name);
        _btnLogin.interactable = success;
    }

    private void HandleLoginBtnClick()
    {
        ClientSingleton.Instance.GameManager.SetPlayerName(_inputName.text);
        _canvasGroup.DOFade(0, 0.3f).OnComplete(() =>
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        });
    }
}
