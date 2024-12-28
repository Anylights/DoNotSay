using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport3_1 : TeleportPoint
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other); // 调用基类的传送逻辑
        if (other.CompareTag("Player"))
        {
            EventCenter.Instance.TriggerEvent("PlayerTransportedToTele0");
        }
    }
}
