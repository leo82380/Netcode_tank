using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageSystem : MonoBehaviour
{
    public static MessageSystem Instance;

    [SerializeField] private Message _prefab;
    
    private List<Message> _messageList = new List<Message>();
    private RectTransform _rectTrm;

    private void Awake()
    {
        Instance = this;
        _rectTrm = transform as RectTransform;
    }

    public void ShowText(string msg, float displayTime)
    {
        Message msgItem = Instantiate(_prefab, _rectTrm);
        msgItem.SetTextAndTimer(msg, displayTime);
        msgItem.OnRemoveEvent += HandleRemoveEvent;
        _messageList.Add(msgItem);
        ReOrderingMessage(false);
    }


    private void HandleRemoveEvent(Message message)
    {
        _messageList.Remove(message);
        ReOrderingMessage(true);
    }
    
    private void ReOrderingMessage(bool withTween)
    {
        float delta = 10f;
        float currentHeight = delta;

        foreach (Message m in _messageList)
        {
            float height = m.rectTrm.sizeDelta.y;
            if (withTween)
            {
                m.MoveToPosition(new Vector2(0, -currentHeight), 0.2f);
            }
            else
            {
                m.rectTrm.anchoredPosition = new Vector2(0, -currentHeight);
            }
            
            currentHeight += height + delta;
        }
    }
}
