using System;
using System.Collections.Generic;
using UnityEngine;

public class EventCenter : MonoBehaviour
{
    private static EventCenter instance;
    public static EventCenter Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("EventCenter").AddComponent<EventCenter>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    private Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();

    // 订阅无参数事件
    public void Subscribe(string eventType, Action listener)
    {
        if (eventTable.ContainsKey(eventType))
        {
            eventTable[eventType] = Delegate.Combine(eventTable[eventType], listener);
        }
        else
        {
            eventTable[eventType] = listener;
        }
    }

    // 订阅带参数事件
    public void Subscribe<T>(string eventType, Action<T> listener)
    {
        if (eventTable.ContainsKey(eventType))
        {
            eventTable[eventType] = Delegate.Combine(eventTable[eventType], listener);
        }
        else
        {
            eventTable[eventType] = listener;
        }
    }

    // 取消订阅无参数事件
    public void Unsubscribe(string eventType, Action listener)
    {
        if (eventTable.ContainsKey(eventType))
        {
            eventTable[eventType] = Delegate.Remove(eventTable[eventType], listener);
            if (eventTable[eventType] == null)
            {
                eventTable.Remove(eventType);
            }
        }
    }

    // 取消订阅带参数事件
    public void Unsubscribe<T>(string eventType, Action<T> listener)
    {
        if (eventTable.ContainsKey(eventType))
        {
            eventTable[eventType] = Delegate.Remove(eventTable[eventType], listener);
            if (eventTable[eventType] == null)
            {
                eventTable.Remove(eventType);
            }
        }
    }

    // 触发无参数事件
    public void TriggerEvent(string eventType)
    {
        if (eventTable.ContainsKey(eventType))
        {
            (eventTable[eventType] as Action)?.Invoke();
        }
    }

    // 触发带参数事件
    public void TriggerEvent<T>(string eventType, T arg)
    {
        if (eventTable.ContainsKey(eventType))
        {
            (eventTable[eventType] as Action<T>)?.Invoke(arg);
        }
    }
}