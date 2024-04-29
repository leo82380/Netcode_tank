using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Message : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    public event Action<Message> OnRemoveEvent;

    public RectTransform rectTrm;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        rectTrm = transform as RectTransform;
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public string Text
    {
        get => _text.text;
        set => _text.text = value;
    }
    
    public void SetTextAndTimer(string text, float timer)
    {
        Text = text;
        StartCoroutine(DelayDestroyCoroutine(timer));
    }

    private IEnumerator DelayDestroyCoroutine(float timer)
    {
        yield return new WaitForSeconds(timer);
        DestroyMessage();
    }

    public void DestroyMessage()
    {
        rectTrm.DOKill();
        
        Sequence seq = DOTween.Sequence();
        float currentX = rectTrm.anchoredPosition.x;
        
        seq.Append(rectTrm.DOAnchorPosX(currentX + 1000, 0.5f));
        seq.Join(_canvasGroup.DOFade(0, 0.5f));

        seq.AppendCallback(() =>
        {
            OnRemoveEvent?.Invoke(this);
            rectTrm.DOKill();
            Destroy(gameObject);
        });
    }
    
    public void MoveToPosition(Vector2 anchorPosition, float timer)
    {
        rectTrm.DOAnchorPos(anchorPosition, timer);
    }
}
