using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinCodePanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField _joinCodeText;
    [SerializeField] private Button _joinBtn;

    private void Awake()
    {
        _joinBtn.onClick.AddListener(HandleJoinBtnClick);
    }

    private async void HandleJoinBtnClick()
    {
        string joinCode = _joinCodeText.text;
        await ClientSingleton.Instance.GameManager.StartClientWithJoinCode(joinCode);
    }
}
