using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JoinCodeDisplay : MonoBehaviour
{
    private TextMeshProUGUI _joinCode;

    private void Awake()
    {
        _joinCode = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        _joinCode.text = HostSingleton.Instance.GameManager.JoinCode;
    }
}
